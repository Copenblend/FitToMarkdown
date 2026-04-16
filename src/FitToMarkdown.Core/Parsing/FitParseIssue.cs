using FitToMarkdown.Core.Enums;

namespace FitToMarkdown.Core.Parsing;

/// <summary>
/// Represents one recoverable or contextual parser issue.
/// </summary>
public sealed record FitParseIssue
{
    /// <summary>The severity level of the parse issue.</summary>
    public FitParseIssueSeverity Severity { get; init; }

    /// <summary>A machine-readable code identifying the issue category.</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>A human-readable description of the issue.</summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>The name of the FIT message where the issue occurred, if applicable.</summary>
    public string? MessageName { get; init; }

    /// <summary>The sequential position in the decode stream where the issue was detected.</summary>
    public int? ParseSequence { get; init; }

    /// <summary>The FIT message index associated with the issue, if applicable.</summary>
    public ushort? MessageIndex { get; init; }

    /// <summary>The UTC timestamp of the message where the issue occurred, if available.</summary>
    public DateTimeOffset? TimestampUtc { get; init; }

    /// <summary>Whether the issue was recoverable without data loss.</summary>
    public bool Recoverable { get; init; }
}
