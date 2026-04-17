using FitToMarkdown.Cli.Models;
using FitToMarkdown.Cli.Rendering;
using Spectre.Console;

namespace FitToMarkdown.Cli.Services;

internal sealed class InteractivePathBrowser
{
    private readonly IAnsiConsole _console;
    private readonly ICliFileSystem _fileSystem;

    internal InteractivePathBrowser(IAnsiConsole console, ICliFileSystem fileSystem)
    {
        _console = console;
        _fileSystem = fileSystem;
    }

    internal async Task<InputTarget?> BrowseAsync(string startDirectory, CancellationToken cancellationToken)
    {
        var currentDirectory = startDirectory;

        while (true)
        {
            var entries = BuildEntries(currentDirectory);

            var prompt = new SelectionPrompt<BrowserEntry>()
                .Title("Choose a [cyan]FIT file[/] or folder:")
                .PageSize(15)
                .WrapAround()
                .HighlightStyle(CliTheme.HighlightStyle)
                .UseConverter(entry => entry.DisplayText);

            if (entries.Count > 10)
            {
                prompt.EnableSearch();
            }

            prompt.AddChoices(entries);

            var selected = await prompt.ShowAsync(_console, cancellationToken).ConfigureAwait(false);

            switch (selected.Kind)
            {
                case BrowserEntryKind.ParentDirectory:
                    currentDirectory = Directory.GetParent(currentDirectory)?.FullName ?? currentDirectory;
                    continue;

                case BrowserEntryKind.UseCurrentDirectory:
                    return new InputTarget(currentDirectory, InputTargetKind.Directory);

                case BrowserEntryKind.Directory:
                    currentDirectory = selected.FullPath;
                    continue;

                case BrowserEntryKind.FitFile:
                    return new InputTarget(selected.FullPath, InputTargetKind.File);

                default:
                    return null;
            }
        }
    }

    private List<BrowserEntry> BuildEntries(string currentDirectory)
    {
        var entries = new List<BrowserEntry>();

        var root = Path.GetPathRoot(currentDirectory);
        if (!string.Equals(currentDirectory, root, StringComparison.OrdinalIgnoreCase))
        {
            entries.Add(new BrowserEntry(
                Directory.GetParent(currentDirectory)?.FullName ?? currentDirectory,
                "..",
                BrowserEntryKind.ParentDirectory,
                "Navigation"));
        }

        entries.Add(new BrowserEntry(
            currentDirectory,
            "(Use current directory)",
            BrowserEntryKind.UseCurrentDirectory,
            "Navigation"));

        try
        {
            var directories = _fileSystem.EnumerateDirectories(currentDirectory)
                .OrderBy(d => Path.GetFileName(d), StringComparer.OrdinalIgnoreCase)
                .Select(d => new BrowserEntry(d, Path.GetFileName(d) + Path.DirectorySeparatorChar, BrowserEntryKind.Directory, "Folders"));

            entries.AddRange(directories);

            var files = _fileSystem.EnumerateFiles(currentDirectory, "*.fit", SearchOption.TopDirectoryOnly)
                .Where(f => string.Equals(Path.GetExtension(f), ".fit", StringComparison.OrdinalIgnoreCase))
                .OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
                .Select(f => new BrowserEntry(f, Path.GetFileName(f), BrowserEntryKind.FitFile, "FIT Files"));

            entries.AddRange(files);
        }
        catch (Exception ex)
        {
            _console.MarkupLine($"[yellow]Could not read directory: {CliMarkup.Escape(ex.Message)}[/]");
        }

        return entries;
    }
}
