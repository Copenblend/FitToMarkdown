using Dynastream.Fit;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.ValueObjects;
using FitToMarkdown.Fit.Internal;
using static FitToMarkdown.Fit.Mapping.FitEnumMapper;
using static FitToMarkdown.Fit.Mapping.FitFieldValueConverter;

namespace FitToMarkdown.Fit.Mapping;

/// <summary>
/// Maps SessionMesg, LapMesg, LengthMesg, and SegmentLapMesg to Core models.
/// </summary>
internal static class SessionAndLapMapper
{
    /// <summary>
    /// Maps all SessionMesg items to Core FitSession models.
    /// </summary>
    public static IReadOnlyList<FitSession> MapSessions(
        List<(SessionMesg Message, int Sequence)> items,
        FitDeveloperFieldResolver? devResolver)
    {
        if (items.Count == 0)
            return [];

        var results = new List<FitSession>(items.Count);

        for (int i = 0; i < items.Count; i++)
        {
            var (mesg, seq) = items[i];

            results.Add(new FitSession
            {
                Message = FitMessageIdentityFactory.Create(seq, mesg.GetMessageIndex()),
                SessionIndex = i,
                Range = new FitTimeRange
                {
                    StartTimeUtc = FitTimestampNormalizer.ToUtcDateTimeOffset(mesg.GetStartTime()),
                    EndTimeUtc = FitTimestampNormalizer.ToUtcDateTimeOffset(mesg.GetTimestamp()),
                    TotalElapsedTime = ToTimeSpan(mesg.GetTotalElapsedTime()),
                    TotalTimerTime = ToTimeSpan(mesg.GetTotalTimerTime()),
                },
                Sport = MapSport(mesg.GetSport()),
                SubSport = MapSubSport(mesg.GetSubSport()),
                Event = MapEvent(mesg.GetEvent()),
                EventType = MapEventType(mesg.GetEventType()),
                Trigger = MapSessionTrigger(mesg.GetTrigger()),
                SportProfileName = null,
                IsTransition = mesg.GetSport() == Sport.Transition,
                StartPosition = FitCoordinateNormalizer.ToGeoCoordinate(
                    mesg.GetStartPositionLat(), mesg.GetStartPositionLong()),
                Bounds = FitCoordinateNormalizer.ToGeoBounds(
                    mesg.GetNecLat(), mesg.GetNecLong(),
                    mesg.GetSwcLat(), mesg.GetSwcLong()),
                Metrics = FitSummaryMetricsMapper.MapFromSession(mesg),
                DeveloperFields = devResolver?.ResolveDeveloperFields(mesg) ?? [],
            });
        }

        return results;
    }

    /// <summary>
    /// Maps all LapMesg items to Core FitLap models.
    /// </summary>
    public static IReadOnlyList<FitLap> MapLaps(
        List<(LapMesg Message, int Sequence)> items,
        FitDeveloperFieldResolver? devResolver)
    {
        if (items.Count == 0)
            return [];

        var results = new List<FitLap>(items.Count);

        for (int i = 0; i < items.Count; i++)
        {
            var (mesg, seq) = items[i];

            results.Add(new FitLap
            {
                Message = FitMessageIdentityFactory.Create(seq, mesg.GetMessageIndex()),
                LapIndex = i,
                ParentSessionIndex = 0,
                Range = new FitTimeRange
                {
                    StartTimeUtc = FitTimestampNormalizer.ToUtcDateTimeOffset(mesg.GetStartTime()),
                    EndTimeUtc = FitTimestampNormalizer.ToUtcDateTimeOffset(mesg.GetTimestamp()),
                    TotalElapsedTime = ToTimeSpan(mesg.GetTotalElapsedTime()),
                    TotalTimerTime = ToTimeSpan(mesg.GetTotalTimerTime()),
                },
                Event = MapEvent(mesg.GetEvent()),
                EventType = MapEventType(mesg.GetEventType()),
                LapTrigger = MapLapTrigger(mesg.GetLapTrigger()),
                WorkoutStepIndex = ToNullableUshort(mesg.GetWktStepIndex()),
                StartPosition = FitCoordinateNormalizer.ToGeoCoordinate(
                    mesg.GetStartPositionLat(), mesg.GetStartPositionLong()),
                EndPosition = FitCoordinateNormalizer.ToGeoCoordinate(
                    mesg.GetEndPositionLat(), mesg.GetEndPositionLong()),
                Bounds = null,
                Metrics = FitSummaryMetricsMapper.MapFromLap(mesg),
                DeveloperFields = devResolver?.ResolveDeveloperFields(mesg) ?? [],
            });
        }

        return results;
    }

