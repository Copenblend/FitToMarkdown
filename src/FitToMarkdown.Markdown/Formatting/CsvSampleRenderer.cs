using System.Globalization;
using System.Text;
using FitToMarkdown.Core.Summaries;

namespace FitToMarkdown.Markdown.Formatting;

internal static class CsvSampleRenderer
{
    private static readonly string[] AllHeaders =
    [
        "timestamp_utc",
        "offset_s",
        "distance_m",
        "speed_mps",
        "heart_rate_bpm",
        "cadence_rpm",
        "power_w",
        "altitude_m",
        "temperature_c",
        "latitude_deg",
        "longitude_deg",
    ];

    internal static string RenderCsvBlock(IReadOnlyList<SampledTimeSeriesRow> samples)
    {
        if (samples.Count == 0)
            return string.Empty;

        // Determine which columns have data
        var hasData = new bool[AllHeaders.Length];
        foreach (var row in samples)
        {
            if (row.TimestampUtc is not null) hasData[0] = true;
            if (row.OffsetFromSessionStart is not null) hasData[1] = true;
            if (row.DistanceMeters is not null) hasData[2] = true;
            if (row.SpeedMetersPerSecond is not null) hasData[3] = true;
            if (row.HeartRateBpm is not null) hasData[4] = true;
            if (row.CadenceRpm is not null) hasData[5] = true;
            if (row.PowerWatts is not null) hasData[6] = true;
            if (row.AltitudeMeters is not null) hasData[7] = true;
            if (row.TemperatureCelsius is not null) hasData[8] = true;
            if (row.Position is not null) { hasData[9] = true; hasData[10] = true; }
        }

        var keepIndices = new List<int>();
        for (int i = 0; i < AllHeaders.Length; i++)
        {
            if (hasData[i])
                keepIndices.Add(i);
        }

        if (keepIndices.Count == 0)
            return string.Empty;

        var sb = new StringBuilder();

        // Header row
        sb.Append(string.Join(",", keepIndices.Select(i => AllHeaders[i])));
        sb.Append('\n');

        // Data rows
        foreach (var row in samples)
        {
            var cells = new List<string>();
            foreach (int idx in keepIndices)
            {
                cells.Add(FormatCell(idx, row));
            }
            sb.Append(string.Join(",", cells));
            sb.Append('\n');
        }

        var csv = sb.ToString();

        return $"```csv\n{csv}```\n";
    }

    private static string FormatCell(int columnIndex, SampledTimeSeriesRow row)
    {
        string? value = columnIndex switch
        {
            0 => row.TimestampUtc?.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
            1 => row.OffsetFromSessionStart?.TotalSeconds.ToString("F1", CultureInfo.InvariantCulture),
            2 => row.DistanceMeters?.ToString("F2", CultureInfo.InvariantCulture),
            3 => row.SpeedMetersPerSecond?.ToString("F2", CultureInfo.InvariantCulture),
            4 => row.HeartRateBpm?.ToString(CultureInfo.InvariantCulture),
            5 => row.CadenceRpm?.ToString("F0", CultureInfo.InvariantCulture),
            6 => row.PowerWatts?.ToString(CultureInfo.InvariantCulture),
            7 => row.AltitudeMeters?.ToString("F2", CultureInfo.InvariantCulture),
            8 => row.TemperatureCelsius?.ToString("F1", CultureInfo.InvariantCulture),
            9 => row.Position?.LatitudeDegrees.ToString("F6", CultureInfo.InvariantCulture),
            10 => row.Position?.LongitudeDegrees.ToString("F6", CultureInfo.InvariantCulture),
            _ => null,
        };

        if (value is null)
            return string.Empty;

        // RFC 4180: quote if contains comma, double-quote, or newline
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        return value;
    }
}
