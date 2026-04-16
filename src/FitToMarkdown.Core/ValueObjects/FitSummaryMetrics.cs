using FitToMarkdown.Core.Enums;

namespace FitToMarkdown.Core.ValueObjects;

/// <summary>
/// Represents the normalized summary metric surface shared by sessions, laps, and segment laps.
/// </summary>
public sealed record FitSummaryMetrics
{
    // ── Time ───────────────────────────────────────────────────────────

    /// <summary>Total elapsed time including pauses.</summary>
    public TimeSpan? TotalElapsedTime { get; init; }

    /// <summary>Total timer time excluding pauses.</summary>
    public TimeSpan? TotalTimerTime { get; init; }

    /// <summary>Total time spent moving.</summary>
    public TimeSpan? TotalMovingTime { get; init; }

    /// <summary>Average lap time.</summary>
    public TimeSpan? AverageLapTime { get; init; }

    // ── Distance / Calories ────────────────────────────────────────────

    /// <summary>Total distance in meters.</summary>
    public double? TotalDistanceMeters { get; init; }

    /// <summary>Total calories burned.</summary>
    public ushort? TotalCalories { get; init; }

    /// <summary>Total fat calories burned.</summary>
    public ushort? TotalFatCalories { get; init; }

    // ── Speed ──────────────────────────────────────────────────────────

    /// <summary>Average speed in meters per second.</summary>
    public double? AverageSpeedMetersPerSecond { get; init; }

    /// <summary>Maximum speed in meters per second.</summary>
    public double? MaximumSpeedMetersPerSecond { get; init; }

    /// <summary>Enhanced average speed in meters per second.</summary>
    public double? EnhancedAverageSpeedMetersPerSecond { get; init; }

    /// <summary>Enhanced maximum speed in meters per second.</summary>
    public double? EnhancedMaximumSpeedMetersPerSecond { get; init; }

    // ── Heart Rate ─────────────────────────────────────────────────────

    /// <summary>Average heart rate in beats per minute.</summary>
    public byte? AverageHeartRateBpm { get; init; }

    /// <summary>Maximum heart rate in beats per minute.</summary>
    public byte? MaximumHeartRateBpm { get; init; }

    /// <summary>Minimum heart rate in beats per minute.</summary>
    public byte? MinimumHeartRateBpm { get; init; }

    // ── Cadence ────────────────────────────────────────────────────────

    /// <summary>Average cadence in revolutions per minute.</summary>
    public double? AverageCadenceRpm { get; init; }

    /// <summary>Maximum cadence in revolutions per minute.</summary>
    public double? MaximumCadenceRpm { get; init; }

    /// <summary>Average fractional cadence in revolutions per minute.</summary>
    public double? AverageFractionalCadenceRpm { get; init; }

    /// <summary>Maximum fractional cadence in revolutions per minute.</summary>
    public double? MaximumFractionalCadenceRpm { get; init; }

    /// <summary>Total fractional cycles.</summary>
    public double? TotalFractionalCycles { get; init; }

    // ── Power ──────────────────────────────────────────────────────────

    /// <summary>Average power in watts.</summary>
    public ushort? AveragePowerWatts { get; init; }

    /// <summary>Maximum power in watts.</summary>
    public ushort? MaximumPowerWatts { get; init; }

    /// <summary>Normalized power in watts.</summary>
    public ushort? NormalizedPowerWatts { get; init; }

    /// <summary>Threshold power in watts.</summary>
    public ushort? ThresholdPowerWatts { get; init; }

    /// <summary>Total work in kilojoules.</summary>
    public uint? TotalWorkKilojoules { get; init; }

    // ── Altitude / Elevation ───────────────────────────────────────────

    /// <summary>Total ascent in meters.</summary>
    public ushort? TotalAscentMeters { get; init; }

    /// <summary>Total descent in meters.</summary>
    public ushort? TotalDescentMeters { get; init; }

