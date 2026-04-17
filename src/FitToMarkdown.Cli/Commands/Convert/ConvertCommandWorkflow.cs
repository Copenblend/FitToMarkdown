using FitToMarkdown.Cli.Configuration;
using FitToMarkdown.Cli.Models;
using FitToMarkdown.Cli.Rendering;
using FitToMarkdown.Cli.Services;
using Spectre.Console;

namespace FitToMarkdown.Cli.Commands.Convert;

internal sealed class ConvertCommandWorkflow : IConvertCommandWorkflow
{
    private readonly IAnsiConsole _console;
    private readonly ICliFileSystem _fileSystem;
    private readonly InputPathResolver _pathResolver;
    private readonly FitFileDiscoveryService _discoveryService;
    private readonly ConvertPromptService _promptService;
    private readonly MarkdownOptionsFactory _optionsFactory;
    private readonly OutputPathResolver _outputPathResolver;
    private readonly ConversionBatchRunner _batchRunner;
    private readonly ConvertSummaryRenderer _summaryRenderer;
    private readonly CliExceptionRenderer _exceptionRenderer;

    internal ConvertCommandWorkflow(
        IAnsiConsole console,
        ICliFileSystem fileSystem,
        InputPathResolver pathResolver,
        FitFileDiscoveryService discoveryService,
        ConvertPromptService promptService,
        MarkdownOptionsFactory optionsFactory,
        OutputPathResolver outputPathResolver,
        ConversionBatchRunner batchRunner,
        ConvertSummaryRenderer summaryRenderer,
        CliExceptionRenderer exceptionRenderer)
    {
        _console = console;
        _fileSystem = fileSystem;
        _pathResolver = pathResolver;
        _discoveryService = discoveryService;
        _promptService = promptService;
        _optionsFactory = optionsFactory;
        _outputPathResolver = outputPathResolver;
        _batchRunner = batchRunner;
        _summaryRenderer = summaryRenderer;
        _exceptionRenderer = exceptionRenderer;
    }

    public async Task<int> ExecuteAsync(ConvertCommandSettings settings, CancellationToken cancellationToken = default)
    {
        // 1. Path resolution
        var target = await _pathResolver.ResolveAsync(settings.Path, settings.NoInteraction, cancellationToken)
            .ConfigureAwait(false);

        if (target is null)
        {
            _exceptionRenderer.RenderInvalidInput("No valid .fit file or directory specified.");
            return CliExitCodes.InvalidInput;
        }

        // 2. Discover files
        IReadOnlyList<DiscoveredFitFile> files;

        if (target.Kind == InputTargetKind.File)
        {
            files =
            [
                new DiscoveredFitFile(
                    FullPath: target.FullPath,
                    RelativeGroupName: "(root)",
                    FileName: Path.GetFileName(target.FullPath),
                    LengthBytes: _fileSystem.GetFileLength(target.FullPath),
                    LastWriteTimeUtc: _fileSystem.GetLastWriteTimeUtc(target.FullPath)),
            ];
        }
        else
        {
            // 3. Directory scan with spinner
            files = await _console.Status()
                .Spinner(Spinner.Known.Dots)
                .SpinnerStyle(CliTheme.Primary)
                .StartAsync("Scanning for .fit files...", async _ =>
                    await _discoveryService.DiscoverAsync(target.FullPath, cancellationToken).ConfigureAwait(false))
                .ConfigureAwait(false);

            if (files.Count == 0)
            {
                _exceptionRenderer.RenderInvalidInput("No .fit files found.");
                return CliExitCodes.InvalidInput;
            }

            // 4. File selection (interactive)
            if (!settings.NoInteraction)
            {
                files = await _promptService.PromptForFilesAsync(target.FullPath, files, cancellationToken)
                    .ConfigureAwait(false);

                var confirmed = await _promptService.ConfirmBatchAsync(files.Count, cancellationToken)
                    .ConfigureAwait(false);

                if (!confirmed)
                {
                    _console.MarkupLine("[dim]Cancelled.[/]");
                    return CliExitCodes.Success;
                }
            }
        }

        // 5. Output configuration
        var defaultOutputDir = _outputPathResolver.GetDefaultOutputDirectory(target);
        string outputDirectory;

        if (!string.IsNullOrWhiteSpace(settings.OutputDirectory))
        {
            outputDirectory = Path.GetFullPath(settings.OutputDirectory);
        }
        else if (!settings.NoInteraction)
        {
            outputDirectory = await _promptService.PromptForOutputDirectoryAsync(defaultOutputDir, cancellationToken)
                .ConfigureAwait(false);
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

        // 6. Overwrite mode
        ConvertOverwriteMode overwriteMode;

        if (!string.IsNullOrWhiteSpace(settings.Overwrite))
        {
            overwriteMode = settings.Overwrite.ToLowerInvariant() switch
            {
                "skip" => ConvertOverwriteMode.Skip,
                "ask-each" => ConvertOverwriteMode.AskEach,
                _ => ConvertOverwriteMode.Overwrite,
            };
        }
        else if (!settings.NoInteraction)
        {
            overwriteMode = await _promptService.PromptForOverwriteModeAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        else
        {
            overwriteMode = ConvertOverwriteMode.Overwrite;
        }

        // 7. Build execution plan
        var plan = new ConvertExecutionPlan
        {
            SourceTarget = target,
            Files = files,
            OutputDirectory = outputDirectory,
            OverwriteMode = overwriteMode,
            NoInteraction = settings.NoInteraction,
        };

        // 8. Run conversion with progress
        var summary = await _console.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new SpinnerColumn(),
                new ElapsedTimeColumn())
            .StartAsync(async ctx =>
            {
                var total = plan.Files.Count;
                var progressTask = ctx.AddTask("Starting conversion...", maxValue: total);
                var started = 0;

                var result = await _batchRunner.RunAsync(plan, cancellationToken, (index, fileName) =>
                {
                    if (started > 0)
                        progressTask.Increment(1);
                    started++;
                    progressTask.Description = $"Converting {index}/{total}: {fileName}";
                }).ConfigureAwait(false);

                progressTask.Value = total;
                return result;
            }).ConfigureAwait(false);

        // 9. Render summary
        _summaryRenderer.Render(summary);

        // 10. Return exit code
        if (summary.FailedCount == 0)
            return CliExitCodes.Success;

        return summary.ConvertedCount > 0
            ? CliExitCodes.PartialSuccess
            : CliExitCodes.TotalFailure;
    }
}
