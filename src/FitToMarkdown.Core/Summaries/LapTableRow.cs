using FitToMarkdown.Core.Enums;

namespace FitToMarkdown.Core.Summaries;

/// <summary>
/// Represents one row in the lap comparison table.
/// </summary>
public sealed record LapTableRow
{
    /// <summary>One-based lap number.</summary>
    public int LapNumber { get; init; }

    /// <summary>Distance covered in the lap in meters.</summary>
    public double? DistanceMeters { get; init; }

    /// <summary>Duration of the lap.</summary>
    public TimeSpan? Duration { get; init; }

    /// <summary>Average speed in meters per second.</summary>
    public double? AverageSpeedMetersPerSecond { get; init; }

    /// <summary>Average pace in seconds per kilometer.</summary>
    public double? AveragePaceSecondsPerKilometer { get; init; }

    /// <summary>Average heart rate in beats per minute.</summary>
    public byte? AverageHeartRateBpm { get; init; }

    /// <summary>Maximum heart rate in beats per minute.</summary>
    public byte? MaximumHeartRateBpm { get; init; }

    /// <summary>Average cadence in revolutions per minute.</summary>
    public double? AverageCadenceRpm { get; init; }

    /// <summary>Average power output in watts.</summary>
    public ushort? AveragePowerWatts { get; init; }

    /// <summary>Trigger that caused this lap.</summary>
    public FitLapTrigger? Trigger { get; init; }
}
