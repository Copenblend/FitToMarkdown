using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Parsing;

namespace FitToMarkdown.Cli.Services;

internal sealed class MarkdownOptionsFactory
{
    internal MarkdownDocumentOptions CreateFor(FitParseResult parseResult)
    {
        var tokenBudget = parseResult.Metadata.IsMultiSport
            ? 6000
            : parseResult.Metadata.HasPoolSwim
                ? 5000
                : 4000;

        return new MarkdownDocumentOptions
        {
            IncludeFrontmatter = true,
            IncludeDeveloperFields = true,
            IncludeRecordSamples = true,
            IncludeDataQualitySection = true,
            PreferCompactTables = true,
            CollapseSingleLapSections = true,
            MaximumRecordSampleRows = 60,
            ApproximateTokenBudget = tokenBudget,
        };
    }
}
