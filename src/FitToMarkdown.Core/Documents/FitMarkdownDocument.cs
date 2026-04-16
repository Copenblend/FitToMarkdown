using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.Summaries;

namespace FitToMarkdown.Core.Documents;

/// <summary>
/// Represents the markdown-ready projection derived from one parsed FIT file.
/// </summary>
public sealed record FitMarkdownDocument
{
    /// <summary>The source parsed FIT file document.</summary>
    public FitFileDocument Source { get; init; } = new();

    /// <summary>The document title.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>The heading timestamp in UTC.</summary>
    public DateTimeOffset? HeadingTimestampUtc { get; init; }

    /// <summary>The YAML frontmatter field set.</summary>
    public FitFrontmatter Frontmatter { get; init; } = new();

    /// <summary>The overview metrics section.</summary>
    public FitOverviewMetrics Overview { get; init; } = new();

    /// <summary>Per-session markdown-ready sections.</summary>
    public IReadOnlyList<SessionSection> SessionSections { get; init; } = [];

    /// <summary>Global record statistics summary across all sessions.</summary>
    public RecordStatisticsSummary? GlobalRecordSummary { get; init; }

    /// <summary>Global downsampled record rows across all sessions.</summary>
    public IReadOnlyList<SampledTimeSeriesRow> GlobalRecordSamples { get; init; } = [];

    /// <summary>Heart-rate zone summary, if available.</summary>
    public HeartRateZoneSummary? HeartRateZones { get; init; }

    /// <summary>HRV summary, if available.</summary>
    public HrvSummary? HrvSummary { get; init; }

    /// <summary>Device summary rows.</summary>
    public IReadOnlyList<DeviceSummaryRow> Devices { get; init; } = [];

    /// <summary>Event timeline items.</summary>
    public IReadOnlyList<EventTimelineItem> Events { get; init; } = [];

    /// <summary>Workout step rows.</summary>
    public IReadOnlyList<WorkoutStepRow> WorkoutSteps { get; init; } = [];

    /// <summary>Course point rows.</summary>
    public IReadOnlyList<CoursePointRow> CoursePoints { get; init; } = [];

    /// <summary>Segment-lap table rows.</summary>
    public IReadOnlyList<SegmentLapTableRow> SegmentLapRows { get; init; } = [];

    /// <summary>Grouped developer-field sections.</summary>
    public IReadOnlyList<DeveloperFieldGroup> DeveloperFieldGroups { get; init; } = [];

    /// <summary>Explicit section render decisions.</summary>
    public IReadOnlyList<SectionRenderDecision> SectionDecisions { get; init; } = [];
}
