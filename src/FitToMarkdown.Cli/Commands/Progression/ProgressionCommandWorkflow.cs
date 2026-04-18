using FitToMarkdown.Cli.Configuration;
using FitToMarkdown.Cli.Models;
using FitToMarkdown.Cli.Rendering;
using FitToMarkdown.Cli.Services;
using Spectre.Console;

namespace FitToMarkdown.Cli.Commands.Progression;

internal sealed class ProgressionCommandWorkflow : IProgressionCommandWorkflow
{
    private readonly IAnsiConsole _console;
    private readonly ICliFileSystem _fileSystem;
    private readonly InputPathResolver _pathResolver;
    private readonly FitFileDiscoveryService _discoveryService;
    private readonly SportScanService _sportScanService;
    private readonly ProgressionDocumentBuilder _documentBuilder;
    private readonly ProgressionDocumentInserter _documentInserter;
    private readonly ProgressionSummaryRenderer _summaryRenderer;
    private readonly CliExceptionRenderer _exceptionRenderer;

    internal ProgressionCommandWorkflow(
        IAnsiConsole console,
        ICliFileSystem fileSystem,
        InputPathResolver pathResolver,
        FitFileDiscoveryService discoveryService,
        SportScanService sportScanService,
        ProgressionDocumentBuilder documentBuilder,
        ProgressionDocumentInserter documentInserter,
        ProgressionSummaryRenderer summaryRenderer,
        CliExceptionRenderer exceptionRenderer)
    {
        _console = console;
        _fileSystem = fileSystem;
        _pathResolver = pathResolver;
        _discoveryService = discoveryService;
        _sportScanService = sportScanService;
        _documentBuilder = documentBuilder;
        _documentInserter = documentInserter;
        _summaryRenderer = summaryRenderer;
        _exceptionRenderer = exceptionRenderer;
    }

    public async Task<int> ExecuteAsync(ProgressionCommandSettings settings, CancellationToken cancellationToken = default)
    {
        // Detect mode: add-to vs build
        if (settings.AddFile is not null && settings.ProgressionFile is not null)
        {
            return await HandleAddToProgressionAsync(settings, cancellationToken).ConfigureAwait(false);
        }

        return await HandleBuildProgressionAsync(settings, cancellationToken).ConfigureAwait(false);
    }

    private async Task<int> HandleBuildProgressionAsync(ProgressionCommandSettings settings, CancellationToken cancellationToken)
    {
        // 1. Resolve input path (must be a directory)
        var target = await _pathResolver.ResolveAsync(settings.Path, settings.NoInteraction, cancellationToken)
            .ConfigureAwait(false);

        if (target is null)
        {
            _exceptionRenderer.RenderInvalidInput("No valid directory specified.");
            return CliExitCodes.InvalidInput;
        }

        if (target.Kind != InputTargetKind.Directory)
        {
            if (target.Kind == InputTargetKind.File && !settings.NoInteraction)
            {
                return await HandleInteractiveAddToProgressionAsync(target.FullPath, cancellationToken).ConfigureAwait(false);
            }

            _exceptionRenderer.RenderInvalidInput("Progression requires a directory of .fit files. Use --add to add a single file to an existing progression.");
            return CliExitCodes.InvalidInput;
        }

        // 2. Discover FIT files
        var files = await _console.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(CliTheme.Primary)
            .StartAsync("Scanning for .fit files...", async _ =>
                await _discoveryService.DiscoverAsync(target.FullPath, cancellationToken).ConfigureAwait(false))
            .ConfigureAwait(false);

        if (files.Count == 0)
        {
            _exceptionRenderer.RenderInvalidInput("No .fit files found in the specified directory.");
            return CliExitCodes.InvalidInput;
        }

        // 3. Scan for sports with progress
        IReadOnlyList<SportGroup> sportGroups;
        sportGroups = await _console.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new SpinnerColumn())
            .StartAsync(async ctx =>
            {
                var progressTask = ctx.AddTask("Scanning sports...", maxValue: files.Count);
                var groups = await _sportScanService.ScanAndGroupBySportAsync(files, cancellationToken, count =>
                {
                    progressTask.Value = count;
                    progressTask.Description = $"Scanning sports... ({count}/{files.Count})";
                }).ConfigureAwait(false);
                progressTask.Value = files.Count;
                return groups;
            }).ConfigureAwait(false);

        if (sportGroups.Count == 0)
        {
            _exceptionRenderer.RenderInvalidInput("No sport data found in any .fit files.");
            return CliExitCodes.InvalidInput;
        }

        // 4. Select sport(s)
        List<SportGroup> selectedGroups;

