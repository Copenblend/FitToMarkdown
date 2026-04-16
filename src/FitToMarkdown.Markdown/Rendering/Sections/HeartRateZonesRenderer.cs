using System.Text;
using FitToMarkdown.Core.Summaries;
using FitToMarkdown.Markdown.Formatting;

namespace FitToMarkdown.Markdown.Rendering.Sections;

internal static class HeartRateZonesRenderer
{
    internal static void Render(HeartRateZoneSummary zones, StringBuilder sb)
    {
        var writer = new MarkdownTextWriter();
        writer.AppendHeading(2, "Heart Rate Zones");

        if (!string.IsNullOrEmpty(zones.Method))
        {
            writer.AppendParagraph($"Method: {MarkdownEscaper.EscapeMarkdownText(zones.Method)}");
        }

        if (zones.MaximumHeartRateBpm is not null)
        {
            writer.AppendParagraph($"Max HR: {MarkdownValueFormatter.FormatHeartRate(zones.MaximumHeartRateBpm.Value)} bpm");
        }

        if (zones.ThresholdHeartRateBpm is not null)
        {
            writer.AppendParagraph($"Threshold HR: {MarkdownValueFormatter.FormatHeartRate(zones.ThresholdHeartRateBpm.Value)} bpm");
        }

        if (zones.Zones.Count > 0)
        {
            var headers = new[] { "Zone", "Range (bpm)", "Time", "% of Total" };
            var rows = new List<string[]>();

            foreach (var zone in zones.Zones)
            {
                string range = zone.LowerBoundBpm is not null && zone.UpperBoundBpm is not null
                    ? $"{MarkdownValueFormatter.FormatHeartRate(zone.LowerBoundBpm.Value)}-{MarkdownValueFormatter.FormatHeartRate(zone.UpperBoundBpm.Value)}"
                    : string.Empty;

                string time = zone.TimeInZone is not null
                    ? MarkdownValueFormatter.FormatDuration(zone.TimeInZone.Value)
                    : string.Empty;

                string percent = zone.PercentOfTotalTime is not null
                    ? MarkdownValueFormatter.FormatPercent(zone.PercentOfTotalTime.Value) + "%"
                    : string.Empty;

                rows.Add([zone.Zone.ToString(), range, time, percent]);
            }

            writer.AppendTable(headers, rows);
        }

        sb.Append(writer.ToString());
    }
}
