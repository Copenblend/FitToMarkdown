using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Summaries;

namespace FitToMarkdown.Markdown.Projection;

internal static class HeartRateZoneProjector
{
    private static readonly (FitZoneKind Zone, double LowerPct, double UpperPct)[] ZoneBounds =
    [
        (FitZoneKind.Zone1, 0.50, 0.60),
        (FitZoneKind.Zone2, 0.60, 0.70),
        (FitZoneKind.Zone3, 0.70, 0.80),
        (FitZoneKind.Zone4, 0.80, 0.90),
        (FitZoneKind.Zone5, 0.90, 1.00),
    ];

    internal static HeartRateZoneSummary? Project(FitFileDocument document)
    {
        var activity = document.ActivityContent;
        if (activity is null)
            return null;

        var zones = activity.ZonesTarget;
        if (zones is null || zones.MaximumHeartRateBpm is null)
            return null;

        byte maxHr = zones.MaximumHeartRateBpm.Value;
        string method = zones.HeartRateCalculationType ?? "percent-max-hr";

        // Build zone rows with BPM bounds
        var zoneRows = new List<HeartRateZoneRow>(ZoneBounds.Length);
        var zoneBpmRanges = new (byte Lower, byte Upper)[ZoneBounds.Length];

        for (int i = 0; i < ZoneBounds.Length; i++)
        {
            byte lower = (byte)Math.Round(ZoneBounds[i].LowerPct * maxHr);
            byte upper = (byte)Math.Round(ZoneBounds[i].UpperPct * maxHr);
            zoneBpmRanges[i] = (lower, upper);

            zoneRows.Add(new HeartRateZoneRow
            {
                Zone = ZoneBounds[i].Zone,
                LowerBoundBpm = lower,
                UpperBoundBpm = upper,
            });
        }

        // Collect HR records across all sessions
        var hrCounts = new int[ZoneBounds.Length];
        int totalHrRecords = 0;
        TimeSpan totalTimerTime = TimeSpan.Zero;

        foreach (var session in activity.Sessions)
        {
            if (session.Metrics.TotalTimerTime is not null)
                totalTimerTime += session.Metrics.TotalTimerTime.Value;

            foreach (var record in session.Records)
            {
                if (record.HeartRateBpm is not { } hr)
                    continue;

                totalHrRecords++;

                for (int i = ZoneBounds.Length - 1; i >= 0; i--)
                {
                    if (hr >= zoneBpmRanges[i].Lower)
                    {
                        hrCounts[i]++;
                        break;
                    }
                }
            }
        }

        // Apply time-in-zone if we have HR data
        if (totalHrRecords > 0)
        {
            for (int i = 0; i < ZoneBounds.Length; i++)
            {
                double fraction = (double)hrCounts[i] / totalHrRecords;
                zoneRows[i] = zoneRows[i] with
                {
                    TimeInZone = TimeSpan.FromSeconds(fraction * totalTimerTime.TotalSeconds),
                    PercentOfTotalTime = Math.Round(fraction * 100.0, 1),
                };
            }
        }

        return new HeartRateZoneSummary
        {
            Method = method,
            MaximumHeartRateBpm = maxHr,
            ThresholdHeartRateBpm = zones.ThresholdHeartRateBpm,
            Zones = zoneRows,
        };
    }
}
