using FitToMarkdown.Core.Enums;

namespace FitToMarkdown.Core.Summaries;

/// <summary>
/// Represents the normalized overview metrics displayed below the document heading.
/// </summary>
public sealed record FitOverviewMetrics
{
    /// <summary>Primary sport type for the activity.</summary>
    public FitSport? PrimarySport { get; init; }

    /// <summary>Primary sub-sport type for the activity.</summary>
    public FitSubSport? PrimarySubSport { get; init; }

    /// <summary>Whether the activity contains multiple sports.</summary>
    public bool IsMultiSport { get; init; }

    /// <summary>UTC start time of the activity.</summary>
    public DateTimeOffset? StartTimeUtc { get; init; }

    /// <summary>Total elapsed time including pauses.</summary>
    public TimeSpan? TotalElapsedTime { get; init; }

    /// <summary>Total timer time excluding pauses.</summary>
    public TimeSpan? TotalTimerTime { get; init; }

    /// <summary>Total distance in meters.</summary>
    public double? TotalDistanceMeters { get; init; }

    /// <summary>Total calories expended.</summary>
    public ushort? Calories { get; init; }

    /// <summary>Average heart rate in beats per minute.</summary>
    public byte? AverageHeartRateBpm { get; init; }

    /// <summary>Maximum heart rate in beats per minute.</summary>
    public byte? MaximumHeartRateBpm { get; init; }

    /// <summary>Average speed in meters per second.</summary>
    public double? AverageSpeedMetersPerSecond { get; init; }

    /// <summary>Average pace in seconds per kilometer.</summary>
    public double? AveragePaceSecondsPerKilometer { get; init; }

    /// <summary>Total ascent in meters.</summary>
    public ushort? TotalAscentMeters { get; init; }

    /// <summary>Total descent in meters.</summary>
    public ushort? TotalDescentMeters { get; init; }

    /// <summary>Number of sessions in the activity.</summary>
    public int SessionCount { get; init; }

    /// <summary>Number of laps in the activity.</summary>
    public int LapCount { get; init; }

    /// <summary>Number of swim lengths in the activity.</summary>
    public int LengthCount { get; init; }

    /// <summary>Number of records in the activity.</summary>
    public int RecordCount { get; init; }
}
