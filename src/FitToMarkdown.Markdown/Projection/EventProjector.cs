using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.Summaries;

namespace FitToMarkdown.Markdown.Projection;

internal static class EventProjector
{
    private static readonly HashSet<FitEventKind> SignificantEvents =
    [
        FitEventKind.Timer,
        FitEventKind.Session,
        FitEventKind.Lap,
        FitEventKind.Activity,
        FitEventKind.FrontGearChange,
        FitEventKind.RearGearChange,
        FitEventKind.RiderPositionChange,
    ];

    internal static IReadOnlyList<EventTimelineItem> Project(FitFileDocument document)
    {
        var activity = document.ActivityContent;
        if (activity is null)
            return [];

        var events = activity.Events;
        var items = new List<EventTimelineItem>(events.Count);

        for (int i = 0; i < events.Count; i++)
        {
            var e = events[i];
            items.Add(new EventTimelineItem
            {
                Order = i + 1,
                TimestampUtc = e.TimestampUtc,
                Event = e.Event,
                EventType = e.EventType,
                IsSignificant = e.Event.HasValue && SignificantEvents.Contains(e.Event.Value),
                SessionIndex = e.SessionIndex,
                DetailText = BuildDetailText(e),
            });
        }

        return items;
    }

    private static string? BuildDetailText(FitEvent e)
    {
        if (e.Event is null)
            return null;

        return e.Event.Value switch
        {
            FitEventKind.Timer =>
                e.TimerTrigger is not null ? $"Trigger: {e.TimerTrigger}" : null,

            FitEventKind.FrontGearChange =>
                $"Front {e.FrontGearNumber}:{e.FrontGearTeeth}",

            FitEventKind.RearGearChange =>
                $"Rear {e.RearGearNumber}:{e.RearGearTeeth}",

            FitEventKind.HrHighAlert =>
                e.HeartRateHighAlertBpm is not null ? $"HR high alert: {e.HeartRateHighAlertBpm} bpm" : null,

            FitEventKind.HrLowAlert =>
                e.HeartRateLowAlertBpm is not null ? $"HR low alert: {e.HeartRateLowAlertBpm} bpm" : null,

            FitEventKind.SpeedHighAlert =>
                e.SpeedHighAlertMetersPerSecond is not null ? $"Speed high alert: {e.SpeedHighAlertMetersPerSecond:F2} m/s" : null,

            FitEventKind.SpeedLowAlert =>
                e.SpeedLowAlertMetersPerSecond is not null ? $"Speed low alert: {e.SpeedLowAlertMetersPerSecond:F2} m/s" : null,

            FitEventKind.CadHighAlert =>
                e.CadenceHighAlertRpm is not null ? $"Cadence high alert: {e.CadenceHighAlertRpm} rpm" : null,

            FitEventKind.CadLowAlert =>
                e.CadenceLowAlertRpm is not null ? $"Cadence low alert: {e.CadenceLowAlertRpm} rpm" : null,

            FitEventKind.PowerHighAlert =>
                e.PowerHighAlertWatts is not null ? $"Power high alert: {e.PowerHighAlertWatts} W" : null,

            FitEventKind.PowerLowAlert =>
                e.PowerLowAlertWatts is not null ? $"Power low alert: {e.PowerLowAlertWatts} W" : null,

            FitEventKind.Battery or FitEventKind.BatteryLow =>
                e.BatteryLevelPercent is not null ? $"Battery: {e.BatteryLevelPercent:F0}%" : null,

            FitEventKind.RiderPositionChange =>
                e.RiderPosition is not null ? $"Position: {e.RiderPosition}" : null,

            _ => null,
        };
    }
}
