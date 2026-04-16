using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.Summaries;

namespace FitToMarkdown.Markdown.Formatting;

internal static class MarkdownTableLayoutService
{
    internal static (string[] FilteredHeaders, List<string[]> FilteredRows) FilterEmptyColumns(
        string[] headers, List<string[]> rows)
    {
        if (headers.Length == 0 || rows.Count == 0)
            return (headers, rows);

        // Determine which columns have any non-empty data
        var hasData = new bool[headers.Length];
        foreach (var row in rows)
        {
            for (int i = 0; i < headers.Length && i < row.Length; i++)
            {
                if (!string.IsNullOrEmpty(row[i]))
                    hasData[i] = true;
            }
        }

        // Build indices of columns that have data
        var keepIndices = new List<int>();
        for (int i = 0; i < headers.Length; i++)
        {
            if (hasData[i])
                keepIndices.Add(i);
        }

        if (keepIndices.Count == headers.Length)
            return (headers, rows);

        if (keepIndices.Count == 0)
            return ([], []);

        var filteredHeaders = keepIndices.Select(i => headers[i]).ToArray();
        var filteredRows = rows.Select(row =>
            keepIndices.Select(i => i < row.Length ? row[i] : string.Empty).ToArray()
        ).ToList();

        return (filteredHeaders, filteredRows);
    }

    internal static (string[] Headers, List<string[]> Rows) SelectLapTableColumns(
        IReadOnlyList<LapTableRow> rows, bool isRunning, bool isCycling, bool isSwim)
    {
        var headers = new List<string> { "Lap" };
        var formatters = new List<Func<LapTableRow, string>> { r => r.LapNumber.ToString() };

        headers.Add("Distance");
        formatters.Add(r => r.DistanceMeters is not null
            ? MarkdownValueFormatter.FormatDistanceKilometers(r.DistanceMeters.Value)
            : string.Empty);

        headers.Add("Duration");
        formatters.Add(r => r.Duration is not null
            ? MarkdownValueFormatter.FormatDuration(r.Duration.Value)
            : string.Empty);

        if (isRunning || isSwim)
        {
            headers.Add("Pace");
            formatters.Add(r => r.AverageSpeedMetersPerSecond is not null
                ? (isSwim
                    ? MarkdownValueFormatter.FormatPacePer100m(r.AverageSpeedMetersPerSecond.Value)
                    : MarkdownValueFormatter.FormatPace(r.AverageSpeedMetersPerSecond.Value))
                : string.Empty);
        }
        else
        {
            headers.Add("Speed");
            formatters.Add(r => r.AverageSpeedMetersPerSecond is not null
                ? MarkdownValueFormatter.FormatSpeed(r.AverageSpeedMetersPerSecond.Value)
                : string.Empty);
        }

        headers.Add("Avg HR");
        formatters.Add(r => r.AverageHeartRateBpm is not null
            ? MarkdownValueFormatter.FormatHeartRate(r.AverageHeartRateBpm.Value)
            : string.Empty);

        headers.Add("Max HR");
        formatters.Add(r => r.MaximumHeartRateBpm is not null
            ? MarkdownValueFormatter.FormatHeartRate(r.MaximumHeartRateBpm.Value)
            : string.Empty);

        if (isRunning || isCycling)
        {
            headers.Add("Cadence");
            formatters.Add(r => r.AverageCadenceRpm is not null
                ? MarkdownValueFormatter.FormatCadenceBody(r.AverageCadenceRpm.Value, isRunning)
                : string.Empty);
        }

        if (isCycling)
        {
            headers.Add("Power");
            formatters.Add(r => r.AveragePowerWatts is not null
                ? MarkdownValueFormatter.FormatPower(r.AveragePowerWatts.Value)
                : string.Empty);
        }

        headers.Add("Trigger");
        formatters.Add(r => r.Trigger?.ToString() ?? string.Empty);

        var dataRows = rows.Select(r =>
            formatters.Select(f => f(r)).ToArray()
        ).ToList();

        return (headers.ToArray(), dataRows);
    }

    internal static (string[] Headers, List<string[]> Rows) SelectSessionSummaryRows(FitSession session)
    {
        var headers = new string[] { "Metric", "Value" };
        var rows = new List<string[]>();

        AddRowIfPresent(rows, "Sport", session.Sport?.ToString());
        AddRowIfPresent(rows, "Sub-Sport", session.SubSport?.ToString());

        if (session.Metrics.TotalDistanceMeters is not null)
            rows.Add(["Distance", MarkdownValueFormatter.FormatDistanceKilometersAndMiles(session.Metrics.TotalDistanceMeters.Value)]);

        if (session.Metrics.TotalElapsedTime is not null)
            rows.Add(["Elapsed Time", MarkdownValueFormatter.FormatDuration(session.Metrics.TotalElapsedTime.Value)]);

        if (session.Metrics.TotalTimerTime is not null)
            rows.Add(["Timer Time", MarkdownValueFormatter.FormatDuration(session.Metrics.TotalTimerTime.Value)]);

        if (session.Metrics.AverageSpeedMetersPerSecond is not null)
        {
            bool isRunning = session.Sport is FitSport.Running or FitSport.Walking;
            if (isRunning)
                rows.Add(["Avg Pace", MarkdownValueFormatter.FormatPace(session.Metrics.AverageSpeedMetersPerSecond.Value) + " /km"]);
            else
                rows.Add(["Avg Speed", MarkdownValueFormatter.FormatSpeed(session.Metrics.AverageSpeedMetersPerSecond.Value) + " m/s"]);
        }

        if (session.Metrics.AverageHeartRateBpm is not null)
            rows.Add(["Avg HR", MarkdownValueFormatter.FormatHeartRate(session.Metrics.AverageHeartRateBpm.Value) + " bpm"]);

        if (session.Metrics.MaximumHeartRateBpm is not null)
            rows.Add(["Max HR", MarkdownValueFormatter.FormatHeartRate(session.Metrics.MaximumHeartRateBpm.Value) + " bpm"]);

        if (session.Metrics.AverageCadenceRpm is not null)
        {
            bool isRunning = session.Sport is FitSport.Running or FitSport.Walking;
            rows.Add(["Avg Cadence", MarkdownValueFormatter.FormatCadenceBody(session.Metrics.AverageCadenceRpm.Value, isRunning) + " rpm"]);
        }

        if (session.Metrics.AveragePowerWatts is not null)
            rows.Add(["Avg Power", MarkdownValueFormatter.FormatPower(session.Metrics.AveragePowerWatts.Value) + " W"]);

        if (session.Metrics.TotalAscentMeters is not null)
            rows.Add(["Total Ascent", MarkdownValueFormatter.FormatAltitude(session.Metrics.TotalAscentMeters.Value) + " m"]);

        if (session.Metrics.TotalDescentMeters is not null)
            rows.Add(["Total Descent", MarkdownValueFormatter.FormatAltitude(session.Metrics.TotalDescentMeters.Value) + " m"]);

        if (session.Metrics.TotalCalories is not null)
            rows.Add(["Calories", MarkdownValueFormatter.FormatCalories(session.Metrics.TotalCalories.Value) + " kcal"]);

        if (session.Metrics.NormalizedPowerWatts is not null)
            rows.Add(["Normalized Power", MarkdownValueFormatter.FormatPower(session.Metrics.NormalizedPowerWatts.Value) + " W"]);

        return (headers, rows);
    }

    private static void AddRowIfPresent(List<string[]> rows, string label, string? value)
    {
        if (!string.IsNullOrEmpty(value))
            rows.Add([label, value]);
    }
}
