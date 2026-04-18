using System.Globalization;
using System.Text;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.Summaries;
using FitToMarkdown.Markdown.Formatting;

namespace FitToMarkdown.Markdown.Rendering.Sections;

internal static class DeveloperFieldsRenderer
{
    internal static void Render(IReadOnlyList<DeveloperFieldGroup> groups, StringBuilder sb)
    {
        var writer = new MarkdownTextWriter();
        writer.AppendHeading(2, "Developer Fields");

        foreach (var group in groups)
        {
            string subtitle = BuildGroupSubtitle(group);
            writer.AppendHeading(3, MarkdownEscaper.EscapeHeading(subtitle));

            foreach (var value in group.Values)
            {
                string display = FormatFieldValue(value);
                string units = !string.IsNullOrEmpty(value.Units) ? $" {MarkdownEscaper.SanitizeFitString(value.Units)}" : string.Empty;
                string name = MarkdownEscaper.EscapeMarkdownText(MarkdownEscaper.SanitizeFitString(value.FieldName) ?? "Unknown");
                writer.AppendBulletItem($"**{name}:** {display}{units}");
            }
        }

        sb.Append(writer.ToString());
    }

    private static string BuildGroupSubtitle(DeveloperFieldGroup group)
    {
        return group.OwnerType switch
        {
            FitDeveloperFieldOwnerType.Session when group.SessionIndex is not null =>
                $"Session {(group.SessionIndex.Value + 1).ToString(CultureInfo.InvariantCulture)}",
            FitDeveloperFieldOwnerType.Lap when group.LapIndex is not null =>
                $"Lap {(group.LapIndex.Value + 1).ToString(CultureInfo.InvariantCulture)}",
            FitDeveloperFieldOwnerType.Length when group.LengthIndex is not null =>
                $"Length {(group.LengthIndex.Value + 1).ToString(CultureInfo.InvariantCulture)}",
            FitDeveloperFieldOwnerType.Record when group.RecordIndex is not null =>
                $"Record {(group.RecordIndex.Value + 1).ToString(CultureInfo.InvariantCulture)}",
            FitDeveloperFieldOwnerType.Event when group.EventIndex is not null =>
                $"Event {(group.EventIndex.Value + 1).ToString(CultureInfo.InvariantCulture)}",
            _ => group.OwnerType.ToString(),
        };
    }

    private static string FormatFieldValue(FitDeveloperFieldValue value)
    {
        if (!string.IsNullOrEmpty(value.RawDisplayValue))
            return MarkdownEscaper.SanitizeFitString(value.RawDisplayValue);

        return value.ValueKind switch
        {
            FitDeveloperValueKind.Numeric when value.NumericValue is not null =>
                value.NumericValue.Value.ToString("G", CultureInfo.InvariantCulture),
            FitDeveloperValueKind.Integer when value.IntegerValue is not null =>
                value.IntegerValue.Value.ToString(CultureInfo.InvariantCulture),
            FitDeveloperValueKind.Boolean when value.BooleanValue is not null =>
                value.BooleanValue.Value ? "true" : "false",
            FitDeveloperValueKind.Text when value.TextValue is not null =>
                MarkdownEscaper.SanitizeFitString(value.TextValue),
            FitDeveloperValueKind.NumericArray when value.NumericArrayValues.Count > 0 =>
                string.Join(", ", value.NumericArrayValues.Select(v => v.ToString("G", CultureInfo.InvariantCulture))),
            FitDeveloperValueKind.IntegerArray when value.IntegerArrayValues.Count > 0 =>
                string.Join(", ", value.IntegerArrayValues.Select(v => v.ToString(CultureInfo.InvariantCulture))),
            FitDeveloperValueKind.TextArray when value.TextArrayValues.Count > 0 =>
                string.Join(", ", value.TextArrayValues.Select(MarkdownEscaper.SanitizeFitString)),
            _ => string.Empty,
        };
    }
}
