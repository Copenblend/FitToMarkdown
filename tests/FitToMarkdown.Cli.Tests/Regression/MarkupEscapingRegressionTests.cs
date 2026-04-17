using FitToMarkdown.Cli.Models;
using FitToMarkdown.Cli.Rendering;
using FluentAssertions;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Regression;

public sealed class MarkupEscapingRegressionTests
{
    private readonly TestConsole _console;

    public MarkupEscapingRegressionTests()
    {
        _console = new TestConsole();
        _console.Profile.Width = 120;
    }

    [Fact]
    public void File_name_with_brackets_does_not_crash_summary_renderer()
    {
        var renderer = new ConvertSummaryRenderer(_console);
        var summary = new ConvertBatchSummary
        {
            ProcessedCount = 1,
            ConvertedCount = 1,
            FailedCount = 0,
            SkippedCount = 0,
            TotalElapsed = TimeSpan.FromSeconds(1),
            OutputDirectory = @"C:\output",
            Results =
            [
                new ConvertFileResult
                {
                    SourcePath = @"C:\data\[test]_activity.fit",
                    OutputPath = @"C:\output\[test]_activity.md",
                    Status = ConvertFileResultStatus.Converted,
                },
            ],
        };

        var act = () => renderer.Render(summary);

        act.Should().NotThrow();
    }

    [Fact]
    public void Path_with_curly_braces_does_not_crash_summary_renderer()
    {
        var renderer = new ConvertSummaryRenderer(_console);
        var summary = new ConvertBatchSummary
        {
            ProcessedCount = 1,
            ConvertedCount = 0,
            FailedCount = 1,
            SkippedCount = 0,
            TotalElapsed = TimeSpan.FromSeconds(1),
            OutputDirectory = @"C:\output",
            Results =
            [
                new ConvertFileResult
                {
                    SourcePath = @"C:\data\{guid}_activity.fit",
                    Status = ConvertFileResultStatus.Failed,
                    FailureReason = "Parse error",
                },
            ],
        };

        var act = () => renderer.Render(summary);

        act.Should().NotThrow();
    }

    [Fact]
    public void Failure_reason_with_special_chars_renders_safely()
    {
        var renderer = new ConvertSummaryRenderer(_console);
        var summary = new ConvertBatchSummary
        {
            ProcessedCount = 1,
            ConvertedCount = 0,
            FailedCount = 1,
            SkippedCount = 0,
            TotalElapsed = TimeSpan.FromSeconds(1),
            OutputDirectory = @"C:\output",
            Results =
            [
                new ConvertFileResult
                {
                    SourcePath = @"C:\data\activity.fit",
                    Status = ConvertFileResultStatus.Failed,
                    FailureReason = "Error at [offset 0x0E]: unexpected {token} in <header>",
                },
            ],
        };

        var act = () => renderer.Render(summary);

        act.Should().NotThrow();
    }
}
