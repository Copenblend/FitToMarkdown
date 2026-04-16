using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Summaries;

/// <summary>
/// Represents one downsampled record row used for the markdown CSV sample section.
/// </summary>
public sealed record SampledTimeSeriesRow
{
    /// <summary>Zero-based index of the source record in the full series.</summary>
    public int SourceRecordIndex { get; init; }

    /// <summary>Zero-based session index this record belongs to.</summary>
    public int SessionIndex { get; init; }

    /// <summary>Zero-based lap index this record belongs to.</summary>
    public int? LapIndex { get; init; }

    /// <summary>UTC timestamp of the record.</summary>
    public DateTimeOffset? TimestampUtc { get; init; }

    /// <summary>Time offset from the session start.</summary>
    public TimeSpan? OffsetFromSessionStart { get; init; }

    /// <summary>GPS position at this record.</summary>
    public GeoCoordinate? Position { get; init; }

    /// <summary>Cumulative distance in meters.</summary>
    public double? DistanceMeters { get; init; }

    /// <summary>Speed in meters per second.</summary>
    public double? SpeedMetersPerSecond { get; init; }

    /// <summary>Heart rate in beats per minute.</summary>
    public byte? HeartRateBpm { get; init; }

    /// <summary>Cadence in revolutions per minute.</summary>
    public double? CadenceRpm { get; init; }

    /// <summary>Power output in watts.</summary>
    public ushort? PowerWatts { get; init; }

    /// <summary>Altitude in meters.</summary>
    public double? AltitudeMeters { get; init; }

    /// <summary>Temperature in degrees Celsius.</summary>
    public double? TemperatureCelsius { get; init; }

    /// <summary>Respiration rate in breaths per minute.</summary>
    public double? RespirationRateBreathsPerMinute { get; init; }

    /// <summary>Depth in meters for dive activities.</summary>
    public double? DepthMeters { get; init; }

    /// <summary>Developer-defined custom fields attached to this record.</summary>
    public IReadOnlyList<FitDeveloperFieldValue> DeveloperFields { get; init; } = [];
}
