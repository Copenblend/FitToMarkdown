using Dynastream.Fit;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Fit.Internal;
using static FitToMarkdown.Fit.Mapping.FitEnumMapper;
using static FitToMarkdown.Fit.Mapping.FitFieldValueConverter;

namespace FitToMarkdown.Fit.Mapping;

/// <summary>
/// Maps RecordMesg, HrvMesg, and HrMesg to Core record and heart rate models.
/// </summary>
internal static class RecordMapper
{
    /// <summary>
    /// Maps all RecordMesg items to Core FitRecord models.
    /// </summary>
    public static IReadOnlyList<FitRecord> MapRecords(
        List<(RecordMesg Message, int Sequence)> items,
        FitDeveloperFieldResolver? devResolver)
    {
        if (items.Count == 0)
            return [];

        var results = new List<FitRecord>(items.Count);

        for (int i = 0; i < items.Count; i++)
        {
            var (mesg, seq) = items[i];

            results.Add(new FitRecord
            {
                Message = FitMessageIdentityFactory.Create(seq),
                RecordIndex = i,
                TimestampUtc = FitTimestampNormalizer.ToUtcDateTimeOffset(mesg.GetTimestamp()),
                Position = FitCoordinateNormalizer.ToGeoCoordinate(
                    mesg.GetPositionLat(), mesg.GetPositionLong()),
                DistanceMeters = ToNullableDouble(mesg.GetDistance()),
                AltitudeMeters = ToNullableDouble(mesg.GetAltitude()),
                EnhancedAltitudeMeters = ToNullableDouble(mesg.GetEnhancedAltitude()),
                SpeedMetersPerSecond = ToNullableDouble(mesg.GetSpeed()),
                EnhancedSpeedMetersPerSecond = ToNullableDouble(mesg.GetEnhancedSpeed()),
                HeartRateBpm = ToNullableByte(mesg.GetHeartRate()),
                CadenceRpm = (double?)ToNullableByte(mesg.GetCadence()),
                FractionalCadenceRpm = ToNullableDouble(mesg.GetFractionalCadence()),
                PowerWatts = ToNullableUshort(mesg.GetPower()),
                TemperatureCelsius = (double?)mesg.GetTemperature(),
                GpsAccuracyMeters = ToNullableByte(mesg.GetGpsAccuracy()),
                VerticalSpeedMetersPerSecond = ToNullableDouble(mesg.GetVerticalSpeed()),
                Calories = ToNullableUshort(mesg.GetCalories()),
                VerticalOscillationMillimeters = ToNullableDouble(mesg.GetVerticalOscillation()),
                StanceTimeMilliseconds = ToNullableDouble(mesg.GetStanceTime()),
                StanceTimePercent = ToNullableDouble(mesg.GetStanceTimePercent()),
                StanceTimeBalancePercent = ToNullableDouble(mesg.GetStanceTimeBalance()),
                StepLengthMeters = ConvertMmToMeters(mesg.GetStepLength()),
                ActivityType = MapActivityType(mesg.GetActivityType()),
                CycleLengthMeters = ToNullableDouble(mesg.GetCycleLength()),
                RespirationRateBreathsPerMinute = null,
                EnhancedRespirationRateBreathsPerMinute = null,
                GradePercent = ToNullableDouble(mesg.GetGrade()),
                ResistanceLevel = ToNullableByte(mesg.GetResistance()),
                TimeFromCourse = ToTimeSpan(mesg.GetTimeFromCourse()),
                AbsolutePressurePascals = null,
                DepthMeters = null,
                EnhancedDepthMeters = null,
                NextStopDepthMeters = null,
                NextStopTime = null,
                TimeToSurface = null,
                NdlTime = null,
                CnsLoadPercent = null,
                N2LoadPercent = null,
                DeveloperFields = devResolver?.ResolveDeveloperFields(mesg) ?? [],
            });
        }

        return results;
    }

    /// <summary>
    /// Maps all HrvMesg items to Core FitHrv models, extracting RR intervals.
    /// </summary>
    public static IReadOnlyList<FitHrv> MapHrvMessages(List<(HrvMesg Message, int Sequence)> items)
    {
        if (items.Count == 0)
            return [];

        var results = new List<FitHrv>(items.Count);

        foreach (var (mesg, seq) in items)
        {
            var intervals = new List<double>();
            int count = mesg.GetNumTime();

            for (int i = 0; i < count; i++)
            {
                var t = mesg.GetTime(i);
                if (t.HasValue && !float.IsNaN(t.Value) && !float.IsInfinity(t.Value))
                    intervals.Add(t.Value);
            }

            results.Add(new FitHrv
            {
                Message = FitMessageIdentityFactory.Create(seq),
                RrIntervalsSeconds = intervals,
            });
        }

        return results;
    }

    /// <summary>
    /// Maps all HrMesg items to Core FitHr models.
    /// </summary>
    public static IReadOnlyList<FitHr> MapHrMessages(List<(HrMesg Message, int Sequence)> items)
    {
        if (items.Count == 0)
            return [];

        var results = new List<FitHr>(items.Count);

        foreach (var (mesg, seq) in items)
        {
            var eventTimestamps = new List<uint>();
            int tsCount = mesg.GetNumEventTimestamp();

            for (int i = 0; i < tsCount; i++)
            {
                var ts = mesg.GetEventTimestamp(i);
                if (ts.HasValue)
                    eventTimestamps.Add((uint)(ts.Value * 1024.0f));
            }

            results.Add(new FitHr
            {
                Message = FitMessageIdentityFactory.Create(seq),
                TimestampUtc = FitTimestampNormalizer.ToUtcDateTimeOffset(mesg.GetTimestamp()),
                FilteredBpm = ToNullableByte(mesg.GetFilteredBpm(0)),
                BeatTimestampsUtc = [],
                EventTimestampRaw = eventTimestamps,
                EventTimestamp12Raw = [],
                MergedIntoRecords = false,
            });
        }

        return results;
    }

    private static double? ConvertMmToMeters(float? mm)
    {
        var d = ToNullableDouble(mm);
        return d.HasValue ? d.Value / 1000.0 : null;
    }
}