        if (!string.IsNullOrWhiteSpace(settings.Sport))
        {
            var match = sportGroups.FirstOrDefault(g =>
                string.Equals(g.DisplayName, settings.Sport, StringComparison.OrdinalIgnoreCase)
                || string.Equals(g.SportName, settings.Sport, StringComparison.OrdinalIgnoreCase));

            if (match is null)
            {
                _exceptionRenderer.RenderInvalidInput(
                    $"Sport '{settings.Sport}' not found. Available: {string.Join(", ", sportGroups.Select(g => g.DisplayName))}");
                return CliExitCodes.InvalidInput;
            }

            selectedGroups = [match];
        }
        else if (!settings.NoInteraction)
        {
            var primaryGroup = _console.Prompt(
                new SelectionPrompt<SportGroup>()
                    .Title("Select a sport to build progression for:")
                    .PageSize(15)
                    .WrapAround()
                    .HighlightStyle(CliTheme.HighlightStyle)
                    .UseConverter(g => $"{CliMarkup.Escape(g.DisplayName)} [dim]({g.Files.Count} files)[/]")
                    .AddChoices(sportGroups));

            selectedGroups = [primaryGroup];

            // Offer to include additional sports
            var otherGroups = sportGroups.Where(g => g != primaryGroup).ToList();
            if (otherGroups.Count > 0)
            {
                var includeMore = _console.Confirm(
                    "Include other sports/sub-sports in this progression?",
                    defaultValue: false);

                if (includeMore)
                {
                    var additional = _console.Prompt(
                        new MultiSelectionPrompt<SportGroup>()
                            .Title("Toggle sports to include:")
                            .PageSize(15)
                            .WrapAround()
                            .Required()
                            .HighlightStyle(CliTheme.HighlightStyle)
                            .InstructionsText("[dim](Press [blue]<space>[/] to toggle, [green]<enter>[/] to confirm)[/]")
                            .UseConverter(g => $"{CliMarkup.Escape(g.DisplayName)} [dim]({g.Files.Count} files)[/]")
                            .AddChoices(otherGroups));

                    selectedGroups.AddRange(additional);
                }
            }
        }
        else
        {
            _exceptionRenderer.RenderInvalidInput("The '--sport' option is required when using '--no-interaction'.");
            return CliExitCodes.InvalidInput;
        }

        var totalFileCount = selectedGroups.Sum(g => g.Files.Count);
        var progressionLabel = selectedGroups.Count > 1
            ? "Activity"
            : selectedGroups[0].DisplayName;

        // 5. Confirm
        if (!settings.NoInteraction)
        {
            var sportListDisplay = string.Join(", ", selectedGroups.Select(g => CliMarkup.Escape(g.DisplayName)));
            var confirmed = _console.Confirm(
                $"Build progression for [cyan]{sportListDisplay}[/] with [cyan]{totalFileCount}[/] files?",
                defaultValue: true);

            if (!confirmed)
            {
                _console.MarkupLine("[dim]Cancelled.[/]");
                return CliExitCodes.Success;
            }
        }

        // 6. Resolve output directory
        var defaultOutputDir = target.FullPath;
        string outputDirectory;

        if (!string.IsNullOrWhiteSpace(settings.OutputDirectory))
        {
            outputDirectory = Path.GetFullPath(settings.OutputDirectory);
        }
        else if (!settings.NoInteraction)
        {
            outputDirectory = _console.Prompt(
                new TextPrompt<string>("Output directory:")
                    .DefaultValue(defaultOutputDir)
                    .ShowDefaultValue()
                    .Validate(value => !string.IsNullOrWhiteSpace(value), "Output directory cannot be empty."));
            outputDirectory = Path.GetFullPath(outputDirectory);
        }
        else
        {
            outputDirectory = defaultOutputDir;
        }

        try
        {
            _fileSystem.CreateDirectory(outputDirectory);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        {
            _exceptionRenderer.RenderInvalidInput($"Cannot create output directory: {ex.Message}");
            return CliExitCodes.InvalidInput;
        }

        // 7. Build progression with progress
        (string content, ProgressionBuildResult result) = await _console.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new SpinnerColumn(),
                new ElapsedTimeColumn())
            .StartAsync(async ctx =>
            {
                var progressTask = ctx.AddTask("Building progression...", maxValue: totalFileCount);
                var buildResult = await _documentBuilder.BuildAsync(selectedGroups, progressionLabel, outputDirectory, cancellationToken, (count, fileName) =>
                {
                    progressTask.Value = count;
                    progressTask.Description = $"Processing {count}/{totalFileCount}: {fileName}";
                }).ConfigureAwait(false);
                progressTask.Value = totalFileCount;
                return buildResult;
            }).ConfigureAwait(false);

        // 8. Write output file
        await _fileSystem.WriteAllTextAsync(result.OutputPath, content, cancellationToken).ConfigureAwait(false);

        // 9. Render summary
        _summaryRenderer.RenderBuildSummary(result);