    /// <summary>
    /// Maps all LengthMesg items to Core FitLength models.
    /// </summary>
    public static IReadOnlyList<FitLength> MapLengths(
        List<(LengthMesg Message, int Sequence)> items,
        FitDeveloperFieldResolver? devResolver)
    {
        if (items.Count == 0)
            return [];

        var results = new List<FitLength>(items.Count);

        for (int i = 0; i < items.Count; i++)
        {
            var (mesg, seq) = items[i];

            results.Add(new FitLength
            {
                Message = FitMessageIdentityFactory.Create(seq, mesg.GetMessageIndex()),
                LengthIndex = i,
                Range = new FitTimeRange
                {
                    StartTimeUtc = FitTimestampNormalizer.ToUtcDateTimeOffset(mesg.GetStartTime()),
                    EndTimeUtc = FitTimestampNormalizer.ToUtcDateTimeOffset(mesg.GetTimestamp()),
                    TotalElapsedTime = ToTimeSpan(mesg.GetTotalElapsedTime()),
                    TotalTimerTime = ToTimeSpan(mesg.GetTotalTimerTime()),
                },
                LengthType = MapLengthType(mesg.GetLengthType()),
                SwimStroke = MapSwimStroke(mesg.GetSwimStroke()),
                TotalStrokes = ToNullableUshort(mesg.GetTotalStrokes()),
                AverageSpeedMetersPerSecond = ToNullableDouble(mesg.GetAvgSpeed()),
                AverageSwimmingCadenceSpm = (double?)ToNullableByte(mesg.GetAvgSwimmingCadence()),
                TotalCalories = ToNullableUshort(mesg.GetTotalCalories()),
                Event = MapEvent(mesg.GetEvent()),
                EventType = MapEventType(mesg.GetEventType()),
                DeveloperFields = devResolver?.ResolveDeveloperFields(mesg) ?? [],
            });
        }

        return results;
    }

    /// <summary>
    /// Maps all SegmentLapMesg items to Core FitSegmentLap models.
    /// </summary>
    public static IReadOnlyList<FitSegmentLap> MapSegmentLaps(
        List<(SegmentLapMesg Message, int Sequence)> items,
        FitDeveloperFieldResolver? devResolver)
    {
        if (items.Count == 0)
            return [];

        var results = new List<FitSegmentLap>(items.Count);

        for (int i = 0; i < items.Count; i++)
        {
            var (mesg, seq) = items[i];

            results.Add(new FitSegmentLap
            {
                Message = FitMessageIdentityFactory.Create(seq, mesg.GetMessageIndex()),
                SegmentIndex = i,
                Range = new FitTimeRange
                {
                    StartTimeUtc = FitTimestampNormalizer.ToUtcDateTimeOffset(mesg.GetStartTime()),
                    EndTimeUtc = FitTimestampNormalizer.ToUtcDateTimeOffset(mesg.GetTimestamp()),
                    TotalElapsedTime = ToTimeSpan(mesg.GetTotalElapsedTime()),
                    TotalTimerTime = ToTimeSpan(mesg.GetTotalTimerTime()),
                },
                SegmentName = ByteArrayToString(mesg.GetName()),
                Sport = MapSport(mesg.GetSport()),
                SubSport = MapSubSport(mesg.GetSubSport()),
                Metrics = MapSegmentMetrics(mesg),
                Status = EnumToString(mesg.GetStatus()),
                Uuid = ByteArrayToString(mesg.GetUuid()),
                SegmentTime = ToTimeSpan(mesg.GetTotalTimerTime()),
                DeveloperFields = devResolver?.ResolveDeveloperFields(mesg) ?? [],
            });
        }

        return results;
    }

    private static FitSummaryMetrics MapSegmentMetrics(SegmentLapMesg mesg)
    {
        return new FitSummaryMetrics
        {
            TotalElapsedTime = ToTimeSpan(mesg.GetTotalElapsedTime()),
            TotalTimerTime = ToTimeSpan(mesg.GetTotalTimerTime()),
            TotalDistanceMeters = ToNullableDouble(mesg.GetTotalDistance()),
            TotalCalories = ToNullableUshort(mesg.GetTotalCalories()),
            AverageSpeedMetersPerSecond = ToNullableDouble(mesg.GetAvgSpeed()),
            MaximumSpeedMetersPerSecond = ToNullableDouble(mesg.GetMaxSpeed()),
            AverageHeartRateBpm = ToNullableByte(mesg.GetAvgHeartRate()),
            MaximumHeartRateBpm = ToNullableByte(mesg.GetMaxHeartRate()),
            AverageCadenceRpm = (double?)ToNullableByte(mesg.GetAvgCadence()),
            MaximumCadenceRpm = (double?)ToNullableByte(mesg.GetMaxCadence()),
            AveragePowerWatts = ToNullableUshort(mesg.GetAvgPower()),
            MaximumPowerWatts = ToNullableUshort(mesg.GetMaxPower()),
            NormalizedPowerWatts = ToNullableUshort(mesg.GetNormalizedPower()),
            TotalAscentMeters = ToNullableUshort(mesg.GetTotalAscent()),
            TotalDescentMeters = ToNullableUshort(mesg.GetTotalDescent()),
            AverageAltitudeMeters = ToNullableDouble(mesg.GetAvgAltitude()),
            MaximumAltitudeMeters = ToNullableDouble(mesg.GetMaxAltitude()),
            MinimumAltitudeMeters = ToNullableDouble(mesg.GetMinAltitude()),
            AverageTemperatureCelsius = (double?)mesg.GetAvgTemperature(),
            MaximumTemperatureCelsius = (double?)mesg.GetMaxTemperature(),
            LeftRightBalance = null,
            StrokeCounts = [],
            ZoneCounts = [],
        };
    }
}
