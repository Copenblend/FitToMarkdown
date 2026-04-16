using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Summaries;

namespace FitToMarkdown.Markdown.Projection;

internal static class HrvProjector
{
    internal static HrvSummary? Project(FitFileDocument document)
    {
        var activity = document.ActivityContent;
        if (activity is null || activity.HrvMessages.Count == 0)
            return null;

        // Collect all valid RR intervals (> 0)
        var intervals = new List<double>();
        foreach (var hrv in activity.HrvMessages)
        {
            foreach (double rr in hrv.RrIntervalsSeconds)
            {
                if (rr > 0)
                    intervals.Add(rr);
            }
        }

        if (intervals.Count == 0)
            return null;

        // Convert to milliseconds
        int count = intervals.Count;
        double sumMs = 0;
        double minMs = double.MaxValue;
        double maxMs = double.MinValue;

        for (int i = 0; i < count; i++)
        {
            double ms = intervals[i] * 1000.0;
            sumMs += ms;
            if (ms < minMs) minMs = ms;
            if (ms > maxMs) maxMs = ms;
        }

        double avgMs = sumMs / count;

        // SDNN: standard deviation of all intervals
        double sumSqDev = 0;
        for (int i = 0; i < count; i++)
        {
            double ms = intervals[i] * 1000.0;
            double dev = ms - avgMs;
            sumSqDev += dev * dev;
        }

        double sdnn = Math.Sqrt(sumSqDev / count);

        // RMSSD: root mean square of successive differences
        double? rmssd = null;
        if (count > 1)
        {
            double sumSqDiff = 0;
            for (int i = 1; i < count; i++)
            {
                double diff = (intervals[i] - intervals[i - 1]) * 1000.0;
                sumSqDiff += diff * diff;
            }

            rmssd = Math.Sqrt(sumSqDiff / (count - 1));
        }

        return new HrvSummary
        {
            SampleCount = count,
            AverageRrMilliseconds = Math.Round(avgMs, 2),
            MinimumRrMilliseconds = Math.Round(minMs, 2),
            MaximumRrMilliseconds = Math.Round(maxMs, 2),
            SdnnMilliseconds = Math.Round(sdnn, 2),
            RmssdMilliseconds = rmssd is not null ? Math.Round(rmssd.Value, 2) : null,
        };
    }
}
