using FitToMarkdown.Cli.Models;
using FitToMarkdown.Cli.Rendering;
using Spectre.Console;

namespace FitToMarkdown.Cli.Services;

internal sealed class ConvertPromptService
{
    private readonly IAnsiConsole _console;

    internal ConvertPromptService(IAnsiConsole console)
    {
        _console = console;
    }

    internal async Task<IReadOnlyList<DiscoveredFitFile>> PromptForFilesAsync(
        string rootDirectory,
        IReadOnlyList<DiscoveredFitFile> discoveredFiles,
        CancellationToken cancellationToken)
    {
        var prompt = new MultiSelectionPrompt<DiscoveredFitFile>()
            .Title($"Select FIT files to convert from [cyan]{CliMarkup.Escape(rootDirectory)}[/]:")
            .PageSize(15)
            .WrapAround()
            .Required()
            .HighlightStyle(CliTheme.HighlightStyle)
            .InstructionsText("[grey](Use [blue]Space[/] to toggle, [green]Enter[/] to confirm)[/]")
            .UseConverter(f => $"{CliMarkup.Escape(f.FileName)} [dim]({FormatFileSize(f.LengthBytes)})[/] [dim]{f.LastWriteTimeUtc:yyyy-MM-dd}[/]");

        prompt.AddChoices(discoveredFiles);

        foreach (var file in discoveredFiles)
        {
            prompt.Select(file);
        }

        var selected = await prompt.ShowAsync(_console, cancellationToken).ConfigureAwait(false);
        return selected;
    }

    internal async Task<bool> ConfirmBatchAsync(int count, CancellationToken cancellationToken)
    {
        var prompt = new ConfirmationPrompt($"Convert [cyan]{count}[/] file{(count != 1 ? "s" : "")}?")
        {
            DefaultValue = true,
        };

        return await prompt.ShowAsync(_console, cancellationToken).ConfigureAwait(false);
    }

    internal async Task<string> PromptForOutputDirectoryAsync(string defaultDirectory, CancellationToken cancellationToken)
    {
        var prompt = new TextPrompt<string>("Output directory:")
            .DefaultValue(defaultDirectory)
            .ShowDefaultValue()
            .Validate(value => !string.IsNullOrWhiteSpace(value), "Output directory cannot be empty.");

        return await prompt.ShowAsync(_console, cancellationToken).ConfigureAwait(false);
    }

    internal async Task<ConvertOverwriteMode> PromptForOverwriteModeAsync(CancellationToken cancellationToken)
    {
        var prompt = new SelectionPrompt<string>()
            .Title("How should existing files be handled?")
            .PageSize(5)
            .WrapAround()
            .HighlightStyle(CliTheme.HighlightStyle)
            .AddChoices("Ask Each", "Skip", "Overwrite");

        var selected = await prompt.ShowAsync(_console, cancellationToken).ConfigureAwait(false);

        return selected switch
        {
            "Skip" => ConvertOverwriteMode.Skip,
            "Overwrite" => ConvertOverwriteMode.Overwrite,
            _ => ConvertOverwriteMode.AskEach,
        };
    }

    internal async Task<bool> ConfirmOverwriteAsync(string outputPath, CancellationToken cancellationToken)
    {
        var prompt = new ConfirmationPrompt($"Overwrite [yellow]{CliMarkup.Escape(outputPath)}[/]?")
        {
            DefaultValue = false,
        };

        return await prompt.ShowAsync(_console, cancellationToken).ConfigureAwait(false);
    }

    private static string FormatFileSize(long bytes)
    {
        return bytes switch
        {
            < 1024 => $"{bytes} B",
            < 1024 * 1024 => $"{bytes / 1024.0:F1} KB",
            _ => $"{bytes / (1024.0 * 1024.0):F1} MB",
        };
    }
}
