using Dynastream.Fit;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Fit.Internal;
using static FitToMarkdown.Fit.Mapping.FitEnumMapper;
using static FitToMarkdown.Fit.Mapping.FitFieldValueConverter;

namespace FitToMarkdown.Fit.Mapping;

/// <summary>
/// Maps EventMesg to Core event models with event type and action classification.
/// </summary>
internal static class EventMapper
{
    /// <summary>
    /// Maps all EventMesg items to Core FitEvent models.
    /// Complex dynamic event data (gear extraction, alert thresholds) is deferred to Phase 3.
    /// </summary>
    public static IReadOnlyList<FitEvent> MapEvents(
        List<(EventMesg Message, int Sequence)> items,
        FitDeveloperFieldResolver? devResolver)
    {
        if (items.Count == 0)
            return [];

        var results = new List<FitEvent>(items.Count);

        for (int i = 0; i < items.Count; i++)
        {
            var (mesg, seq) = items[i];

            results.Add(new FitEvent
            {
                Message = FitMessageIdentityFactory.Create(seq),
                EventIndex = i,
                TimestampUtc = FitTimestampNormalizer.ToUtcDateTimeOffset(mesg.GetTimestamp()),
                Event = MapEvent(mesg.GetEvent()),
                EventType = MapEventType(mesg.GetEventType()),
                Data = ToNullableUint(mesg.GetData()),
                Data16 = ToNullableUshort(mesg.GetData16()),
                TimerTrigger = EnumToString(SafeGetEnum(mesg.GetTimerTrigger)),
                CoursePointIndex = ToNullableUshort(mesg.GetCoursePointIndex()),
                RiderPosition = EnumToString(SafeGetEnum(mesg.GetRiderPosition)),

                // Dynamic event data fields — deferred to Phase 3
                BatteryLevelPercent = null,
                VirtualPartnerSpeedMetersPerSecond = null,
                HeartRateHighAlertBpm = null,
                HeartRateLowAlertBpm = null,
                SpeedHighAlertMetersPerSecond = null,
                SpeedLowAlertMetersPerSecond = null,
                CadenceHighAlertRpm = null,
                CadenceLowAlertRpm = null,
                PowerHighAlertWatts = null,
                PowerLowAlertWatts = null,
                TimeDurationAlert = null,
                DistanceDurationAlertMeters = null,
                CalorieDurationAlert = null,
                FitnessEquipmentState = null,
                SportPoint = null,
                FrontGearNumber = null,
                FrontGearTeeth = null,
                RearGearNumber = null,
                RearGearTeeth = null,
                CommunicationTimeout = null,
                RadarThreatAlertType = null,
                DeveloperFields = devResolver?.ResolveDeveloperFields(mesg) ?? [],
            });
        }

        return results;
    }
}
