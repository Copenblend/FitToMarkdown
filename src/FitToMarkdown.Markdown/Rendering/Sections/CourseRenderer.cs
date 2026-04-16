using System.Globalization;
using System.Text;
using FitToMarkdown.Core.Summaries;
using FitToMarkdown.Markdown.Formatting;

namespace FitToMarkdown.Markdown.Rendering.Sections;

internal static class CourseRenderer
{
    internal static void Render(IReadOnlyList<CoursePointRow> coursePoints, StringBuilder sb)
    {
        var writer = new MarkdownTextWriter();
        writer.AppendHeading(2, "Course Points");

        var headers = new[] { "#", "Name", "Type", "Distance", "Position" };
        var rows = new List<string[]>();

        foreach (var point in coursePoints)
        {
            string distance = point.DistanceMeters is not null
                ? MarkdownValueFormatter.FormatDistanceMeters(point.DistanceMeters.Value) + " m"
                : string.Empty;

            string position = point.Position is not null
                ? $"{MarkdownValueFormatter.FormatCoordinate(point.Position.LatitudeDegrees)}, {MarkdownValueFormatter.FormatCoordinate(point.Position.LongitudeDegrees)}"
                : string.Empty;

            rows.Add(
            [
                point.Order.ToString(CultureInfo.InvariantCulture),
                MarkdownEscaper.SanitizeFitString(point.PointName),
                point.PointType?.ToString() ?? string.Empty,
                distance,
                position,
            ]);
        }

        writer.AppendTable(headers, rows);
        sb.Append(writer.ToString());
    }
}
