namespace FitToMarkdown.Core.Summaries;

/// <summary>
/// Represents the complete markdown-ready heart-rate-zone section.
/// </summary>
public sealed record HeartRateZoneSummary
{
    /// <summary>Method used to determine heart-rate zones.</summary>
    public string Method { get; init; } = string.Empty;

    /// <summary>Maximum heart rate in beats per minute.</summary>
    public byte? MaximumHeartRateBpm { get; init; }

    /// <summary>Threshold heart rate in beats per minute.</summary>
    public byte? ThresholdHeartRateBpm { get; init; }

    /// <summary>Heart-rate zone rows.</summary>
    public IReadOnlyList<HeartRateZoneRow> Zones { get; init; } = [];
}
