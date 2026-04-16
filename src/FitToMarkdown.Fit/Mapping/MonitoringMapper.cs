using Dynastream.Fit;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Fit.Internal;
using static FitToMarkdown.Fit.Mapping.FitEnumMapper;
using static FitToMarkdown.Fit.Mapping.FitFieldValueConverter;

namespace FitToMarkdown.Fit.Mapping;

/// <summary>
/// Maps Monitoring messages (raw Mesg) to Core monitoring models.
/// MonitoringMesg is not broadcast by MesgBroadcaster in SDK v1.0.1,
/// so messages arrive as unhandled Mesg objects.
/// </summary>
internal static class MonitoringMapper
{
    private const ushort MonitoringMesgNum = 55;

    /// <summary>
    /// Filters unhandled messages for Monitoring (mesg num 55) and maps to Core FitMonitoring models.
    /// </summary>
    public static IReadOnlyList<FitMonitoring> MapMonitoring(
        List<(Mesg Message, int Sequence)> unhandledMesgs,
        FitDeveloperFieldResolver? devResolver)
    {
        if (unhandledMesgs.Count == 0)
            return [];

        var results = new List<FitMonitoring>();

        foreach (var (rawMesg, seq) in unhandledMesgs)
        {
            if (rawMesg.Num != MonitoringMesgNum)
                continue;

            var mesg = new MonitoringMesg(rawMesg);

            results.Add(new FitMonitoring
            {
                Message = FitMessageIdentityFactory.Create(seq),
                TimestampUtc = FitTimestampNormalizer.ToUtcDateTimeOffset(mesg.GetTimestamp()),
                DeviceIndex = ToNullableUshort(mesg.GetDeviceIndex()),
                Calories = ToNullableUshort(mesg.GetCalories()),
                DistanceMeters = ToNullableDouble(mesg.GetDistance()),
                Cycles = ToNullableDouble(mesg.GetCycles()),
                ActiveTime = ToTimeSpan(mesg.GetActiveTime()),
                ActivityType = MapActivityType(mesg.GetActivityType()),
                ActivitySubtype = EnumToString(mesg.GetActivitySubtype()),
                ActivityLevel = null,
                Distance16Meters = null,
                Cycles16 = null,
                ActiveTime16 = null,
                LocalTimestamp = ToUtcFromRawTimestamp(mesg.GetLocalTimestamp()),
                TemperatureCelsius = ToNullableDouble(mesg.GetTemperature()),
                MinimumTemperatureCelsius = null,
                MaximumTemperatureCelsius = null,
                ActivityTime = null,
                ActiveCalories = ToNullableUshort(mesg.GetActiveCalories()),
                CurrentActivityTypeIntensity = null,
                TimestampMin8 = null,
                Timestamp16 = null,
                HeartRateBpm = null,
                Intensity = null,
                DurationMin = null,
                Duration = null,
                AscentMeters = mesg.GetAscent() is float asc ? (ushort?)asc : null,
                DescentMeters = mesg.GetDescent() is float desc ? (ushort?)desc : null,
                ModerateActivityMinutes = ToNullableUshort(mesg.GetModerateActivityMinutes()),
                VigorousActivityMinutes = ToNullableUshort(mesg.GetVigorousActivityMinutes()),
                DeveloperFields = devResolver?.ResolveDeveloperFields(rawMesg) ?? [],
            });
        }

        return results;
    }

    private static DateTimeOffset? ToUtcFromRawTimestamp(uint? rawTimestamp)
    {
        if (rawTimestamp is null)
            return null;
        return FitTimestampNormalizer.ToUtcDateTimeOffset(new Dynastream.Fit.DateTime(rawTimestamp.Value));
    }
}
