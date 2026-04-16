using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized SegmentLapMesg from a FIT file.
/// </summary>
public sealed record FitSegmentLap
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>Zero-based segment index within the activity.</summary>
    public int SegmentIndex { get; init; }

    /// <summary>Start and end time range for this segment lap.</summary>
    public FitTimeRange Range { get; init; } = new();

    /// <summary>Name of the segment.</summary>
    public string? SegmentName { get; init; }

    /// <summary>Primary sport type.</summary>
    public FitSport? Sport { get; init; }

    /// <summary>Sub-sport type.</summary>
    public FitSubSport? SubSport { get; init; }

    /// <summary>Aggregated summary metrics for the segment lap.</summary>
    public FitSummaryMetrics Metrics { get; init; } = new();

    /// <summary>Segment completion status.</summary>
    public string? Status { get; init; }

    /// <summary>Unique identifier for the segment.</summary>
    public string? Uuid { get; init; }

    /// <summary>Time spent on this segment.</summary>
    public TimeSpan? SegmentTime { get; init; }

    /// <summary>Developer field values attached to this message.</summary>
    public IReadOnlyList<FitDeveloperFieldValue> DeveloperFields { get; init; } = [];
}
