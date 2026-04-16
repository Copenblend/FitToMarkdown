namespace FitToMarkdown.Core.Abstractions;

/// <summary>
/// Represents the projection rules used to build a markdown-ready document.
/// </summary>
public sealed record MarkdownDocumentOptions
{
    /// <summary>Whether to include YAML frontmatter in the output.</summary>
    public bool IncludeFrontmatter { get; init; }

    /// <summary>Whether to include developer fields in the output.</summary>
    public bool IncludeDeveloperFields { get; init; }

    /// <summary>Whether to include record sample data in the output.</summary>
    public bool IncludeRecordSamples { get; init; }

    /// <summary>Whether to include a data quality section in the output.</summary>
    public bool IncludeDataQualitySection { get; init; }

    /// <summary>Whether to prefer compact tables over detailed tables.</summary>
    public bool PreferCompactTables { get; init; }

    /// <summary>Whether to collapse single-lap sessions into the session section.</summary>
    public bool CollapseSingleLapSections { get; init; }

    /// <summary>Maximum number of record sample rows to include.</summary>
    public int MaximumRecordSampleRows { get; init; }

    /// <summary>Approximate token budget for the output document.</summary>
    public int ApproximateTokenBudget { get; init; }
}