    /// <summary>Average altitude in meters.</summary>
    public double? AverageAltitudeMeters { get; init; }

    /// <summary>Maximum altitude in meters.</summary>
    public double? MaximumAltitudeMeters { get; init; }

    /// <summary>Minimum altitude in meters.</summary>
    public double? MinimumAltitudeMeters { get; init; }

    /// <summary>Enhanced average altitude in meters.</summary>
    public double? EnhancedAverageAltitudeMeters { get; init; }

    /// <summary>Enhanced maximum altitude in meters.</summary>
    public double? EnhancedMaximumAltitudeMeters { get; init; }

    /// <summary>Enhanced minimum altitude in meters.</summary>
    public double? EnhancedMinimumAltitudeMeters { get; init; }

    // ── Temperature ────────────────────────────────────────────────────

    /// <summary>Average temperature in degrees Celsius.</summary>
    public double? AverageTemperatureCelsius { get; init; }

    /// <summary>Maximum temperature in degrees Celsius.</summary>
    public double? MaximumTemperatureCelsius { get; init; }

    // ── Training Load ──────────────────────────────────────────────────

    /// <summary>Training Stress Score (TSS).</summary>
    public float? TrainingStressScore { get; init; }

    /// <summary>Intensity Factor (IF).</summary>
    public float? IntensityFactor { get; init; }

    // ── Left/Right Balance ─────────────────────────────────────────────

    /// <summary>Normalized left/right balance percentages.</summary>
    public FitLeftRightBalance? LeftRightBalance { get; init; }

    // ── Torque Effectiveness / Pedal Smoothness ────────────────────────

    /// <summary>Average left torque effectiveness as a percentage.</summary>
    public double? AverageLeftTorqueEffectivenessPercent { get; init; }

    /// <summary>Average right torque effectiveness as a percentage.</summary>
    public double? AverageRightTorqueEffectivenessPercent { get; init; }

    /// <summary>Average left pedal smoothness as a percentage.</summary>
    public double? AverageLeftPedalSmoothnessPercent { get; init; }

    /// <summary>Average right pedal smoothness as a percentage.</summary>
    public double? AverageRightPedalSmoothnessPercent { get; init; }

    /// <summary>Average combined pedal smoothness as a percentage.</summary>
    public double? AverageCombinedPedalSmoothnessPercent { get; init; }

    // ── Platform Center Offset ─────────────────────────────────────────

    /// <summary>Average left platform center offset in millimeters.</summary>
    public double? AverageLeftPlatformCenterOffsetMillimeters { get; init; }

    /// <summary>Average right platform center offset in millimeters.</summary>
    public double? AverageRightPlatformCenterOffsetMillimeters { get; init; }

    // ── Swimming ───────────────────────────────────────────────────────

    /// <summary>Average stroke count per length.</summary>
    public double? AverageStrokeCount { get; init; }

    /// <summary>Total number of strokes.</summary>
    public ushort? TotalStrokes { get; init; }

    /// <summary>Average stroke distance in meters.</summary>
    public double? AverageStrokeDistanceMeters { get; init; }

    /// <summary>The primary swim stroke type.</summary>
    public FitSwimStroke? SwimStroke { get; init; }

    /// <summary>Pool length in meters.</summary>
    public double? PoolLengthMeters { get; init; }

    /// <summary>The unit used for pool length.</summary>
    public FitLengthUnit? PoolLengthUnit { get; init; }

    /// <summary>Number of active (non-rest) lengths.</summary>
    public ushort? NumberOfActiveLengths { get; init; }

    /// <summary>Total number of lengths.</summary>
    public ushort? NumberOfLengths { get; init; }

    // ── Lap Structure ──────────────────────────────────────────────────

    /// <summary>Index of the first lap in this session.</summary>
    public ushort? FirstLapIndex { get; init; }

    /// <summary>Number of laps in this session.</summary>
    public ushort? NumberOfLaps { get; init; }

