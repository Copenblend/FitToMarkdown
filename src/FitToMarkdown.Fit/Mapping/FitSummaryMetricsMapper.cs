using Dynastream.Fit;
using FitToMarkdown.Core.ValueObjects;
using static FitToMarkdown.Fit.Mapping.FitEnumMapper;
using static FitToMarkdown.Fit.Mapping.FitFieldValueConverter;

namespace FitToMarkdown.Fit.Mapping;

/// <summary>
/// Maps summary metrics from SessionMesg and LapMesg to the shared FitSummaryMetrics value object.
/// </summary>
internal static class FitSummaryMetricsMapper
{
    /// <summary>
    /// Maps all summary metric properties from a SessionMesg.
    /// </summary>
    public static FitSummaryMetrics MapFromSession(SessionMesg session)
    {
        return new FitSummaryMetrics
        {
            // ── Time ──────────────────────────────────────────────────
            TotalElapsedTime = ToTimeSpan(session.GetTotalElapsedTime()),
            TotalTimerTime = ToTimeSpan(session.GetTotalTimerTime()),
            TotalMovingTime = null,
            AverageLapTime = null,

            // ── Distance / Calories ───────────────────────────────────
            TotalDistanceMeters = ToNullableDouble(session.GetTotalDistance()),
            TotalCalories = ToNullableUshort(session.GetTotalCalories()),
            TotalFatCalories = ToNullableUshort(session.GetTotalFatCalories()),

            // ── Speed ─────────────────────────────────────────────────
            AverageSpeedMetersPerSecond = ToNullableDouble(session.GetAvgSpeed()),
            MaximumSpeedMetersPerSecond = ToNullableDouble(session.GetMaxSpeed()),
            EnhancedAverageSpeedMetersPerSecond = ToNullableDouble(session.GetEnhancedAvgSpeed()),
            EnhancedMaximumSpeedMetersPerSecond = ToNullableDouble(session.GetEnhancedMaxSpeed()),

            // ── Heart Rate ────────────────────────────────────────────
            AverageHeartRateBpm = ToNullableByte(session.GetAvgHeartRate()),
            MaximumHeartRateBpm = ToNullableByte(session.GetMaxHeartRate()),
            MinimumHeartRateBpm = ToNullableByte(session.GetMinHeartRate()),

            // ── Cadence ───────────────────────────────────────────────
            AverageCadenceRpm = (double?)ToNullableByte(session.GetAvgCadence()),
            MaximumCadenceRpm = (double?)ToNullableByte(session.GetMaxCadence()),
            AverageFractionalCadenceRpm = ToNullableDouble(session.GetAvgFractionalCadence()),
            MaximumFractionalCadenceRpm = ToNullableDouble(session.GetMaxFractionalCadence()),
            TotalFractionalCycles = ToNullableDouble(session.GetTotalFractionalCycles()),

            // ── Power ─────────────────────────────────────────────────
            AveragePowerWatts = ToNullableUshort(session.GetAvgPower()),
            MaximumPowerWatts = ToNullableUshort(session.GetMaxPower()),
            NormalizedPowerWatts = ToNullableUshort(session.GetNormalizedPower()),
            ThresholdPowerWatts = ToNullableUshort(session.GetThresholdPower()),
            TotalWorkKilojoules = ConvertJoulesToKilojoules(ToNullableUint(session.GetTotalWork())),

            // ── Altitude / Elevation ──────────────────────────────────
            TotalAscentMeters = ToNullableUshort(session.GetTotalAscent()),
            TotalDescentMeters = ToNullableUshort(session.GetTotalDescent()),
            AverageAltitudeMeters = ToNullableDouble(session.GetAvgAltitude()),
            MaximumAltitudeMeters = ToNullableDouble(session.GetMaxAltitude()),
            MinimumAltitudeMeters = ToNullableDouble(session.GetMinAltitude()),
            EnhancedAverageAltitudeMeters = ToNullableDouble(session.GetEnhancedAvgAltitude()),
            EnhancedMaximumAltitudeMeters = ToNullableDouble(session.GetEnhancedMaxAltitude()),
            EnhancedMinimumAltitudeMeters = ToNullableDouble(session.GetEnhancedMinAltitude()),

            // ── Temperature ───────────────────────────────────────────
            AverageTemperatureCelsius = (double?)session.GetAvgTemperature(),
            MaximumTemperatureCelsius = (double?)session.GetMaxTemperature(),

            // ── Training Load ─────────────────────────────────────────
            TrainingStressScore = session.GetTrainingStressScore(),
            IntensityFactor = session.GetIntensityFactor(),

            // ── Left/Right Balance ────────────────────────────────────
            LeftRightBalance = DecodeLeftRightBalance100(session.GetLeftRightBalance()),

            // ── Torque / Pedal Smoothness ─────────────────────────────
            AverageLeftTorqueEffectivenessPercent = ToNullableDouble(session.GetAvgLeftTorqueEffectiveness()),
            AverageRightTorqueEffectivenessPercent = ToNullableDouble(session.GetAvgRightTorqueEffectiveness()),
            AverageLeftPedalSmoothnessPercent = ToNullableDouble(session.GetAvgLeftPedalSmoothness()),
            AverageRightPedalSmoothnessPercent = ToNullableDouble(session.GetAvgRightPedalSmoothness()),
            AverageCombinedPedalSmoothnessPercent = ToNullableDouble(session.GetAvgCombinedPedalSmoothness()),

            // ── Platform Center Offset ────────────────────────────────
            AverageLeftPlatformCenterOffsetMillimeters = null,
            AverageRightPlatformCenterOffsetMillimeters = null,

            // ── Swimming ──────────────────────────────────────────────
            AverageStrokeCount = ToNullableDouble(session.GetAvgStrokeCount()),
            TotalStrokes = null,
            AverageStrokeDistanceMeters = ToNullableDouble(session.GetAvgStrokeDistance()),
            SwimStroke = MapSwimStroke(session.GetSwimStroke()),
            PoolLengthMeters = ToNullableDouble(session.GetPoolLength()),
            PoolLengthUnit = MapDisplayMeasure(session.GetPoolLengthUnit()),
            NumberOfActiveLengths = ToNullableUshort(session.GetNumActiveLengths()),
            NumberOfLengths = null,

            // ── Lap Structure ─────────────────────────────────────────
            FirstLapIndex = ToNullableUshort(session.GetFirstLapIndex()),
            NumberOfLaps = ToNullableUshort(session.GetNumLaps()),

            // ── Running Dynamics ──────────────────────────────────────
            AverageStanceTimeMilliseconds = ToNullableDouble(session.GetAvgStanceTime()),
            AverageStanceTimePercent = ToNullableDouble(session.GetAvgStanceTimePercent()),
            AverageStanceTimeBalancePercent = null,
            AverageStepLengthMeters = ConvertMmToMeters(session.GetAvgStepLength()),
            AverageVerticalOscillationMillimeters = ToNullableDouble(session.GetAvgVerticalOscillation()),
            AverageVerticalRatioPercent = ToNullableDouble(session.GetAvgVerticalRatio()),
            AverageGroundContactTimeMilliseconds = null,
            AverageGroundContactTimeBalancePercent = null,

            // ── Scores ────────────────────────────────────────────────
            PlayerScore = null,
            OpponentScore = null,

            // ── Stroke / Zone Counts ──────────────────────────────────
            StrokeCounts = [],
            ZoneCounts = [],

            // ── Standing / Seated ─────────────────────────────────────
            TimeStanding = ToTimeSpan(session.GetTimeStanding()),
            TimeSeated = null,
            StandCount = ToNullableUshort(session.GetStandCount()),

            // ── Respiration ───────────────────────────────────────────
            AverageRespirationRateBreathsPerMinute = null,
            MaximumRespirationRateBreathsPerMinute = null,
            MinimumRespirationRateBreathsPerMinute = null,
            TrainingLoadPeak = null,
            EnhancedAverageRespirationRateBreathsPerMinute = null,
            EnhancedMaximumRespirationRateBreathsPerMinute = null,
            EnhancedMinimumRespirationRateBreathsPerMinute = null,

            // ── Grit / Flow ───────────────────────────────────────────
            TotalGrit = null,
            AverageGrit = null,
            TotalFlow = null,
            AverageFlow = null,

            // ── Climbing ──────────────────────────────────────────────
            AverageVamMetersPerHour = null,
        };
    }

