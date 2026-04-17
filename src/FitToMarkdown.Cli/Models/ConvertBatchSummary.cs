namespace FitToMarkdown.Cli.Models;

internal sealed record ConvertBatchSummary
{
    public int ProcessedCount { get; init; }
    public int ConvertedCount { get; init; }
    public int FailedCount { get; init; }
    public int SkippedCount { get; init; }
    public TimeSpan TotalElapsed { get; init; }
    public string OutputDirectory { get; init; } = string.Empty;
    public IReadOnlyList<ConvertFileResult> Results { get; init; } = [];
}