        return result.FailedFiles.Count == 0 ? CliExitCodes.Success : CliExitCodes.PartialSuccess;
    }

    private async Task<int> HandleAddToProgressionAsync(ProgressionCommandSettings settings, CancellationToken cancellationToken)
    {
        var addFilePath = Path.GetFullPath(settings.AddFile!);
        var progressionFilePath = Path.GetFullPath(settings.ProgressionFile!);

        // Validate the files exist
        if (!_fileSystem.FileExists(addFilePath))
        {
            _exceptionRenderer.RenderInvalidInput($"FIT file not found: {addFilePath}");
            return CliExitCodes.InvalidInput;
        }

        if (!_fileSystem.FileExists(progressionFilePath))
        {
            _exceptionRenderer.RenderInvalidInput($"Progression file not found: {progressionFilePath}");
            return CliExitCodes.InvalidInput;
        }

        // 1. Parse and render the single activity
        _console.MarkupLine("[dim]Parsing FIT file...[/]");
        var (activityContent, timestamp, error) = await _documentBuilder.BuildSingleActivityAsync(addFilePath, cancellationToken)
            .ConfigureAwait(false);

        if (activityContent is null || error is not null)
        {
            _exceptionRenderer.RenderInvalidInput($"Failed to process FIT file: {error ?? "Unknown error"}");
            return CliExitCodes.InvalidInput;
        }

        // 2. Read existing progression
        var existingContent = await _fileSystem.ReadAllTextAsync(progressionFilePath, cancellationToken).ConfigureAwait(false);

        // 3. Insert new activity chronologically
        var mergedContent = _documentInserter.InsertChronologically(
            existingContent,
            activityContent,
            timestamp ?? DateTimeOffset.UtcNow);

        // 4. Write updated progression
        await _fileSystem.WriteAllTextAsync(progressionFilePath, mergedContent, cancellationToken).ConfigureAwait(false);

        _console.MarkupLine($"[green]Added activity to[/] {CliMarkup.Escape(progressionFilePath)}");
        if (timestamp.HasValue)
        {
            _console.MarkupLine($"[dim]Activity timestamp: {timestamp.Value:yyyy-MM-dd HH:mm:ss} UTC[/]");
        }

        return CliExitCodes.Success;
    }

    private async Task<int> HandleInteractiveAddToProgressionAsync(string fitFilePath, CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(fitFilePath)!;

        // Find progression files in the same directory
        var progressionFiles = _fileSystem
            .EnumerateFiles(directory, "*_Progression_*.md", SearchOption.TopDirectoryOnly)
            .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (progressionFiles.Count == 0)
        {
            _exceptionRenderer.RenderInvalidInput(
                $"No progression files found in '{directory}'. Build a progression first with a directory of .fit files.");
            return CliExitCodes.InvalidInput;
        }

        // Let user pick a progression file
        string selectedProgressionFile;
        if (progressionFiles.Count == 1)
        {
            var name = Path.GetFileName(progressionFiles[0]);
            var confirmed = _console.Confirm(
                $"Add to [cyan]{CliMarkup.Escape(name)}[/]?",
                defaultValue: true);
            if (!confirmed)
            {
                _console.MarkupLine("[dim]Cancelled.[/]");
                return CliExitCodes.Success;
            }

            selectedProgressionFile = progressionFiles[0];
        }
        else
        {
            selectedProgressionFile = _console.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a progression file to add to:")
                    .PageSize(15)
                    .WrapAround()
                    .HighlightStyle(CliTheme.HighlightStyle)
                    .UseConverter(Path.GetFileName!)
                    .AddChoices(progressionFiles));
        }

        // 1. Parse and render the single activity
        _console.MarkupLine("[dim]Parsing FIT file...[/]");
        var (activityContent, timestamp, error) = await _documentBuilder
            .BuildSingleActivityAsync(fitFilePath, cancellationToken)
            .ConfigureAwait(false);

        if (activityContent is null || error is not null)
        {
            _exceptionRenderer.RenderInvalidInput($"Failed to process FIT file: {error ?? "Unknown error"}");
            return CliExitCodes.InvalidInput;
        }

        // 2. Read existing progression
        var existingContent = await _fileSystem
            .ReadAllTextAsync(selectedProgressionFile, cancellationToken)
            .ConfigureAwait(false);

        // 3. Insert new activity chronologically
        var mergedContent = _documentInserter.InsertChronologically(
            existingContent,
            activityContent,
            timestamp ?? DateTimeOffset.UtcNow);

        // 4. Write updated progression
        await _fileSystem
            .WriteAllTextAsync(selectedProgressionFile, mergedContent, cancellationToken)
            .ConfigureAwait(false);

        _console.MarkupLine($"[green]Added activity to[/] {CliMarkup.Escape(Path.GetFileName(selectedProgressionFile))}");
        if (timestamp.HasValue)
        {
            _console.MarkupLine($"[dim]Activity timestamp: {timestamp.Value:yyyy-MM-dd HH:mm:ss} UTC[/]");
        }

        return CliExitCodes.Success;
    }
}
