using FitToMarkdown.Core.Models;

namespace FitToMarkdown.Core.Summaries;

/// <summary>
/// Represents one markdown-ready session section.
/// </summary>
public sealed record SessionSection
{
    /// <summary>The normalized session data.</summary>
    public FitSession Session { get; init; } = new();

    /// <summary>Rows for the lap comparison table.</summary>
    public IReadOnlyList<LapTableRow> LapRows { get; init; } = [];

    /// <summary>Rows for the pool-swim length table.</summary>
    public IReadOnlyList<LengthTableRow> LengthRows { get; init; } = [];

    /// <summary>Statistical summary over the record series.</summary>
    public RecordStatisticsSummary? RecordSummary { get; init; }

    /// <summary>Downsampled record rows for markdown CSV output.</summary>
    public IReadOnlyList<SampledTimeSeriesRow> RecordSamples { get; init; } = [];

    /// <summary>Derived pause intervals within this session.</summary>
    public IReadOnlyList<PauseInterval> Pauses { get; init; } = [];

    /// <summary>Explicit render or omit decisions for each section.</summary>
    public IReadOnlyList<SectionRenderDecision> SectionDecisions { get; init; } = [];
}
