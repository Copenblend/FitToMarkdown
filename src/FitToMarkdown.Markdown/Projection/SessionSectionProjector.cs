using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.Summaries;

namespace FitToMarkdown.Markdown.Projection;

internal static class SessionSectionProjector
{
    internal static IReadOnlyList<SessionSection> Project(FitFileDocument document, MarkdownDocumentOptions options)
    {
        if (document.ActivityContent is null)
            return [];

        var sessions = document.ActivityContent.Sessions;
        var events = document.ActivityContent.Events;
        var result = new List<SessionSection>(sessions.Count);

        for (int i = 0; i < sessions.Count; i++)
        {
            var session = sessions[i];

            var lapRows = BuildLapRows(session);
            var lengthRows = BuildLengthRows(session);
            var recordSummary = RecordSummaryProjector.ProjectForSession(session);
            var recordSamples = TimeSeriesSampler.SampleForSession(session, options);
            var pauses = DerivePauses(events, session);

            var sectionBeforeDecisions = new SessionSection
            {
                Session = session,
                LapRows = lapRows,
                LengthRows = lengthRows,
                RecordSummary = recordSummary,
                RecordSamples = recordSamples,
                Pauses = pauses,
            };

            var sectionDecisions = SectionDecisionProjector.ProjectSessionLevel(options, sectionBeforeDecisions, i);

            result.Add(sectionBeforeDecisions with { SectionDecisions = sectionDecisions });
        }

        return result;
    }

    private static IReadOnlyList<LapTableRow> BuildLapRows(FitSession session)
    {
        var laps = session.Laps;
        if (laps.Count == 0)
            return [];

        var rows = new List<LapTableRow>(laps.Count);

        for (int i = 0; i < laps.Count; i++)
        {
            var lap = laps[i];
            var m = lap.Metrics;

            double? avgSpeed = m.EnhancedAverageSpeedMetersPerSecond ?? m.AverageSpeedMetersPerSecond;
            double? avgPace = avgSpeed is > 0 ? 1000.0 / avgSpeed.Value : null;

            rows.Add(new LapTableRow
            {
                LapNumber = i + 1,
                DistanceMeters = m.TotalDistanceMeters,
                Duration = m.TotalTimerTime,
                AverageSpeedMetersPerSecond = avgSpeed,
                AveragePaceSecondsPerKilometer = avgPace,
                AverageHeartRateBpm = m.AverageHeartRateBpm,
                MaximumHeartRateBpm = m.MaximumHeartRateBpm,
                AverageCadenceRpm = m.AverageCadenceRpm,
                AveragePowerWatts = m.AveragePowerWatts,
                Trigger = lap.LapTrigger,
            });
        }

        return rows;
    }

    private static IReadOnlyList<LengthTableRow> BuildLengthRows(FitSession session)
    {
        var lengths = session.Lengths;
        if (lengths.Count == 0)
            return [];

        var rows = new List<LengthTableRow>(lengths.Count);

        for (int i = 0; i < lengths.Count; i++)
        {
            var length = lengths[i];

            TimeSpan? duration = null;
            if (length.Range.StartTimeUtc is not null && length.Range.EndTimeUtc is not null)
                duration = length.Range.EndTimeUtc.Value - length.Range.StartTimeUtc.Value;

            ushort? swolf = null;
            if (duration is not null && length.TotalStrokes.HasValue)
            {
                int computed = (int)Math.Round(duration.Value.TotalSeconds) + length.TotalStrokes.Value;
                if (computed is >= 0 and <= ushort.MaxValue)
                    swolf = (ushort)computed;
            }

            double? pacePer100m = length.AverageSpeedMetersPerSecond is > 0
                ? 100.0 / length.AverageSpeedMetersPerSecond.Value
                : null;

            double? distancePerStroke = null;
            if (length.AverageSpeedMetersPerSecond.HasValue && duration is not null && length.TotalStrokes is > 0)
            {
                double totalDistance = length.AverageSpeedMetersPerSecond.Value * duration.Value.TotalSeconds;
                distancePerStroke = totalDistance / length.TotalStrokes.Value;
            }

            int? parentLapNumber = length.ParentLapIndex.HasValue
                ? length.ParentLapIndex.Value + 1
                : null;

            rows.Add(new LengthTableRow
            {
                LengthNumber = i + 1,
                LengthType = length.LengthType,
                SwimStroke = length.SwimStroke,
                Duration = duration,
                TotalStrokes = length.TotalStrokes,
                Swolf = swolf,
                PaceSecondsPer100Meters = pacePer100m,
                DistancePerStrokeMeters = distancePerStroke,
                ParentLapNumber = parentLapNumber,
            });
        }

        return rows;
    }

    private static IReadOnlyList<PauseInterval> DerivePauses(
        IReadOnlyList<FitEvent> events,
        FitSession session)
    {
        // Filter events relevant to this session's time range
        var sessionStart = session.Range.StartTimeUtc;
        var sessionEnd = session.Range.EndTimeUtc;

        var timerEvents = events
            .Where(e => e.Event == FitEventKind.Timer && e.TimestampUtc is not null)
            .Where(e =>
            {
                if (sessionStart is null && sessionEnd is null) return true;
                if (sessionStart is not null && e.TimestampUtc < sessionStart) return false;
                if (sessionEnd is not null && e.TimestampUtc > sessionEnd) return false;
                return true;
            })
            .OrderBy(e => e.TimestampUtc)
            .ToList();

        var pauses = new List<PauseInterval>();
        int order = 1;

        DateTimeOffset? stopTime = null;

        foreach (var evt in timerEvents)
        {
            if (evt.EventType is FitEventAction.Stop or FitEventAction.StopAll or FitEventAction.StopDisable or FitEventAction.StopDisableAll)
            {
                stopTime ??= evt.TimestampUtc;
            }
            else if (evt.EventType == FitEventAction.Start && stopTime is not null)
            {
                var duration = evt.TimestampUtc!.Value - stopTime.Value;
                if (duration > TimeSpan.Zero)
                {
                    pauses.Add(new PauseInterval
                    {
                        Order = order++,
                        StartUtc = stopTime.Value,
                        EndUtc = evt.TimestampUtc!.Value,
                        Duration = duration,
                    });
                }

                stopTime = null;
            }
        }

        return pauses;
    }
}
