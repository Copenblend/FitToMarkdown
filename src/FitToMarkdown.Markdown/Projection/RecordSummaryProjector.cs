using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.Summaries;

namespace FitToMarkdown.Markdown.Projection;

internal static class RecordSummaryProjector
{
    internal static RecordStatisticsSummary? ProjectGlobal(FitFileDocument document)
    {
        if (document.ActivityContent is null)
            return null;

        var allRecords = document.ActivityContent.Sessions
            .SelectMany(s => s.Records)
            .ToList();

        return ProjectFromRecords(allRecords);
    }

    internal static RecordStatisticsSummary? ProjectForSession(FitSession session)
    {
        return ProjectFromRecords(session.Records);
    }

    internal static RecordStatisticsSummary? ProjectFromRecords(IReadOnlyList<FitRecord> records)
    {
        if (records.Count == 0)
            return null;

        var firstTimestamp = records.FirstOrDefault(r => r.TimestampUtc is not null)?.TimestampUtc;
        var lastTimestamp = records.LastOrDefault(r => r.TimestampUtc is not null)?.TimestampUtc;

        TimeSpan? duration = firstTimestamp is not null && lastTimestamp is not null
            ? lastTimestamp.Value - firstTimestamp.Value
            : null;

        var metrics = new List<MetricStatistics>();

        AppendMetric(metrics, "heart_rate", "Heart Rate", "bpm",
            records, r => r.HeartRateBpm.HasValue ? (double)r.HeartRateBpm.Value : null);

        AppendMetric(metrics, "speed", "Speed", "m/s",
            records, r => r.EnhancedSpeedMetersPerSecond ?? r.SpeedMetersPerSecond);

        AppendMetric(metrics, "altitude", "Altitude", "m",
            records, r => r.EnhancedAltitudeMeters ?? r.AltitudeMeters);

        AppendMetric(metrics, "cadence", "Cadence", "rpm",
            records, r => r.CadenceRpm);

        AppendMetric(metrics, "power", "Power", "W",
            records, r => r.PowerWatts.HasValue ? (double)r.PowerWatts.Value : null);

        AppendMetric(metrics, "temperature", "Temperature", "°C",
            records, r => r.TemperatureCelsius);

        AppendMetric(metrics, "distance", "Distance", "m",
            records, r => r.DistanceMeters);

        AppendMetric(metrics, "respiration_rate", "Respiration Rate", "brpm",
            records, r => r.RespirationRateBreathsPerMinute);

        return new RecordStatisticsSummary
        {
            RecordCount = records.Count,
            FirstTimestampUtc = firstTimestamp,
            LastTimestampUtc = lastTimestamp,
            Duration = duration,
            Metrics = metrics,
        };
    }

    private static void AppendMetric(
        List<MetricStatistics> metrics,
        string key,
        string displayName,
        string unit,
        IReadOnlyList<FitRecord> records,
        Func<FitRecord, double?> selector)
    {
        var values = new List<double>();
        foreach (var record in records)
        {
            var v = selector(record);
            if (v.HasValue)
                values.Add(v.Value);
        }

        if (values.Count == 0)
            return;

        double min = values[0];
        double max = values[0];
        double sum = 0;

        foreach (var v in values)
        {
            if (v < min) min = v;
            if (v > max) max = v;
            sum += v;
        }

        double avg = sum / values.Count;

        double varianceSum = 0;
        foreach (var v in values)
        {
            double diff = v - avg;
            varianceSum += diff * diff;
        }

        double stdDev = Math.Sqrt(varianceSum / values.Count);

        metrics.Add(new MetricStatistics
        {
            MetricKey = key,
            DisplayName = displayName,
            UnitSymbol = unit,
            SampleCount = values.Count,
            Minimum = min,
            Average = avg,
            Maximum = max,
            StandardDeviation = stdDev,
        });
    }
}