    /// <summary>
    /// Maps all summary metric properties from a LapMesg.
    /// Session-only properties are set to null.
    /// </summary>
    public static FitSummaryMetrics MapFromLap(LapMesg lap)
    {
        return new FitSummaryMetrics
        {
            // ── Time ──────────────────────────────────────────────────
            TotalElapsedTime = ToTimeSpan(lap.GetTotalElapsedTime()),
            TotalTimerTime = ToTimeSpan(lap.GetTotalTimerTime()),
            TotalMovingTime = null,
            AverageLapTime = null,

            // ── Distance / Calories ───────────────────────────────────
            TotalDistanceMeters = ToNullableDouble(lap.GetTotalDistance()),
            TotalCalories = ToNullableUshort(lap.GetTotalCalories()),
            TotalFatCalories = ToNullableUshort(lap.GetTotalFatCalories()),

            // ── Speed ─────────────────────────────────────────────────
            AverageSpeedMetersPerSecond = ToNullableDouble(lap.GetAvgSpeed()),
            MaximumSpeedMetersPerSecond = ToNullableDouble(lap.GetMaxSpeed()),
            EnhancedAverageSpeedMetersPerSecond = ToNullableDouble(lap.GetEnhancedAvgSpeed()),
            EnhancedMaximumSpeedMetersPerSecond = ToNullableDouble(lap.GetEnhancedMaxSpeed()),

            // ── Heart Rate ────────────────────────────────────────────
            AverageHeartRateBpm = ToNullableByte(lap.GetAvgHeartRate()),
            MaximumHeartRateBpm = ToNullableByte(lap.GetMaxHeartRate()),
            MinimumHeartRateBpm = null,

            // ── Cadence ───────────────────────────────────────────────
            AverageCadenceRpm = (double?)ToNullableByte(lap.GetAvgCadence()),
            MaximumCadenceRpm = (double?)ToNullableByte(lap.GetMaxCadence()),
            AverageFractionalCadenceRpm = ToNullableDouble(lap.GetAvgFractionalCadence()),
            MaximumFractionalCadenceRpm = ToNullableDouble(lap.GetMaxFractionalCadence()),
            TotalFractionalCycles = ToNullableDouble(lap.GetTotalFractionalCycles()),

            // ── Power ─────────────────────────────────────────────────
            AveragePowerWatts = ToNullableUshort(lap.GetAvgPower()),
            MaximumPowerWatts = ToNullableUshort(lap.GetMaxPower()),
            NormalizedPowerWatts = ToNullableUshort(lap.GetNormalizedPower()),
            ThresholdPowerWatts = null,
            TotalWorkKilojoules = ConvertJoulesToKilojoules(ToNullableUint(lap.GetTotalWork())),

            // ── Altitude / Elevation ──────────────────────────────────
            TotalAscentMeters = ToNullableUshort(lap.GetTotalAscent()),
            TotalDescentMeters = ToNullableUshort(lap.GetTotalDescent()),
            AverageAltitudeMeters = ToNullableDouble(lap.GetAvgAltitude()),
            MaximumAltitudeMeters = ToNullableDouble(lap.GetMaxAltitude()),
            MinimumAltitudeMeters = ToNullableDouble(lap.GetMinAltitude()),
            EnhancedAverageAltitudeMeters = ToNullableDouble(lap.GetEnhancedAvgAltitude()),
            EnhancedMaximumAltitudeMeters = ToNullableDouble(lap.GetEnhancedMaxAltitude()),
            EnhancedMinimumAltitudeMeters = ToNullableDouble(lap.GetEnhancedMinAltitude()),

            // ── Temperature ───────────────────────────────────────────
            AverageTemperatureCelsius = (double?)lap.GetAvgTemperature(),
            MaximumTemperatureCelsius = (double?)lap.GetMaxTemperature(),

            // ── Training Load ─────────────────────────────────────────
            TrainingStressScore = null,
            IntensityFactor = null,

            // ── Left/Right Balance ────────────────────────────────────
            LeftRightBalance = DecodeLeftRightBalance100(lap.GetLeftRightBalance()),

            // ── Torque / Pedal Smoothness ─────────────────────────────
            AverageLeftTorqueEffectivenessPercent = ToNullableDouble(lap.GetAvgLeftTorqueEffectiveness()),
            AverageRightTorqueEffectivenessPercent = ToNullableDouble(lap.GetAvgRightTorqueEffectiveness()),
            AverageLeftPedalSmoothnessPercent = ToNullableDouble(lap.GetAvgLeftPedalSmoothness()),
            AverageRightPedalSmoothnessPercent = ToNullableDouble(lap.GetAvgRightPedalSmoothness()),
            AverageCombinedPedalSmoothnessPercent = ToNullableDouble(lap.GetAvgCombinedPedalSmoothness()),

            // ── Platform Center Offset ────────────────────────────────
            AverageLeftPlatformCenterOffsetMillimeters = null,
            AverageRightPlatformCenterOffsetMillimeters = null,

            // ── Swimming ──────────────────────────────────────────────
            AverageStrokeCount = null,
            TotalStrokes = null,
            AverageStrokeDistanceMeters = null,
            SwimStroke = MapSwimStroke(lap.GetSwimStroke()),
            PoolLengthMeters = null,
            PoolLengthUnit = null,
            NumberOfActiveLengths = null,
            NumberOfLengths = null,

            // ── Lap Structure ─────────────────────────────────────────
            FirstLapIndex = null,
            NumberOfLaps = null,

            // ── Running Dynamics ──────────────────────────────────────
            AverageStanceTimeMilliseconds = ToNullableDouble(lap.GetAvgStanceTime()),
            AverageStanceTimePercent = ToNullableDouble(lap.GetAvgStanceTimePercent()),
            AverageStanceTimeBalancePercent = null,
            AverageStepLengthMeters = ConvertMmToMeters(lap.GetAvgStepLength()),
            AverageVerticalOscillationMillimeters = ToNullableDouble(lap.GetAvgVerticalOscillation()),
            AverageVerticalRatioPercent = ToNullableDouble(lap.GetAvgVerticalRatio()),
            AverageGroundContactTimeMilliseconds = null,
            AverageGroundContactTimeBalancePercent = null,

            // ── Scores ────────────────────────────────────────────────
            PlayerScore = null,
            OpponentScore = null,

            // ── Stroke / Zone Counts ──────────────────────────────────
            StrokeCounts = [],
            ZoneCounts = [],

            // ── Standing / Seated ─────────────────────────────────────
            TimeStanding = null,
            TimeSeated = null,
            StandCount = null,

            // ── Respiration ───────────────────────────────────────────
            AverageRespirationRateBreathsPerMinute = null,
            MaximumRespirationRateBreathsPerMinute = null,
            MinimumRespirationRateBreathsPerMinute = null,
            TrainingLoadPeak = null,
            EnhancedAverageRespirationRateBreathsPerMinute = null,
            EnhancedMaximumRespirationRateBreathsPerMinute = null,
            EnhancedMinimumRespirationRateBreathsPerMinute = null,

            // ── Grit / Flow ───────────────────────────────────────────
            TotalGrit = null,
            AverageGrit = null,
            TotalFlow = null,
            AverageFlow = null,

            // ── Climbing ──────────────────────────────────────────────
            AverageVamMetersPerHour = null,
        };
    }

    private static FitLeftRightBalance? DecodeLeftRightBalance100(ushort? raw)
    {
        if (raw is null || raw.Value == 0xFFFF)
            return null;

        ushort val = raw.Value;
        bool rightRef = (val & 0x8000) != 0;
        double percent = (val & 0x3FFF) / 100.0;

        return rightRef
            ? new FitLeftRightBalance
            {
                LeftPercent = 100.0 - percent,
                RightPercent = percent,
                UsesRightReference = true,
            }
            : new FitLeftRightBalance
            {
                LeftPercent = percent,
                RightPercent = 100.0 - percent,
                UsesRightReference = false,
            };
    }

    private static uint? ConvertJoulesToKilojoules(uint? joules) =>
        joules.HasValue ? joules.Value / 1000 : null;

    private static double? ConvertMmToMeters(float? mm)
    {
        var d = ToNullableDouble(mm);
        return d.HasValue ? d.Value / 1000.0 : null;
    }

    private static ushort? CastUintToNullableUshort(uint? value) =>
        value.HasValue ? (ushort?)value.Value : null;
}
