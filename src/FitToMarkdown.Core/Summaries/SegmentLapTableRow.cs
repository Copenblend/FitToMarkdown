namespace FitToMarkdown.Core.Summaries;

/// <summary>
/// Represents one row in the segment-lap comparison table.
/// </summary>
public sealed record SegmentLapTableRow
{
    /// <summary>One-based segment number.</summary>
    public int SegmentNumber { get; init; }

    /// <summary>Name of the segment.</summary>
    public string? Name { get; init; }

    /// <summary>Distance covered in the segment in meters.</summary>
    public double? DistanceMeters { get; init; }

    /// <summary>Duration of the segment.</summary>
    public TimeSpan? Duration { get; init; }

    /// <summary>Average heart rate in beats per minute.</summary>
    public byte? AverageHeartRateBpm { get; init; }

    /// <summary>Average cadence in revolutions per minute.</summary>
    public double? AverageCadenceRpm { get; init; }

    /// <summary>Average power output in watts.</summary>
    public ushort? AveragePowerWatts { get; init; }

    /// <summary>Status of the segment attempt.</summary>
    public string? Status { get; init; }
}
