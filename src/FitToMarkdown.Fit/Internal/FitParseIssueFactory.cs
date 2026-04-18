using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Parsing;

namespace FitToMarkdown.Fit.Internal;

/// <summary>
/// Creates common <see cref="FitParseIssue"/> instances with standardized codes and messages.
/// </summary>
internal static class FitParseIssueFactory
{
    /// <summary>Creates an issue for an invalid FIT file header.</summary>
    /// <returns>An error-level parse issue.</returns>
    public static FitParseIssue InvalidHeader() => new()
    {
        Severity = FitParseIssueSeverity.Error,
        Code = "fit.invalid-header",
        Message = "Stream does not contain a valid FIT file header.",
        Recoverable = false,
    };

    /// <summary>Creates an issue for a decode fault (FitException during Read).</summary>
    /// <param name="message">The exception message.</param>
    /// <returns>A warning-level parse issue indicating partial data may be available.</returns>
    public static FitParseIssue DecodeFault(string message) => new()
    {
        Severity = FitParseIssueSeverity.Warning,
        Code = "fit.decode-fault",
        Message = $"Decode fault encountered: {message}",
        Recoverable = true,
    };

    /// <summary>Creates an issue for a failed CRC integrity check.</summary>
    /// <returns>A warning-level parse issue.</returns>
    public static FitParseIssue IntegrityCheckFailed() => new()
    {
        Severity = FitParseIssueSeverity.Warning,
        Code = "fit.integrity-failed",
        Message = "FIT file CRC integrity check failed.",
        Recoverable = true,
    };

    /// <summary>Creates an issue for a missing FileIdMesg.</summary>
    /// <returns>An error-level parse issue.</returns>
    public static FitParseIssue MissingFileId() => new()
    {
        Severity = FitParseIssueSeverity.Error,
        Code = "fit.missing-file-id",
        Message = "No FileIdMesg found in the decoded data.",
        Recoverable = false,
    };

    /// <summary>Creates an issue when a synthetic ActivityMesg was created during recovery.</summary>
    /// <returns>A warning-level parse issue.</returns>
    public static FitParseIssue SyntheticActivityCreated() => new()
    {
        Severity = FitParseIssueSeverity.Warning,
        Code = "fit.synthetic-activity",
        Message = "A synthetic ActivityMesg was created because the file was truncated before the activity summary.",
        Recoverable = true,
    };

    /// <summary>Creates an issue when a synthetic SessionMesg was created during recovery.</summary>
    /// <returns>A warning-level parse issue.</returns>
    public static FitParseIssue SyntheticSessionCreated() => new()
    {
        Severity = FitParseIssueSeverity.Warning,
        Code = "fit.synthetic-session",
        Message = "A synthetic SessionMesg was created because no session summary was decoded.",
        Recoverable = true,
    };

    /// <summary>Creates an issue when a synthetic LapMesg was created during recovery.</summary>
    /// <returns>A warning-level parse issue.</returns>
    public static FitParseIssue SyntheticLapCreated() => new()
    {
        Severity = FitParseIssueSeverity.Warning,
        Code = "fit.synthetic-lap",
        Message = "A synthetic LapMesg was created because no lap summary was decoded.",
        Recoverable = true,
    };

    /// <summary>Creates an issue for an unrecognized file type.</summary>
    /// <param name="rawValue">The raw file type byte value.</param>
    /// <returns>A warning-level parse issue.</returns>
    public static FitParseIssue UnrecognizedFileType(byte? rawValue) => new()
    {
        Severity = FitParseIssueSeverity.Warning,
        Code = "fit.unknown-file-type",
        Message = $"Unrecognized FIT file type: {rawValue?.ToString() ?? "null"}.",
        Recoverable = true,
    };

    /// <summary>Creates an issue for a fault in a chained FIT file segment.</summary>
    /// <param name="segmentIndex">The zero-based index of the faulted segment.</param>
    /// <param name="message">The fault description.</param>
    /// <returns>A warning-level parse issue.</returns>
    public static FitParseIssue ChainedSegmentFault(int segmentIndex, string message) => new()
    {
        Severity = FitParseIssueSeverity.Warning,
        Code = "fit.chained-segment-fault",
        Message = $"Chained segment {segmentIndex} fault: {message}",
        Recoverable = true,
    };

    /// <summary>Creates an issue for an unresolved developer field definition.</summary>
    /// <param name="devDataIndex">The developer data index.</param>
    /// <param name="fieldDefNum">The field definition number.</param>
    /// <returns>A warning-level parse issue.</returns>
    public static FitParseIssue UnresolvedDeveloperField(byte devDataIndex, byte fieldDefNum) => new()
    {
        Severity = FitParseIssueSeverity.Warning,
        Code = "fit.unresolved-developer-field",
        Message = $"Developer field (devDataIndex={devDataIndex}, fieldDefNum={fieldDefNum}) could not be resolved to a definition.",
        Recoverable = true,
    };

    /// <summary>Creates an issue for a duplicate developer field definition key.</summary>
    /// <param name="devDataIndex">The developer data index.</param>
    /// <param name="fieldDefNum">The field definition number.</param>
    /// <returns>A warning-level parse issue.</returns>
    public static FitParseIssue DuplicateDeveloperDefinition(byte devDataIndex, byte fieldDefNum) => new()
    {
        Severity = FitParseIssueSeverity.Warning,
        Code = "fit.duplicate-developer-definition",
        Message = $"Duplicate developer field definition for (devDataIndex={devDataIndex}, fieldDefNum={fieldDefNum}); last definition wins.",
        Recoverable = true,
    };

    /// <summary>Creates an issue for ambiguous grouping during session assignment.</summary>
    /// <param name="detail">A description of the ambiguity.</param>
    /// <returns>A warning-level parse issue.</returns>
    public static FitParseIssue GroupingAmbiguous(string detail) => new()
    {
        Severity = FitParseIssueSeverity.Warning,
        Code = "fit.grouping-ambiguous",
        Message = $"Grouping ambiguity: {detail}",
        Recoverable = true,
    };

    /// <summary>Creates an issue for partial metadata summary extraction.</summary>
    /// <param name="detail">A description of the missing metadata.</param>
    /// <returns>A warning-level parse issue.</returns>
    public static FitParseIssue MetadataSummaryPartial(string detail) => new()
    {
        Severity = FitParseIssueSeverity.Warning,
        Code = "fit.metadata-summary-partial",
        Message = $"Metadata summary partial: {detail}",
        Recoverable = true,
    };

    /// <summary>Creates an issue for a monitoring file integrity failure.</summary>
    /// <param name="message">The failure description.</param>
    /// <returns>An error-level parse issue.</returns>
    public static FitParseIssue MonitoringIntegrityFailed(string message) => new()
    {
        Severity = FitParseIssueSeverity.Error,
        Code = "fit.monitoring-integrity-failed",
        Message = $"Monitoring file integrity failed: {message}",
        Recoverable = false,
    };

    /// <summary>Creates an issue when a plugin fell back to a default behavior.</summary>
    /// <param name="message">The fallback description.</param>
    /// <returns>A warning-level parse issue.</returns>
    public static FitParseIssue PluginFallback(string message) => new()
    {
        Severity = FitParseIssueSeverity.Warning,
        Code = "fit.plugin-fallback",
        Message = $"Plugin fallback: {message}",
        Recoverable = true,
    };
}
