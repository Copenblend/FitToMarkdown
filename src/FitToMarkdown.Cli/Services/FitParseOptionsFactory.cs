using FitToMarkdown.Core.Parsing;

namespace FitToMarkdown.Cli.Services;

internal sealed class FitParseOptionsFactory
{
    internal FitParseOptions CreateForConvert()
    {
        return new FitParseOptions
        {
            AllowPartialExtraction = true,
            RecoverTruncatedActivityFiles = true,
            ValidateIntegrityForNonActivityFiles = true,
            MetadataOnly = false,
            ResolveDeveloperFields = true,
            MergeCompressedHeartRateIntoRecords = true,
            IncludeMonitoringMessages = true,
        };
    }
}
