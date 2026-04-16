using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Summaries;

namespace FitToMarkdown.Markdown.Projection;

internal static class SegmentLapProjector
{
    internal static IReadOnlyList<SegmentLapTableRow> Project(FitFileDocument document)
    {
        var activity = document.ActivityContent;
        if (activity is null || activity.SegmentLaps.Count == 0)
            return [];

        var rows = new List<SegmentLapTableRow>(activity.SegmentLaps.Count);
        foreach (var segment in activity.SegmentLaps)
        {
            rows.Add(new SegmentLapTableRow
            {
                SegmentNumber = segment.SegmentIndex + 1,
                Name = segment.SegmentName,
                DistanceMeters = segment.Metrics.TotalDistanceMeters,
                Duration = segment.Metrics.TotalTimerTime,
                AverageHeartRateBpm = segment.Metrics.AverageHeartRateBpm,
                AverageCadenceRpm = segment.Metrics.AverageCadenceRpm,
                AveragePowerWatts = segment.Metrics.AveragePowerWatts,
                Status = segment.Status,
            });
        }

        return rows;
    }
}
