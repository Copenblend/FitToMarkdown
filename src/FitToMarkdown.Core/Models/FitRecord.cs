using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized RecordMesg from a FIT file.
/// </summary>
public sealed record FitRecord
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>Zero-based record index within the activity.</summary>
    public int RecordIndex { get; init; }

    /// <summary>Session index this record was assigned to.</summary>
    public int? SessionIndex { get; init; }

    /// <summary>Timestamp of the record in UTC.</summary>
    public DateTimeOffset? TimestampUtc { get; init; }

    /// <summary>GPS position coordinate.</summary>
    public GeoCoordinate? Position { get; init; }

    /// <summary>Accumulated distance in meters.</summary>
    public double? DistanceMeters { get; init; }

    /// <summary>Altitude in meters.</summary>
    public double? AltitudeMeters { get; init; }

    /// <summary>Enhanced altitude in meters.</summary>
    public double? EnhancedAltitudeMeters { get; init; }

    /// <summary>Speed in meters per second.</summary>
    public double? SpeedMetersPerSecond { get; init; }

    /// <summary>Enhanced speed in meters per second.</summary>
    public double? EnhancedSpeedMetersPerSecond { get; init; }

    /// <summary>Heart rate in beats per minute.</summary>
    public byte? HeartRateBpm { get; init; }

    /// <summary>Cadence in revolutions per minute.</summary>
    public double? CadenceRpm { get; init; }

    /// <summary>Fractional cadence in revolutions per minute.</summary>
    public double? FractionalCadenceRpm { get; init; }

    /// <summary>Power output in watts.</summary>
    public ushort? PowerWatts { get; init; }

    /// <summary>Temperature in degrees Celsius.</summary>
    public double? TemperatureCelsius { get; init; }

    /// <summary>Left/right balance data.</summary>
    public FitLeftRightBalance? LeftRightBalance { get; init; }

    /// <summary>GPS accuracy in meters.</summary>
    public byte? GpsAccuracyMeters { get; init; }

    /// <summary>Vertical speed in meters per second.</summary>
    public double? VerticalSpeedMetersPerSecond { get; init; }

    /// <summary>Accumulated calories.</summary>
    public ushort? Calories { get; init; }

    /// <summary>Vertical oscillation in millimeters.</summary>
    public double? VerticalOscillationMillimeters { get; init; }

    /// <summary>Stance time in milliseconds.</summary>
    public double? StanceTimeMilliseconds { get; init; }

    /// <summary>Stance time as a percentage of step time.</summary>
    public double? StanceTimePercent { get; init; }

    /// <summary>Stance time balance as a percentage.</summary>
    public double? StanceTimeBalancePercent { get; init; }

    /// <summary>Step length in meters.</summary>
    public double? StepLengthMeters { get; init; }

    /// <summary>Ground contact time in milliseconds.</summary>
    public double? GroundContactTimeMilliseconds { get; init; }

    /// <summary>Ground contact time balance as a percentage.</summary>
    public double? GroundContactTimeBalancePercent { get; init; }

    /// <summary>Activity type for this record.</summary>
    public FitActivityType? ActivityType { get; init; }

    /// <summary>Vertical ratio as a percentage.</summary>
    public double? VerticalRatioPercent { get; init; }

    /// <summary>Heart rate variability RR intervals in seconds.</summary>
    public IReadOnlyList<double> HeartRateVariabilitySeconds { get; init; } = [];

    /// <summary>Cycle length in meters.</summary>
    public double? CycleLengthMeters { get; init; }

    /// <summary>Respiration rate in breaths per minute.</summary>
    public double? RespirationRateBreathsPerMinute { get; init; }

    /// <summary>Enhanced respiration rate in breaths per minute.</summary>
    public double? EnhancedRespirationRateBreathsPerMinute { get; init; }

    /// <summary>Raw compressed speed/distance value.</summary>
    public uint? CompressedSpeedDistanceRaw { get; init; }

    /// <summary>Grade as a percentage.</summary>
    public double? GradePercent { get; init; }

    /// <summary>Resistance level.</summary>
    public byte? ResistanceLevel { get; init; }

    /// <summary>Time offset from the course.</summary>
    public TimeSpan? TimeFromCourse { get; init; }

    /// <summary>Cycle length in 1/16 meter resolution.</summary>
    public double? CycleLength16Meters { get; init; }

    /// <summary>Absolute pressure in pascals.</summary>
    public double? AbsolutePressurePascals { get; init; }

    /// <summary>Depth in meters.</summary>
    public double? DepthMeters { get; init; }

    /// <summary>Enhanced depth in meters.</summary>
    public double? EnhancedDepthMeters { get; init; }

    /// <summary>Next decompression stop depth in meters.</summary>
    public double? NextStopDepthMeters { get; init; }

    /// <summary>Time until the next decompression stop.</summary>
    public TimeSpan? NextStopTime { get; init; }

    /// <summary>Time to surface.</summary>
    public TimeSpan? TimeToSurface { get; init; }

    /// <summary>No decompression limit time.</summary>
    public TimeSpan? NdlTime { get; init; }

    /// <summary>Central nervous system oxygen toxicity load percentage.</summary>
    public ushort? CnsLoadPercent { get; init; }

    /// <summary>Nitrogen tissue loading percentage.</summary>
    public ushort? N2LoadPercent { get; init; }

    /// <summary>Developer field values attached to this message.</summary>
    public IReadOnlyList<FitDeveloperFieldValue> DeveloperFields { get; init; } = [];
}
