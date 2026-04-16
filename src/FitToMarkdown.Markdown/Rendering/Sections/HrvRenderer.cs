using System.Globalization;
using System.Text;
using FitToMarkdown.Core.Summaries;
using FitToMarkdown.Markdown.Formatting;

namespace FitToMarkdown.Markdown.Rendering.Sections;

internal static class HrvRenderer
{
    internal static void Render(HrvSummary hrv, StringBuilder sb)
    {
        var writer = new MarkdownTextWriter();
        writer.AppendHeading(2, "Heart Rate Variability");

        writer.AppendBulletItem($"**Sample Count:** {hrv.SampleCount.ToString(CultureInfo.InvariantCulture)}");

        if (hrv.AverageRrMilliseconds is not null)
            writer.AppendBulletItem($"**Avg RR:** {hrv.AverageRrMilliseconds.Value.ToString("F1", CultureInfo.InvariantCulture)} ms");

        if (hrv.RmssdMilliseconds is not null)
            writer.AppendBulletItem($"**RMSSD:** {hrv.RmssdMilliseconds.Value.ToString("F1", CultureInfo.InvariantCulture)} ms");

        if (hrv.SdnnMilliseconds is not null)
            writer.AppendBulletItem($"**SDNN:** {hrv.SdnnMilliseconds.Value.ToString("F1", CultureInfo.InvariantCulture)} ms");

        if (hrv.MinimumRrMilliseconds is not null)
            writer.AppendBulletItem($"**Min RR:** {hrv.MinimumRrMilliseconds.Value.ToString("F1", CultureInfo.InvariantCulture)} ms");

        if (hrv.MaximumRrMilliseconds is not null)
            writer.AppendBulletItem($"**Max RR:** {hrv.MaximumRrMilliseconds.Value.ToString("F1", CultureInfo.InvariantCulture)} ms");

        sb.Append(writer.ToString());
    }
}
