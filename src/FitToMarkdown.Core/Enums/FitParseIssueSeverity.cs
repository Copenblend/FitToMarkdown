namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Distinguishes recoverable issues from hard failures.
/// </summary>
public enum FitParseIssueSeverity : byte
{
    /// <summary>A non-fatal issue that allowed parsing to continue.</summary>
    Warning = 0,

    /// <summary>A fatal issue that prevented further parsing.</summary>
    Error = 1,
}
