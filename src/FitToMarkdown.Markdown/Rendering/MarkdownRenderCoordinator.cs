using System.Globalization;
using System.Text;
using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Summaries;
using FitToMarkdown.Markdown.Formatting;
using FitToMarkdown.Markdown.Rendering.Sections;

namespace FitToMarkdown.Markdown.Rendering;

internal sealed class MarkdownRenderCoordinator
{
    internal string Render(FitMarkdownDocument document, CancellationToken cancellationToken)
    {
        var sb = new StringBuilder();

        // 1. Frontmatter
        if (ShouldRender(document.SectionDecisions, FitMarkdownSectionKey.Frontmatter))
        {
            RenderFrontmatter(document, sb);
        }

        // 2. Document heading (always rendered)
        DocumentHeaderRenderer.Render(document, sb);

        cancellationToken.ThrowIfCancellationRequested();

        // 3. Overview
        if (ShouldRender(document.SectionDecisions, FitMarkdownSectionKey.Overview))
        {
            RenderOverview(document.Overview, sb);
        }

        // 4. Multi-session summary table
        if (ShouldRender(document.SectionDecisions, FitMarkdownSectionKey.SessionSummary) && document.SessionSections.Count > 1)
        {
            RenderMultiSessionSummary(document.SessionSections, sb);
        }

        cancellationToken.ThrowIfCancellationRequested();

        // 5. Per-session detail blocks
        if (ShouldRender(document.SectionDecisions, FitMarkdownSectionKey.SessionDetails))
        {
            for (int i = 0; i < document.SessionSections.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                SessionBlockRenderer.Render(document.SessionSections[i], i, sb);
            }
        }

        // 6. Global record summary
        if (document.GlobalRecordSummary is not null && ShouldRender(document.SectionDecisions, FitMarkdownSectionKey.RecordSummary))
        {
            RenderGlobalRecordSummary(document.GlobalRecordSummary, sb);
        }

        // 7. Global record time series
        if (document.GlobalRecordSamples.Count > 0 && ShouldRender(document.SectionDecisions, FitMarkdownSectionKey.RecordTimeSeries))
        {
            RenderGlobalRecordTimeSeries(document.GlobalRecordSamples, sb);
        }

        cancellationToken.ThrowIfCancellationRequested();

        // 8. Heart Rate Zones
        if (document.HeartRateZones is not null && ShouldRender(document.SectionDecisions, FitMarkdownSectionKey.HeartRateZones))
        {
            HeartRateZonesRenderer.Render(document.HeartRateZones, sb);
        }

        // 9. HRV
        if (document.HrvSummary is not null && ShouldRender(document.SectionDecisions, FitMarkdownSectionKey.HrvData))
        {
            HrvRenderer.Render(document.HrvSummary, sb);
        }

        // 10. Devices
        if (document.Devices.Count > 0 && ShouldRender(document.SectionDecisions, FitMarkdownSectionKey.Devices))
        {
            DevicesRenderer.Render(document.Devices, sb);
        }

        // 11. Events
        if (document.Events.Count > 0 && ShouldRender(document.SectionDecisions, FitMarkdownSectionKey.Events))
        {
            EventsRenderer.Render(document.Events, sb);
        }

        // 12. Developer Fields
        if (document.DeveloperFieldGroups.Count > 0 && ShouldRender(document.SectionDecisions, FitMarkdownSectionKey.DeveloperFields))
        {
            DeveloperFieldsRenderer.Render(document.DeveloperFieldGroups, sb);
        }

        // 13. Workout
        if (document.WorkoutSteps.Count > 0 && ShouldRender(document.SectionDecisions, FitMarkdownSectionKey.Workout))
        {
            WorkoutRenderer.Render(document.WorkoutSteps, sb);
        }

        // 14. Course
        if (document.CoursePoints.Count > 0 && ShouldRender(document.SectionDecisions, FitMarkdownSectionKey.Course))
        {
            CourseRenderer.Render(document.CoursePoints, sb);
        }

        // 15. Segment Laps
        if (document.SegmentLapRows.Count > 0 && ShouldRender(document.SectionDecisions, FitMarkdownSectionKey.SegmentLaps))
        {
            SegmentLapsRenderer.Render(document.SegmentLapRows, sb);
        }

        // 16. User Profile
        if (ShouldRender(document.SectionDecisions, FitMarkdownSectionKey.UserProfile))
        {
            var userProfile = document.Source.ActivityContent?.UserProfile;
            if (userProfile is not null)
            {
                UserProfileRenderer.Render(userProfile, sb);
            }
        }

        // Ensure trailing newline
        var result = sb.ToString();
        if (result.Length > 0 && result[^1] != '\n')
        {
            result += '\n';
        }

        return result;
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

    private static void RenderFrontmatter(FitMarkdownDocument document, StringBuilder sb)
    {
        var fm = document.Frontmatter;
        sb.Append("---\n");

        AppendYaml(sb, YamlScalarFormatter.FormatScalar("file_type", fm.FileType?.ToString()));
        AppendYaml(sb, YamlScalarFormatter.FormatScalar("sport", fm.Sport?.ToString()));
        AppendYaml(sb, YamlScalarFormatter.FormatScalar("sub_sport", fm.SubSport?.ToString()));

        if (fm.TimeCreatedUtc is not null)
            AppendYaml(sb, YamlScalarFormatter.FormatScalar("time_created", fm.TimeCreatedUtc.Value));

        AppendYaml(sb, YamlScalarFormatter.FormatScalar("manufacturer", fm.ManufacturerName));
        AppendYaml(sb, YamlScalarFormatter.FormatScalar("product", fm.ProductName));

        if (fm.SerialNumber is not null)
            AppendYaml(sb, YamlScalarFormatter.FormatInteger("serial_number", (int)fm.SerialNumber.Value));

        AppendYaml(sb, YamlScalarFormatter.FormatNumeric("distance_meters", fm.DistanceMeters, 2));
        AppendYaml(sb, YamlScalarFormatter.FormatNumeric("duration_seconds", fm.DurationSeconds, 2));

        if (fm.AverageHeartRateBpm is not null)
            AppendYaml(sb, YamlScalarFormatter.FormatInteger("avg_heart_rate_bpm", fm.AverageHeartRateBpm.Value));

        if (fm.MaximumHeartRateBpm is not null)
            AppendYaml(sb, YamlScalarFormatter.FormatInteger("max_heart_rate_bpm", fm.MaximumHeartRateBpm.Value));

        AppendYaml(sb, YamlScalarFormatter.FormatNumeric("avg_speed_mps", fm.AverageSpeedMetersPerSecond, 2));

        if (fm.AveragePowerWatts is not null)
            AppendYaml(sb, YamlScalarFormatter.FormatInteger("avg_power_watts", fm.AveragePowerWatts.Value));

        if (fm.TotalAscentMeters is not null)
            AppendYaml(sb, YamlScalarFormatter.FormatInteger("total_ascent_meters", fm.TotalAscentMeters.Value));

        if (fm.TotalDescentMeters is not null)
            AppendYaml(sb, YamlScalarFormatter.FormatInteger("total_descent_meters", fm.TotalDescentMeters.Value));

        if (fm.TotalCalories is not null)
            AppendYaml(sb, YamlScalarFormatter.FormatInteger("total_calories", fm.TotalCalories.Value));

        AppendYaml(sb, YamlScalarFormatter.FormatInteger("session_count", fm.SessionCount));
        AppendYaml(sb, YamlScalarFormatter.FormatInteger("lap_count", fm.LapCount));
        AppendYaml(sb, YamlScalarFormatter.FormatInteger("record_count", fm.RecordCount));
        AppendYaml(sb, YamlScalarFormatter.FormatNumeric("pool_length_meters", fm.PoolLengthMeters, 2));

        sb.Append("---\n\n");
    }

    private static void AppendYaml(StringBuilder sb, string? yamlLine)
    {
        if (yamlLine is not null)
            sb.Append(yamlLine);
    }

    private static void RenderOverview(FitOverviewMetrics overview, StringBuilder sb)
    {
        var writer = new MarkdownTextWriter();
        writer.AppendHeading(2, "Overview");

        if (overview.PrimarySport is not null)
        {
            string sportText = overview.PrimarySubSport is not null && overview.PrimarySubSport != FitSubSport.Generic
                ? $"{overview.PrimarySport} ({overview.PrimarySubSport})"
                : overview.PrimarySport.ToString()!;
            writer.AppendBulletItem($"**Sport:** {sportText}");
        }

        if (overview.IsMultiSport)
            writer.AppendBulletItem("**Multi-Sport:** Yes");

        if (overview.StartTimeUtc is not null)
            writer.AppendBulletItem($"**Start Time:** {MarkdownValueFormatter.FormatTimestampBody(overview.StartTimeUtc.Value)}");

        if (overview.TotalDistanceMeters is not null)
            writer.AppendBulletItem($"**Distance:** {MarkdownValueFormatter.FormatDistanceKilometersAndMiles(overview.TotalDistanceMeters.Value)}");

        if (overview.TotalElapsedTime is not null)
            writer.AppendBulletItem($"**Elapsed Time:** {MarkdownValueFormatter.FormatDuration(overview.TotalElapsedTime.Value)}");

        if (overview.TotalTimerTime is not null)
            writer.AppendBulletItem($"**Timer Time:** {MarkdownValueFormatter.FormatDuration(overview.TotalTimerTime.Value)}");

        if (overview.Calories is not null)
            writer.AppendBulletItem($"**Calories:** {MarkdownValueFormatter.FormatCalories(overview.Calories.Value)} kcal");

        if (overview.AverageHeartRateBpm is not null)
            writer.AppendBulletItem($"**Avg HR:** {MarkdownValueFormatter.FormatHeartRate(overview.AverageHeartRateBpm.Value)} bpm");

        if (overview.MaximumHeartRateBpm is not null)
            writer.AppendBulletItem($"**Max HR:** {MarkdownValueFormatter.FormatHeartRate(overview.MaximumHeartRateBpm.Value)} bpm");

        if (overview.AverageSpeedMetersPerSecond is not null)
            writer.AppendBulletItem($"**Avg Speed:** {MarkdownValueFormatter.FormatSpeed(overview.AverageSpeedMetersPerSecond.Value)} m/s");

        if (overview.AveragePaceSecondsPerKilometer is not null && overview.AverageSpeedMetersPerSecond is not null)
            writer.AppendBulletItem($"**Avg Pace:** {MarkdownValueFormatter.FormatPace(overview.AverageSpeedMetersPerSecond.Value)} /km");

        if (overview.TotalAscentMeters is not null)
            writer.AppendBulletItem($"**Total Ascent:** {overview.TotalAscentMeters.Value.ToString(CultureInfo.InvariantCulture)} m");

        if (overview.TotalDescentMeters is not null)
            writer.AppendBulletItem($"**Total Descent:** {overview.TotalDescentMeters.Value.ToString(CultureInfo.InvariantCulture)} m");

        if (overview.SessionCount > 0)
            writer.AppendBulletItem($"**Sessions:** {overview.SessionCount.ToString(CultureInfo.InvariantCulture)}");

        if (overview.LapCount > 0)
            writer.AppendBulletItem($"**Laps:** {overview.LapCount.ToString(CultureInfo.InvariantCulture)}");

        if (overview.LengthCount > 0)
            writer.AppendBulletItem($"**Lengths:** {overview.LengthCount.ToString(CultureInfo.InvariantCulture)}");

        if (overview.RecordCount > 0)
            writer.AppendBulletItem($"**Records:** {overview.RecordCount.ToString(CultureInfo.InvariantCulture)}");

        sb.Append(writer.ToString());
    }

    private static void RenderMultiSessionSummary(IReadOnlyList<SessionSection> sessions, StringBuilder sb)
    {
        var writer = new MarkdownTextWriter();
        writer.AppendHeading(2, "Sessions");

        var headers = new[] { "#", "Sport", "Distance", "Duration", "Avg HR" };
        var rows = new List<string[]>();

        for (int i = 0; i < sessions.Count; i++)
        {
            var s = sessions[i].Session;
            string sport = s.Sport?.ToString() ?? "Unknown";
            string distance = s.Metrics.TotalDistanceMeters is not null
                ? MarkdownValueFormatter.FormatDistanceKilometersAndMiles(s.Metrics.TotalDistanceMeters.Value)
                : string.Empty;
            string duration = s.Metrics.TotalElapsedTime is not null
                ? MarkdownValueFormatter.FormatDuration(s.Metrics.TotalElapsedTime.Value)
                : string.Empty;
            string avgHr = s.Metrics.AverageHeartRateBpm is not null
                ? MarkdownValueFormatter.FormatHeartRate(s.Metrics.AverageHeartRateBpm.Value) + " bpm"
                : string.Empty;

            rows.Add(
            [
                (i + 1).ToString(CultureInfo.InvariantCulture),
                sport,
                distance,
                duration,
                avgHr,
            ]);
        }

        writer.AppendTable(headers, rows);
        sb.Append(writer.ToString());
    }

    private static void RenderGlobalRecordSummary(RecordStatisticsSummary summary, StringBuilder sb)
    {
        var writer = new MarkdownTextWriter();
        writer.AppendHeading(2, "Record Statistics");

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
        sb.Append(writer.ToString());
    }

    private static void RenderGlobalRecordTimeSeries(IReadOnlyList<SampledTimeSeriesRow> samples, StringBuilder sb)
    {
        var writer = new MarkdownTextWriter();
        writer.AppendHeading(2, "Time Series Data");

        string csv = CsvSampleRenderer.RenderCsvBlock(samples);
        if (!string.IsNullOrEmpty(csv))
            writer.AppendRaw(csv);

        sb.Append(writer.ToString());
    }
}
