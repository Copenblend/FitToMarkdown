using System.Globalization;
using System.Text;
using FitToMarkdown.Core.Summaries;
using FitToMarkdown.Markdown.Formatting;

namespace FitToMarkdown.Markdown.Rendering.Sections;

internal static class EventsRenderer
{
    internal static void Render(IReadOnlyList<EventTimelineItem> events, StringBuilder sb)
    {
        var writer = new MarkdownTextWriter();
        writer.AppendHeading(2, "Events");

        var headers = new[] { "#", "Time", "Event", "Action", "Detail" };
        var rows = new List<string[]>();

        foreach (var evt in events)
        {
            string time = evt.TimestampUtc is not null
                ? MarkdownValueFormatter.FormatTimestampEvent(evt.TimestampUtc.Value)
                : string.Empty;

            rows.Add(
            [
                evt.Order.ToString(CultureInfo.InvariantCulture),
                time,
                evt.Event?.ToString() ?? string.Empty,
                evt.EventType?.ToString() ?? string.Empty,
                MarkdownEscaper.SanitizeFitString(evt.DetailText),
            ]);
        }

        writer.AppendTable(headers, rows);
        sb.Append(writer.ToString());
    }
}
