using FitToMarkdown.Core.Enums;

namespace FitToMarkdown.Core.Summaries;

/// <summary>
/// Represents one heart-rate-zone row.
/// </summary>
public sealed record HeartRateZoneRow
{
    /// <summary>Zone classification.</summary>
    public FitZoneKind Zone { get; init; }

    /// <summary>Lower bound of the zone in beats per minute.</summary>
    public byte? LowerBoundBpm { get; init; }

    /// <summary>Upper bound of the zone in beats per minute.</summary>
    public byte? UpperBoundBpm { get; init; }

    /// <summary>Total time spent in this zone.</summary>
    public TimeSpan? TimeInZone { get; init; }

    /// <summary>Percentage of total activity time spent in this zone.</summary>
    public double? PercentOfTotalTime { get; init; }
}
