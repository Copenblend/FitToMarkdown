using System.Globalization;
using System.Text;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Summaries;
using FitToMarkdown.Markdown.Formatting;

namespace FitToMarkdown.Markdown.Rendering.Sections;

internal static class SessionBlockRenderer
{
    internal static void Render(SessionSection session, int sessionIndex, StringBuilder sb)
    {
        var writer = new MarkdownTextWriter();
        var s = session.Session;

        // H2: Session heading
        string sport = s.Sport?.ToString() ?? "Unknown";
        string subSport = s.SubSport is not null && s.SubSport != FitSubSport.Generic
            ? $" \u2014 {s.SubSport}"
            : string.Empty;
        writer.AppendHeading(2, $"Session {(sessionIndex + 1).ToString(CultureInfo.InvariantCulture)}: {sport}{subSport}");

        // Session summary metric table
        var (summaryHeaders, summaryRows) = MarkdownTableLayoutService.SelectSessionSummaryRows(s);
        if (summaryRows.Count > 0)
            writer.AppendTable(summaryHeaders, summaryRows);

        // Laps
        if (session.LapRows.Count > 0 && ShouldRender(session.SectionDecisions, FitMarkdownSectionKey.LapDetails))
        {
            bool isRunning = s.Sport is FitSport.Running or FitSport.Walking;
            bool isCycling = s.Sport is FitSport.Cycling;
            bool isSwim = s.Sport is FitSport.Swimming;
            var (lapHeaders, lapRows) = MarkdownTableLayoutService.SelectLapTableColumns(session.LapRows, isRunning, isCycling, isSwim);
            writer.AppendHeading(3, "Laps");
            writer.AppendTable(lapHeaders, lapRows);
        }

        // Lengths (pool swim)
        if (session.LengthRows.Count > 0 && ShouldRender(session.SectionDecisions, FitMarkdownSectionKey.LengthDetails))
        {
            writer.AppendHeading(3, "Lengths");

            var lengthHeaders = new[] { "Length", "Type", "Stroke", "Duration", "Strokes", "SWOLF", "Pace/100m" };
            var lengthRows = new List<string[]>();

            foreach (var length in session.LengthRows)
            {
                string duration = length.Duration is not null
                    ? MarkdownValueFormatter.FormatDuration(length.Duration.Value)
                    : string.Empty;

                string strokes = length.TotalStrokes is not null
                    ? length.TotalStrokes.Value.ToString(CultureInfo.InvariantCulture)
                    : string.Empty;

                string swolf = length.Swolf is not null
                    ? length.Swolf.Value.ToString(CultureInfo.InvariantCulture)
                    : string.Empty;

                string pace = length.PaceSecondsPer100Meters is not null
                    ? MarkdownValueFormatter.FormatPacePer100m(100.0 / length.PaceSecondsPer100Meters.Value)
                    : string.Empty;

                lengthRows.Add(
                [
                    length.LengthNumber.ToString(CultureInfo.InvariantCulture),
                    length.LengthType?.ToString() ?? string.Empty,
                    length.SwimStroke?.ToString() ?? string.Empty,
                    duration,
                    strokes,
                    swolf,
                    pace,
                ]);
            }

            writer.AppendTable(lengthHeaders, lengthRows);
        }

        // Record Statistics
        if (session.RecordSummary is not null && ShouldRender(session.SectionDecisions, FitMarkdownSectionKey.RecordSummary))
        {
            writer.AppendHeading(3, "Record Statistics");
            RenderMetricStatisticsTable(writer, session.RecordSummary);
        }

        // Record Time Series
        if (session.RecordSamples.Count > 0 && ShouldRender(session.SectionDecisions, FitMarkdownSectionKey.RecordTimeSeries))
        {
            writer.AppendHeading(3, "Time Series Data");
            string csv = CsvSampleRenderer.RenderCsvBlock(session.RecordSamples);
            if (!string.IsNullOrEmpty(csv))
                writer.AppendRaw(csv);
        }

        // Pauses
        if (session.Pauses.Count > 0)
        {
            writer.AppendHeading(3, "Pauses");
            var pauseHeaders = new[] { "Start", "End", "Duration" };
            var pauseRows = new List<string[]>();

            foreach (var pause in session.Pauses)
            {
                pauseRows.Add(
                [
                    MarkdownValueFormatter.FormatTimestampEvent(pause.StartUtc),
                    MarkdownValueFormatter.FormatTimestampEvent(pause.EndUtc),
                    MarkdownValueFormatter.FormatDuration(pause.Duration),
                ]);
            }

            writer.AppendTable(pauseHeaders, pauseRows);
        }

        sb.Append(writer.ToString());
    }

    private static void RenderMetricStatisticsTable(MarkdownTextWriter writer, RecordStatisticsSummary summary)
    {
        var headers = new[] { "Metric", "Min", "Avg", "Max", "Std Dev", "Samples" };
        var rows = new List<string[]>();

        foreach (var metric in summary.Metrics)
        {
            string unit = !string.IsNullOrEmpty(metric.UnitSymbol) ? $" {metric.UnitSymbol}" : string.Empty;

            rows.Add(
            [
                metric.DisplayName,
                metric.Minimum is not null ? $"{metric.Minimum.Value.ToString("F2", CultureInfo.InvariantCulture)}{unit}" : string.Empty,
                metric.Average is not null ? $"{metric.Average.Value.ToString("F2", CultureInfo.InvariantCulture)}{unit}" : string.Empty,
                metric.Maximum is not null ? $"{metric.Maximum.Value.ToString("F2", CultureInfo.InvariantCulture)}{unit}" : string.Empty,
                metric.StandardDeviation is not null ? metric.StandardDeviation.Value.ToString("F2", CultureInfo.InvariantCulture) : string.Empty,
                metric.SampleCount.ToString(CultureInfo.InvariantCulture),
            ]);
        }

        writer.AppendTable(headers, rows);
    }

    private static bool ShouldRender(IReadOnlyList<SectionRenderDecision> decisions, FitMarkdownSectionKey key)
    {
        foreach (var decision in decisions)
        {
            if (decision.Section == key)
                return decision.ShouldRender;
        }

        return false;
    }
}
