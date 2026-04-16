namespace FitToMarkdown.Core.ValueObjects;

/// <summary>
/// Preserves parse-order identity, message index, and recovery provenance for one normalized FIT message.
/// </summary>
public sealed record FitMessageIdentity
{
    /// <summary>The zero-based order in which this message was decoded from the FIT stream.</summary>
    public int ParseSequence { get; init; }

    /// <summary>The optional message index field value from the FIT message.</summary>
    public ushort? MessageIndex { get; init; }

    /// <summary>Indicates whether this message was synthesized rather than decoded from the file.</summary>
    public bool IsSynthetic { get; init; }

    /// <summary>Indicates whether this message was recovered after a decode fault.</summary>
    public bool RecoveredAfterDecodeFault { get; init; }
}
