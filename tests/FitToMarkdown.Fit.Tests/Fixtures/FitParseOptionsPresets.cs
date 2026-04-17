using FitToMarkdown.Core.Parsing;

namespace FitToMarkdown.Fit.Tests.Fixtures;

/// <summary>
/// Centralizes common parser-option combinations used across unit and integration tests.
/// </summary>
internal static class FitParseOptionsPresets
{
    internal static FitParseOptions DefaultParse() => new()
    {
        AllowPartialExtraction = true,
        RecoverTruncatedActivityFiles = true,
        ValidateIntegrityForNonActivityFiles = true,
        ResolveDeveloperFields = true,
        MergeCompressedHeartRateIntoRecords = true,
        IncludeMonitoringMessages = true,
    };

    internal static FitParseOptions WithoutCompressedHeartRateMerge() => new()
    {
        AllowPartialExtraction = true,
        RecoverTruncatedActivityFiles = true,
        ValidateIntegrityForNonActivityFiles = true,
        ResolveDeveloperFields = true,
        MergeCompressedHeartRateIntoRecords = false,
        IncludeMonitoringMessages = true,
    };

    internal static FitParseOptions WithoutTruncatedRecovery() => new()
    {
        AllowPartialExtraction = true,
        RecoverTruncatedActivityFiles = false,
        ValidateIntegrityForNonActivityFiles = true,
        ResolveDeveloperFields = true,
        MergeCompressedHeartRateIntoRecords = true,
        IncludeMonitoringMessages = true,
    };

    internal static FitParseOptions WithoutNonActivityIntegrityValidation() => new()
    {
        AllowPartialExtraction = true,
        RecoverTruncatedActivityFiles = true,
        ValidateIntegrityForNonActivityFiles = false,
        ResolveDeveloperFields = true,
        MergeCompressedHeartRateIntoRecords = true,
        IncludeMonitoringMessages = true,
    };

    internal static FitParseOptions MetadataOnly() => new()
    {
        MetadataOnly = true,
    };
}
