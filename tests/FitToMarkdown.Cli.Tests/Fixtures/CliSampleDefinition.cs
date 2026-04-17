namespace FitToMarkdown.Cli.Tests.Fixtures;

/// <summary>
/// Describes a CLI-facing sample category used for batch, info, and end-to-end scenarios.
/// </summary>
internal sealed record CliSampleDefinition(
    string Key,
    string RelativePath,
    string Category,
    bool IsValidFit,
    bool IsRecoverable);
