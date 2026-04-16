using FitToMarkdown.Core.Models;

namespace FitToMarkdown.Core.Parsing;

/// <summary>
/// Represents the result of metadata-only inspection for one FIT file.
/// </summary>
public sealed record FitMetadataInspectionResult
{
    /// <summary>The file info summary extracted during inspection, or null if inspection failed.</summary>
    public FitFileInfoSummary? Summary { get; init; }

    /// <summary>The metadata describing parser state, recovery actions, and coverage.</summary>
    public FitParseMetadata Metadata { get; init; } = new();

    /// <summary>The collection of recoverable or contextual issues encountered during inspection.</summary>
    public IReadOnlyList<FitParseIssue> Issues { get; init; } = [];

    /// <summary>The fatal error that prevented inspection, if any.</summary>
    public FitParseError? FatalError { get; init; }
}
