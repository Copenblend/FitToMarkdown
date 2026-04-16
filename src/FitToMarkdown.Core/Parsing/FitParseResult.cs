using FitToMarkdown.Core.Documents;

namespace FitToMarkdown.Core.Parsing;

/// <summary>
/// Represents the full parser result envelope for one FIT file.
/// </summary>
public sealed record FitParseResult
{
    /// <summary>The parsed FIT file document, or null if parsing failed fatally.</summary>
    public FitFileDocument? Document { get; init; }

    /// <summary>The metadata describing parser state, recovery actions, and coverage.</summary>
    public FitParseMetadata Metadata { get; init; } = new();

    /// <summary>The collection of recoverable or contextual issues encountered during parsing.</summary>
    public IReadOnlyList<FitParseIssue> Issues { get; init; } = [];

    /// <summary>The fatal error that prevented document construction, if any.</summary>
    public FitParseError? FatalError { get; init; }
}
