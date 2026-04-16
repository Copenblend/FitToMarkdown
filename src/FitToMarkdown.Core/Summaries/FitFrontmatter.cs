using FitToMarkdown.Core.Enums;

namespace FitToMarkdown.Core.Summaries;

/// <summary>
/// Represents the exact normalized frontmatter field set required by the markdown output specification.
/// </summary>
public sealed record FitFrontmatter
{
    /// <summary>The FIT file type.</summary>
    public FitFileType? FileType { get; init; }

    /// <summary>Primary sport type.</summary>
    public FitSport? Sport { get; init; }

    /// <summary>Sub-sport type.</summary>
    public FitSubSport? SubSport { get; init; }

    /// <summary>UTC timestamp when the file was created.</summary>
    public DateTimeOffset? TimeCreatedUtc { get; init; }

    /// <summary>Name of the device manufacturer.</summary>
    public string? ManufacturerName { get; init; }

    /// <summary>Name of the product or device model.</summary>
    public string? ProductName { get; init; }

    /// <summary>Device serial number.</summary>
    public uint? SerialNumber { get; init; }

    /// <summary>Total distance in meters.</summary>
    public double? DistanceMeters { get; init; }

    /// <summary>Total duration in seconds.</summary>
    public double? DurationSeconds { get; init; }

    /// <summary>Average heart rate in beats per minute.</summary>
    public byte? AverageHeartRateBpm { get; init; }

    /// <summary>Maximum heart rate in beats per minute.</summary>
    public byte? MaximumHeartRateBpm { get; init; }

    /// <summary>Average speed in meters per second.</summary>
    public double? AverageSpeedMetersPerSecond { get; init; }

    /// <summary>Average power output in watts.</summary>
    public ushort? AveragePowerWatts { get; init; }

    /// <summary>Total ascent in meters.</summary>
    public ushort? TotalAscentMeters { get; init; }

    /// <summary>Total descent in meters.</summary>
    public ushort? TotalDescentMeters { get; init; }

    /// <summary>Total calories expended.</summary>
    public ushort? TotalCalories { get; init; }

    /// <summary>Number of sessions in the activity.</summary>
    public int SessionCount { get; init; }

    /// <summary>Number of laps in the activity.</summary>
    public int LapCount { get; init; }

    /// <summary>Number of records in the activity.</summary>
    public int RecordCount { get; init; }

    /// <summary>Pool length in meters for swim activities.</summary>
    public double? PoolLengthMeters { get; init; }
}
