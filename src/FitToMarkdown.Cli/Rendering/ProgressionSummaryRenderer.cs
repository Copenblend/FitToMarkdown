using FitToMarkdown.Cli.Models;
using Spectre.Console;

namespace FitToMarkdown.Cli.Rendering;

internal sealed class ProgressionSummaryRenderer
{
    private readonly IAnsiConsole _console;

    internal ProgressionSummaryRenderer(IAnsiConsole console)
    {
        _console = console;
    }

    internal void RenderBuildSummary(ProgressionBuildResult result)
    {
        _console.Write(new Rule("Progression Complete").RuleStyle(CliTheme.PrimaryBold));

        var table = new Table()
            .RoundedBorder()
            .BorderColor(CliTheme.PrimaryColor)
            .HideHeaders()
            .AddColumn("Key")
            .AddColumn("Value");

        table.AddRow("Sport", CliMarkup.Escape(result.SportName));
        table.AddRow("Activities", $"[green]{result.ActivityCount}[/]");

        if (result.FailedFiles.Count > 0)
            table.AddRow("Failed", $"[red]{result.FailedFiles.Count}[/]");

        table.AddRow("Elapsed", FormatElapsed(result.ElapsedTime));
        table.AddRow("Output", CliMarkup.Escape(result.OutputPath));

        _console.Write(table);

        if (result.Warnings.Count > 0)
        {
            _console.MarkupLine("[yellow]Warnings:[/]");
            foreach (var warning in result.Warnings)
            {
                _console.MarkupLine($"  [yellow]•[/] {CliMarkup.Escape(warning)}");
            }
        }

        string statusLabel = (result.ActivityCount, result.FailedFiles.Count) switch
        {
            (> 0, 0) => "[green]Progression built successfully.[/]",
            (> 0, > 0) => "[yellow]Progression built with some failures.[/]",
            (0, _) => "[red]No activities could be processed.[/]",
            _ => string.Empty,
        };

        if (!string.IsNullOrEmpty(statusLabel))
            _console.MarkupLine(statusLabel);
    }

    private static string FormatElapsed(TimeSpan elapsed)
    {
        return elapsed.TotalHours >= 1
            ? elapsed.ToString(@"hh\:mm\:ss\.ff")
            : elapsed.TotalMinutes >= 1
                ? elapsed.ToString(@"mm\:ss\.ff")
                : $"{elapsed.TotalSeconds:F2}s";
    }
}
