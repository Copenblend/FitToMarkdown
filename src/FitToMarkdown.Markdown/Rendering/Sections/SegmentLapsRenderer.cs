using System.Globalization;
using System.Text;
using FitToMarkdown.Core.Summaries;
using FitToMarkdown.Markdown.Formatting;

namespace FitToMarkdown.Markdown.Rendering.Sections;

internal static class SegmentLapsRenderer
{
    internal static void Render(IReadOnlyList<SegmentLapTableRow> rows, StringBuilder sb)
    {
        var writer = new MarkdownTextWriter();
        writer.AppendHeading(2, "Segment Laps");

        var headers = new[] { "#", "Name", "Distance", "Duration", "Avg HR", "Cadence", "Power", "Status" };
        var tableRows = new List<string[]>();

        foreach (var row in rows)
        {
            string distance = row.DistanceMeters is not null
                ? MarkdownValueFormatter.FormatDistanceKilometers(row.DistanceMeters.Value) + " km"
                : string.Empty;

            string duration = row.Duration is not null
                ? MarkdownValueFormatter.FormatDuration(row.Duration.Value)
                : string.Empty;

            string avgHr = row.AverageHeartRateBpm is not null
                ? MarkdownValueFormatter.FormatHeartRate(row.AverageHeartRateBpm.Value)
                : string.Empty;

            string cadence = row.AverageCadenceRpm is not null
                ? ((int)row.AverageCadenceRpm.Value).ToString(CultureInfo.InvariantCulture)
                : string.Empty;

            string power = row.AveragePowerWatts is not null
                ? MarkdownValueFormatter.FormatPower(row.AveragePowerWatts.Value)
                : string.Empty;

            tableRows.Add(
            [
                row.SegmentNumber.ToString(CultureInfo.InvariantCulture),
                MarkdownEscaper.SanitizeFitString(row.Name),
                distance,
                duration,
                avgHr,
                cadence,
                power,
                MarkdownEscaper.SanitizeFitString(row.Status),
            ]);
        }

        writer.AppendTable(headers, tableRows);
        sb.Append(writer.ToString());
    }
}
