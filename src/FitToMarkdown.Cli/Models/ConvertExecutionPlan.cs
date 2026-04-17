namespace FitToMarkdown.Cli.Models;

internal sealed record ConvertExecutionPlan
{
    public InputTarget SourceTarget { get; init; } = null!;
    public IReadOnlyList<DiscoveredFitFile> Files { get; init; } = [];
    public string OutputDirectory { get; init; } = string.Empty;
    public ConvertOverwriteMode OverwriteMode { get; init; }
    public bool NoInteraction { get; init; }
}
