using FitToMarkdown.Cli.Models;
using Spectre.Console;

namespace FitToMarkdown.Cli.Rendering;

internal sealed class ConvertSummaryRenderer
{
    private readonly IAnsiConsole _console;

    internal ConvertSummaryRenderer(IAnsiConsole console)
    {
        _console = console;
    }

    internal void Render(ConvertBatchSummary summary)
    {
        _console.Write(new Rule("Conversion Complete").RuleStyle(CliTheme.PrimaryBold));

        var table = new Table()
            .RoundedBorder()
            .BorderColor(CliTheme.PrimaryColor)
            .HideHeaders()
            .AddColumn("Key")
            .AddColumn("Value");

        table.AddRow("Processed", summary.ProcessedCount.ToString());
        table.AddRow("Converted", $"[green]{summary.ConvertedCount}[/]");

        if (summary.FailedCount > 0)
            table.AddRow("Failed", $"[red]{summary.FailedCount}[/]");

        if (summary.SkippedCount > 0)
            table.AddRow("Skipped", $"[yellow]{summary.SkippedCount}[/]");

        table.AddRow("Elapsed", FormatElapsed(summary.TotalElapsed));
        table.AddRow("Output", CliMarkup.Escape(summary.OutputDirectory));

        _console.Write(table);

        string statusLabel = (summary.ConvertedCount, summary.FailedCount, summary.SkippedCount) switch
        {
            (> 0, 0, _) => "[green]All files converted successfully.[/]",
            (> 0, > 0, _) => "[yellow]Some files failed during conversion.[/]",
            (0, > 0, _) => "[red]All files failed during conversion.[/]",
            (0, 0, > 0) => "[yellow]All files were skipped.[/]",
            _ => string.Empty,
        };

        if (!string.IsNullOrEmpty(statusLabel))
            _console.MarkupLine(statusLabel);

        var failures = summary.Results
            .Where(r => r.Status == ConvertFileResultStatus.Failed)
            .ToList();

        if (failures.Count > 0)
        {
            var failureTable = new Table()
                .RoundedBorder()
                .BorderColor(CliTheme.ErrorColor)
                .AddColumn("File")
                .AddColumn("Reason");

            foreach (var result in failures)
            {
                failureTable.AddRow(
                    CliMarkup.Escape(Path.GetFileName(result.SourcePath)),
                    result.FailureReason is not null
                        ? CliMarkup.Escape(result.FailureReason)
                        : CliMarkup.DimOrFallback(null));
            }

            _console.Write(failureTable);
        }
    }

    private static string FormatElapsed(TimeSpan elapsed)
    {
        return elapsed.TotalHours >= 1
            ? elapsed.ToString(@"hh\:mm\:ss\.ff")
            : elapsed.ToString(@"mm\:ss\.ff");
    }
}
