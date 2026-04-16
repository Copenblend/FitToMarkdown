namespace FitToMarkdown.Core.Enums;

/// <summary>
/// High-level parser outcome.
/// </summary>
public enum FitParseStatus : byte
{
    /// <summary>File decoded completely with no issues.</summary>
    Succeeded = 0,

    /// <summary>File decoded but one or more non-fatal issues were encountered.</summary>
    PartiallySucceeded = 1,

    /// <summary>File could not be decoded at all.</summary>
    Failed = 2,
}
