using FitToMarkdown.Core.Models;

namespace FitToMarkdown.Fit.Grouping;

/// <summary>
/// Result of grouping flat message lists into session-centric structures.
/// </summary>
internal sealed class FitActivityGroupingResult
{
    public IReadOnlyList<FitSession> GroupedSessions { get; init; } = [];
}

/// <summary>
/// Groups decoded messages into session-centric activity structures including multi-sport handling.
/// </summary>
internal sealed class FitActivityGroupingService
{
    /// <summary>
    /// Groups flat lists of mapped models into session-centric structures.
    /// </summary>
    public FitActivityGroupingResult Group(
        IReadOnlyList<FitSession> sessions,
        IReadOnlyList<FitLap> laps,
        IReadOnlyList<FitRecord> records,
        IReadOnlyList<FitLength> lengths,
        IReadOnlyList<FitEvent> events)
    {
        if (sessions.Count == 0)
        {
            return new FitActivityGroupingResult { GroupedSessions = [] };
        }

        // Single session optimization: assign everything to it
        if (sessions.Count == 1)
        {
            var session = sessions[0];
            var groupedSession = session with
            {
                Laps = AssignParentSessionToLaps(laps, session.SessionIndex),
                Records = AssignSessionToRecords(records, session.SessionIndex),
                Lengths = AssignSessionToLengths(lengths, session.SessionIndex),
            };
            return new FitActivityGroupingResult { GroupedSessions = [groupedSession] };
        }

        // Multi-session: group laps, records, lengths, events by session
        var lapsBySession = GroupLapsBySession(sessions, laps);
        var recordsBySession = GroupByTimestamp(sessions, records, static r => r.TimestampUtc);
        var lengthsBySession = GroupByTimestamp(sessions, lengths, static l => l.Range.StartTimeUtc);
        var eventsBySession = GroupByTimestamp(sessions, events, static e => e.TimestampUtc);

        var groupedSessions = new List<FitSession>(sessions.Count);
        for (int i = 0; i < sessions.Count; i++)
        {
            var session = sessions[i];
            var sessionLaps = lapsBySession.TryGetValue(i, out var sl) ? sl : [];
            var sessionRecords = recordsBySession.TryGetValue(i, out var sr) ? sr : [];
            var sessionLengths = lengthsBySession.TryGetValue(i, out var slen) ? slen : [];

            groupedSessions.Add(session with
            {
                Laps = AssignParentSessionToLaps(sessionLaps, session.SessionIndex),
                Records = AssignSessionToRecords(sessionRecords, session.SessionIndex),
                Lengths = AssignSessionToLengths(sessionLengths, session.SessionIndex),
            });
        }

        return new FitActivityGroupingResult { GroupedSessions = groupedSessions };
    }

    private static Dictionary<int, List<FitLap>> GroupLapsBySession(
        IReadOnlyList<FitSession> sessions,
        IReadOnlyList<FitLap> laps)
    {
        var result = new Dictionary<int, List<FitLap>>();

        // Try index-based assignment first
        bool useIndexBased = sessions.All(s =>
            s.Metrics.FirstLapIndex.HasValue && s.Metrics.NumberOfLaps.HasValue);

        if (useIndexBased)
        {
            for (int i = 0; i < sessions.Count; i++)
            {
                var session = sessions[i];
                int firstLap = session.Metrics.FirstLapIndex!.Value;
                int numLaps = session.Metrics.NumberOfLaps!.Value;

                var sessionLaps = new List<FitLap>();
                for (int j = firstLap; j < firstLap + numLaps && j < laps.Count; j++)
                {
                    sessionLaps.Add(laps[j]);
                }
                result[i] = sessionLaps;
            }
            return result;
        }

        // Fallback: timestamp-based assignment
        foreach (var lap in laps)
        {
            int sessionIdx = FindSessionForTimestamp(sessions, lap.Range.StartTimeUtc);
            if (sessionIdx >= 0)
            {
                if (!result.TryGetValue(sessionIdx, out var list))
                {
                    list = [];
                    result[sessionIdx] = list;
                }
                list.Add(lap);
            }
        }

        return result;
    }

    private static Dictionary<int, List<T>> GroupByTimestamp<T>(
        IReadOnlyList<FitSession> sessions,
        IReadOnlyList<T> items,
        Func<T, DateTimeOffset?> timestampSelector)
    {
        var result = new Dictionary<int, List<T>>();

        foreach (var item in items)
        {
            int sessionIdx = FindSessionForTimestamp(sessions, timestampSelector(item));
            if (sessionIdx >= 0)
            {
                if (!result.TryGetValue(sessionIdx, out var list))
                {
                    list = [];
                    result[sessionIdx] = list;
                }
                list.Add(item);
            }
        }

        return result;
    }

    private static int FindSessionForTimestamp(IReadOnlyList<FitSession> sessions, DateTimeOffset? timestamp)
    {
        if (timestamp is null)
            return sessions.Count > 0 ? 0 : -1;

        for (int i = 0; i < sessions.Count; i++)
        {
            var session = sessions[i];
            var start = session.Range.StartTimeUtc;
            var end = session.Range.EndTimeUtc;

            if (start.HasValue && end.HasValue &&
                timestamp.Value >= start.Value && timestamp.Value <= end.Value)
            {
                return i;
            }
        }

        // If no exact match, assign to the last session as a best-effort
        return sessions.Count > 0 ? sessions.Count - 1 : -1;
    }

    private static IReadOnlyList<FitLap> AssignParentSessionToLaps(IReadOnlyList<FitLap> laps, int sessionIndex)
    {
        var result = new List<FitLap>(laps.Count);
        foreach (var lap in laps)
        {
            result.Add(lap with { ParentSessionIndex = sessionIndex });
        }
        return result;
    }

    private static IReadOnlyList<FitRecord> AssignSessionToRecords(IReadOnlyList<FitRecord> records, int sessionIndex)
    {
        var result = new List<FitRecord>(records.Count);
        foreach (var record in records)
        {
            result.Add(record with { SessionIndex = sessionIndex });
        }
        return result;
    }

    private static IReadOnlyList<FitLength> AssignSessionToLengths(IReadOnlyList<FitLength> lengths, int sessionIndex)
    {
        var result = new List<FitLength>(lengths.Count);
        foreach (var length in lengths)
        {
            result.Add(length with { SessionIndex = sessionIndex });
        }
        return result;
    }
}
