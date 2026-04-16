using System.Globalization;
using System.Text;
using FitToMarkdown.Core.Summaries;
using FitToMarkdown.Markdown.Formatting;

namespace FitToMarkdown.Markdown.Rendering.Sections;

internal static class WorkoutRenderer
{
    internal static void Render(IReadOnlyList<WorkoutStepRow> steps, StringBuilder sb)
    {
        var writer = new MarkdownTextWriter();
        writer.AppendHeading(2, "Workout");

        var headers = new[] { "#", "Name", "Duration Type", "Duration", "Target Type", "Target", "Intensity" };
        var rows = new List<string[]>();

        foreach (var step in steps)
        {
            string durationValue = step.DurationValue is not null
                ? step.DurationValue.Value.ToString("G", CultureInfo.InvariantCulture)
                : string.Empty;

            string target = FormatTarget(step);

            rows.Add(
            [
                step.Order.ToString(CultureInfo.InvariantCulture),
                MarkdownEscaper.SanitizeFitString(step.StepName),
                step.DurationType?.ToString() ?? string.Empty,
                durationValue,
                step.TargetType?.ToString() ?? string.Empty,
                target,
                step.Intensity?.ToString() ?? string.Empty,
            ]);
        }

        writer.AppendTable(headers, rows);
        sb.Append(writer.ToString());
    }

    private static string FormatTarget(WorkoutStepRow step)
    {
        if (step.TargetRange is null)
            return string.Empty;

        var range = step.TargetRange;
        if (range.CustomLow is not null && range.CustomHigh is not null)
        {
            string unit = !string.IsNullOrEmpty(range.Unit) ? $" {range.Unit}" : string.Empty;
            return $"{range.CustomLow.Value.ToString("G", CultureInfo.InvariantCulture)}-{range.CustomHigh.Value.ToString("G", CultureInfo.InvariantCulture)}{unit}";
        }

        if (range.TargetValue is not null)
            return range.TargetValue.Value.ToString("G", CultureInfo.InvariantCulture);

        return string.Empty;
    }
}
