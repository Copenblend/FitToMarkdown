using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.Summaries;

namespace FitToMarkdown.Markdown.Projection;

internal static class TimeSeriesSampler
{
    internal static IReadOnlyList<SampledTimeSeriesRow> SampleGlobal(FitFileDocument document, MarkdownDocumentOptions options)
    {
        if (document.ActivityContent is null)
            return [];

        var allRecords = document.ActivityContent.Sessions
            .SelectMany(s => s.Records)
            .ToList();

        if (allRecords.Count == 0 || options.MaximumRecordSampleRows <= 0 || !options.IncludeRecordSamples)
            return [];

        var indices = ComputeSampleIndices(allRecords.Count, options.MaximumRecordSampleRows);
        var rows = new List<SampledTimeSeriesRow>(indices.Count);

        foreach (var idx in indices)
        {
            var record = allRecords[idx];
            rows.Add(MapRecord(record, sessionIndex: record.SessionIndex ?? 0, lapIndex: null, sessionStart: null));
        }

        return rows;
    }

    internal static IReadOnlyList<SampledTimeSeriesRow> SampleForSession(FitSession session, MarkdownDocumentOptions options)
    {
        if (session.Records.Count == 0 || options.MaximumRecordSampleRows <= 0 || !options.IncludeRecordSamples)
            return [];

        var indices = ComputeSampleIndices(session.Records.Count, options.MaximumRecordSampleRows);
        var rows = new List<SampledTimeSeriesRow>(indices.Count);
        var sessionStart = session.Range.StartTimeUtc;

        foreach (var idx in indices)
        {
            var record = session.Records[idx];
            int? lapIndex = FindLapIndex(session.Laps, record.TimestampUtc);

            rows.Add(MapRecord(record, session.SessionIndex, lapIndex, sessionStart));
        }

        return rows;
    }

    private static List<int> ComputeSampleIndices(int totalCount, int maxRows)
    {
        if (totalCount <= maxRows)
            return Enumerable.Range(0, totalCount).ToList();

        var indices = new List<int>(maxRows) { 0 };

        for (int i = 1; i < maxRows - 1; i++)
        {
            int idx = (int)Math.Round((double)i * (totalCount - 1) / (maxRows - 1));
            indices.Add(idx);
        }

        indices.Add(totalCount - 1);

        return indices;
    }

    private static int? FindLapIndex(IReadOnlyList<FitLap> laps, DateTimeOffset? recordTimestamp)
    {
        if (recordTimestamp is null || laps.Count == 0)
            return null;

        for (int i = laps.Count - 1; i >= 0; i--)
        {
            var lapStart = laps[i].Range.StartTimeUtc;
            if (lapStart is not null && recordTimestamp >= lapStart)
                return i;
        }

        return 0;
    }

    private static SampledTimeSeriesRow MapRecord(FitRecord record, int sessionIndex, int? lapIndex, DateTimeOffset? sessionStart)
    {
        TimeSpan? offset = record.TimestampUtc is not null && sessionStart is not null
            ? record.TimestampUtc.Value - sessionStart.Value
            : null;

        return new SampledTimeSeriesRow
        {
            SourceRecordIndex = record.RecordIndex,
            SessionIndex = sessionIndex,
            LapIndex = lapIndex,
            TimestampUtc = record.TimestampUtc,
            OffsetFromSessionStart = offset,
            Position = record.Position,
            DistanceMeters = record.DistanceMeters,
            SpeedMetersPerSecond = record.EnhancedSpeedMetersPerSecond ?? record.SpeedMetersPerSecond,
            HeartRateBpm = record.HeartRateBpm,
            CadenceRpm = record.CadenceRpm,
            PowerWatts = record.PowerWatts,
            AltitudeMeters = record.EnhancedAltitudeMeters ?? record.AltitudeMeters,
            TemperatureCelsius = record.TemperatureCelsius,
            RespirationRateBreathsPerMinute = record.RespirationRateBreathsPerMinute,
            DepthMeters = record.EnhancedDepthMeters ?? record.DepthMeters,
            DeveloperFields = record.DeveloperFields,
        };
    }
}
