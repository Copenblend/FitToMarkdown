using FitToMarkdown.Core.Enums;

namespace FitToMarkdown.Core.Parsing;

/// <summary>
/// Represents explicit parser state, recovery state, and coverage metadata.
/// </summary>
public sealed record FitParseMetadata
{
    /// <summary>The overall parse status indicating success, partial success, or failure.</summary>
    public FitParseStatus Status { get; init; }

    /// <summary>The ordering mode used to arrange decoded messages.</summary>
    public FitMessageOrderingMode OrderingMode { get; init; }

    /// <summary>The recovery mode applied during parsing.</summary>
    public FitRecoveryMode RecoveryMode { get; init; }

    /// <summary>Whether the parse result contains only partial data.</summary>
    public bool IsPartial { get; init; }

    /// <summary>Whether truncation was detected in the source file.</summary>
    public bool TruncationDetected { get; init; }

    /// <summary>Whether a decode fault occurred during parsing.</summary>
    public bool HadDecodeFault { get; init; }

    /// <summary>Whether a synthetic activity message was injected during recovery.</summary>
    public bool UsedSyntheticActivity { get; init; }

    /// <summary>Whether synthetic session messages were injected during recovery.</summary>
    public bool UsedSyntheticSessions { get; init; }

    /// <summary>Whether the file contains multiple sport sessions.</summary>
    public bool IsMultiSport { get; init; }

    /// <summary>Whether the file contains pool swim data.</summary>
    public bool HasPoolSwim { get; init; }

    /// <summary>Whether the file contains developer-defined fields.</summary>
    public bool HasDeveloperFields { get; init; }

    /// <summary>The total number of messages successfully decoded.</summary>
    public int DecodedMessageCount { get; init; }

    /// <summary>The total number of messages dropped during parsing.</summary>
    public int DroppedMessageCount { get; init; }

    /// <summary>The UTC timestamp of the first decoded message, if available.</summary>
    public DateTimeOffset? FirstTimestampUtc { get; init; }

    /// <summary>The UTC timestamp of the last decoded message, if available.</summary>
    public DateTimeOffset? LastTimestampUtc { get; init; }
}
