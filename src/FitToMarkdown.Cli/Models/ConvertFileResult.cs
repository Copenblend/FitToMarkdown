namespace FitToMarkdown.Cli.Models;

internal sealed record ConvertFileResult
{
    public string SourcePath { get; init; } = string.Empty;
    public string? OutputPath { get; init; }
    public ConvertFileResultStatus Status { get; init; }
    public string? FailureReason { get; init; }
    public TimeSpan ElapsedTime { get; init; }
}
