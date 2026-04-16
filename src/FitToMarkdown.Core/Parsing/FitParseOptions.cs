using FitToMarkdown.Core.Enums;

namespace FitToMarkdown.Core.Parsing;

/// <summary>
/// Represents parser behavior flags without leaking Dynastream-specific concerns into Core.
/// </summary>
public sealed record FitParseOptions
{
    /// <summary>Whether to allow partial data extraction from corrupt or truncated files.</summary>
    public bool AllowPartialExtraction { get; init; }

    /// <summary>Whether to attempt recovery of truncated activity files by synthesizing missing summary messages.</summary>
    public bool RecoverTruncatedActivityFiles { get; init; }

    /// <summary>Whether to validate file integrity via CRC for non-activity files.</summary>
    public bool ValidateIntegrityForNonActivityFiles { get; init; }

    /// <summary>Whether to perform metadata-only inspection without full decode.</summary>
    public bool MetadataOnly { get; init; }

    /// <summary>Whether to resolve developer field definitions and associate values.</summary>
    public bool ResolveDeveloperFields { get; init; }

    /// <summary>Whether to merge compressed heart-rate data from HrMesg into RecordMesg timestamps.</summary>
    public bool MergeCompressedHeartRateIntoRecords { get; init; }

    /// <summary>Whether to include monitoring messages in the parsed output.</summary>
    public bool IncludeMonitoringMessages { get; init; }
}
