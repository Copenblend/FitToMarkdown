using FitToMarkdown.Cli.Models;
using FitToMarkdown.Cli.Rendering;
using FluentAssertions;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Rendering;

public sealed class ConvertSummaryRendererTests
{
    private readonly TestConsole _console;
    private readonly ConvertSummaryRenderer _renderer;

    public ConvertSummaryRendererTests()
    {
        _console = new TestConsole();
        _console.Profile.Width = 120;
        _renderer = new ConvertSummaryRenderer(_console);
    }

    [Fact]
    public void Render_outputs_Conversion_Complete_rule()
    {
        var summary = CreateSummary(processed: 1, converted: 1);

        _renderer.Render(summary);

        _console.Output.Should().Contain("Conversion Complete");
    }

    [Fact]
    public void Render_outputs_summary_counts()
    {
        var summary = CreateSummary(processed: 5, converted: 3, failed: 1, skipped: 1);

        _renderer.Render(summary);

        _console.Output.Should().Contain("5");
        _console.Output.Should().Contain("3");
    }

    [Fact]
    public void Render_outputs_failure_table_when_failures_exist()
    {
        var summary = CreateSummary(processed: 2, converted: 1, failed: 1,
            results:
            [
                new ConvertFileResult
                {
                    SourcePath = @"C:\test\good.fit",
                    OutputPath = @"C:\test\good.md",
                    Status = ConvertFileResultStatus.Converted,
                },
                new ConvertFileResult
                {
                    SourcePath = @"C:\test\bad.fit",
                    Status = ConvertFileResultStatus.Failed,
                    FailureReason = "CRC mismatch",
                },
            ]);

        _renderer.Render(summary);

        _console.Output.Should().Contain("CRC mismatch");
    }

    [Fact]
    public void Render_omits_skipped_row_when_no_skips()
    {
        var summary = CreateSummary(processed: 2, converted: 2, skipped: 0);

        _renderer.Render(summary);

        _console.Output.Should().NotContain("Skipped");
    }

    private static ConvertBatchSummary CreateSummary(
        int processed = 0,
        int converted = 0,
        int failed = 0,
        int skipped = 0,
        IReadOnlyList<ConvertFileResult>? results = null)
    {
        return new ConvertBatchSummary
        {
            ProcessedCount = processed,
            ConvertedCount = converted,
            FailedCount = failed,
            SkippedCount = skipped,
            TotalElapsed = TimeSpan.FromSeconds(2.5),
            OutputDirectory = @"C:\test\output",
            Results = results ?? [],
        };
    }
}