    // ── Running Dynamics ───────────────────────────────────────────────

    /// <summary>Average stance time in milliseconds.</summary>
    public double? AverageStanceTimeMilliseconds { get; init; }

    /// <summary>Average stance time as a percentage of the gait cycle.</summary>
    public double? AverageStanceTimePercent { get; init; }

    /// <summary>Average stance time balance as a percentage.</summary>
    public double? AverageStanceTimeBalancePercent { get; init; }

    /// <summary>Average step length in meters.</summary>
    public double? AverageStepLengthMeters { get; init; }

    /// <summary>Average vertical oscillation in millimeters.</summary>
    public double? AverageVerticalOscillationMillimeters { get; init; }

    /// <summary>Average vertical ratio as a percentage.</summary>
    public double? AverageVerticalRatioPercent { get; init; }

    /// <summary>Average ground contact time in milliseconds.</summary>
    public double? AverageGroundContactTimeMilliseconds { get; init; }

    /// <summary>Average ground contact time balance as a percentage.</summary>
    public double? AverageGroundContactTimeBalancePercent { get; init; }

    // ── Scores ─────────────────────────────────────────────────────────

    /// <summary>Player score.</summary>
    public ushort? PlayerScore { get; init; }

    /// <summary>Opponent score.</summary>
    public ushort? OpponentScore { get; init; }

    // ── Stroke / Zone Counts ───────────────────────────────────────────

    /// <summary>Stroke counts broken down by stroke type.</summary>
    public IReadOnlyList<FitStrokeCountByType> StrokeCounts { get; init; } = [];

    /// <summary>Zone counts broken down by zone kind.</summary>
    public IReadOnlyList<FitZoneCount> ZoneCounts { get; init; } = [];

    // ── Standing / Seated ──────────────────────────────────────────────

    /// <summary>Total time spent standing.</summary>
    public TimeSpan? TimeStanding { get; init; }

    /// <summary>Total time spent seated.</summary>
    public TimeSpan? TimeSeated { get; init; }

    /// <summary>Number of times the rider stood.</summary>
    public ushort? StandCount { get; init; }

    // ── Respiration ────────────────────────────────────────────────────

    /// <summary>Average respiration rate in breaths per minute.</summary>
    public double? AverageRespirationRateBreathsPerMinute { get; init; }

    /// <summary>Maximum respiration rate in breaths per minute.</summary>
    public double? MaximumRespirationRateBreathsPerMinute { get; init; }

    /// <summary>Minimum respiration rate in breaths per minute.</summary>
    public double? MinimumRespirationRateBreathsPerMinute { get; init; }

    /// <summary>Peak training load.</summary>
    public uint? TrainingLoadPeak { get; init; }

    /// <summary>Enhanced average respiration rate in breaths per minute.</summary>
    public double? EnhancedAverageRespirationRateBreathsPerMinute { get; init; }

    /// <summary>Enhanced maximum respiration rate in breaths per minute.</summary>
    public double? EnhancedMaximumRespirationRateBreathsPerMinute { get; init; }

    /// <summary>Enhanced minimum respiration rate in breaths per minute.</summary>
    public double? EnhancedMinimumRespirationRateBreathsPerMinute { get; init; }

    // ── Grit / Flow ────────────────────────────────────────────────────

    /// <summary>Total grit score for the activity.</summary>
    public float? TotalGrit { get; init; }

    /// <summary>Average grit score.</summary>
    public float? AverageGrit { get; init; }

    /// <summary>Total flow score for the activity.</summary>
    public float? TotalFlow { get; init; }

    /// <summary>Average flow score.</summary>
    public float? AverageFlow { get; init; }

    // ── Climbing ───────────────────────────────────────────────────────

    /// <summary>Average vertical ascent meters per hour (VAM).</summary>
    public double? AverageVamMetersPerHour { get; init; }
}
