using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Parsing;

namespace FitToMarkdown.Fit.Internal;

/// <summary>
/// Creates common <see cref="FitParseIssue"/> instances with standardized codes and messages.
/// </summary>
internal static class FitParseIssueFactory
{
    /// <summary>Creates an issue for an invalid FIT file header.</summary>
    /// <returns>A warning-level parse issue.</returns>
    public static FitParseIssue InvalidHeader() => new()
    {
        Severity = FitParseIssueSeverity.Error,
        Code = "FIT_INVALID_HEADER",
        Message = "Stream does not contain a valid FIT file header.",
        Recoverable = false,
    };

    /// <summary>Creates an issue for a decode fault (FitException during Read).</summary>
    /// <param name="message">The exception message.</param>
    /// <returns>A warning-level parse issue indicating partial data may be available.</returns>
    public static FitParseIssue DecodeFault(string message) => new()
    {
        Severity = FitParseIssueSeverity.Warning,
        Code = "FIT_DECODE_FAULT",
        Message = $"Decode fault encountered: {message}",
        Recoverable = true,
    };

    /// <summary>Creates an issue for a failed CRC integrity check.</summary>
    /// <returns>A warning-level parse issue.</returns>
    public static FitParseIssue IntegrityCheckFailed() => new()
    {
        Severity = FitParseIssueSeverity.Warning,
        Code = "FIT_INTEGRITY_FAILED",
        Message = "FIT file CRC integrity check failed.",
        Recoverable = true,
    };

    /// <summary>Creates an issue for a missing FileIdMesg.</summary>
    /// <returns>An error-level parse issue.</returns>
    public static FitParseIssue MissingFileId() => new()
    {
        Severity = FitParseIssueSeverity.Error,
        Code = "FIT_MISSING_FILE_ID",
        Message = "No FileIdMesg found in the decoded data.",
        Recoverable = false,
    };

    /// <summary>Creates an issue when a synthetic ActivityMesg was created during recovery.</summary>
    /// <returns>A warning-level parse issue.</returns>
    public static FitParseIssue SyntheticActivityCreated() => new()
    {
        Severity = FitParseIssueSeverity.Warning,
        Code = "FIT_SYNTHETIC_ACTIVITY",
        Message = "A synthetic ActivityMesg was created because the file was truncated before the activity summary.",
        Recoverable = true,
    };

    /// <summary>Creates an issue when a synthetic SessionMesg was created during recovery.</summary>
    /// <returns>A warning-level parse issue.</returns>
    public static FitParseIssue SyntheticSessionCreated() => new()
    {
        Severity = FitParseIssueSeverity.Warning,
        Code = "FIT_SYNTHETIC_SESSION",
        Message = "A synthetic SessionMesg was created because no session summary was decoded.",
        Recoverable = true,
    };

    /// <summary>Creates an issue when a synthetic LapMesg was created during recovery.</summary>
    /// <returns>A warning-level parse issue.</returns>
    public static FitParseIssue SyntheticLapCreated() => new()
    {
        Severity = FitParseIssueSeverity.Warning,
        Code = "FIT_SYNTHETIC_LAP",
        Message = "A synthetic LapMesg was created because no lap summary was decoded.",
        Recoverable = true,
    };

    /// <summary>Creates an issue for an unrecognized file type.</summary>
    /// <param name="rawValue">The raw file type byte value.</param>
    /// <returns>A warning-level parse issue.</returns>
    public static FitParseIssue UnrecognizedFileType(byte? rawValue) => new()
    {
        Severity = FitParseIssueSeverity.Warning,
        Code = "FIT_UNKNOWN_FILE_TYPE",
        Message = $"Unrecognized FIT file type: {rawValue?.ToString() ?? "null"}.",
        Recoverable = true,
    };
}
