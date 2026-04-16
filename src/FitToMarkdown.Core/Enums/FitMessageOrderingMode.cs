namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Records the raw message-ordering pattern observed during decode.
/// </summary>
public enum FitMessageOrderingMode : byte
{
    /// <summary>Summary messages (session, lap) appear before their detail records.</summary>
    SummaryFirst = 0,

    /// <summary>Summary messages appear after their detail records.</summary>
    SummaryLast = 1,

    /// <summary>Summary and detail messages are interleaved in no consistent order.</summary>
    Mixed = 2,

    /// <summary>Ordering pattern could not be determined.</summary>
    Unknown = 3,
}
