using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.Parsing;
using FitToMarkdown.Core.ValueObjects;
using FitToMarkdown.Fit.Internal;
using FitToMarkdown.Fit.Mapping;

namespace FitToMarkdown.Fit.Recovery;

/// <summary>
/// Result of synthetic recovery for truncated activity files.
/// </summary>
internal sealed class FitSyntheticRecoveryResult
{
    public FitActivity? Activity { get; init; }
    public IReadOnlyList<FitSession> Sessions { get; init; } = [];
    public IReadOnlyList<FitLap> Laps { get; init; } = [];
    public IReadOnlyList<FitParseIssue> Issues { get; init; } = [];
    public bool UsedSyntheticActivity { get; init; }
    public bool UsedSyntheticSessions { get; init; }
}

/// <summary>
/// Synthesizes missing summary messages (Activity, Session, Lap) for truncated activity files.
/// </summary>
internal sealed class FitSyntheticRecoveryService
{
    /// <summary>
    /// Attempts to recover a truncated activity by synthesizing missing summary messages.
    /// </summary>
    public FitSyntheticRecoveryResult Recover(
        FitActivity? activity,
        IReadOnlyList<FitSession> sessions,
        IReadOnlyList<FitLap> laps,
        IReadOnlyList<FitRecord> records,
        IReadOnlyList<FitEvent> events,
        FitFileId? fileId)
    {
        if (records.Count == 0)
        {
            return new FitSyntheticRecoveryResult
            {
                Activity = activity,
                Sessions = sessions,
                Laps = laps,
            };
        }

        var issues = new List<FitParseIssue>();
        bool usedSyntheticActivity = false;
        bool usedSyntheticSessions = false;

        var firstTimestamp = records
            .Where(r => r.TimestampUtc.HasValue)
            .Select(r => r.TimestampUtc!.Value)
            .DefaultIfEmpty()
            .Min();

        var lastTimestamp = records
            .Where(r => r.TimestampUtc.HasValue)
            .Select(r => r.TimestampUtc!.Value)
            .DefaultIfEmpty()
            .Max();

        var timeSpan = firstTimestamp != default && lastTimestamp != default
            ? lastTimestamp - firstTimestamp
            : TimeSpan.Zero;

        // Recover missing Activity
        if (activity is null)
        {
            activity = new FitActivity
            {
                Message = FitMessageIdentityFactory.Create(parseSequence: -1, isSynthetic: true),
                TimestampUtc = firstTimestamp != default ? firstTimestamp : null,
                TotalTimerTime = timeSpan > TimeSpan.Zero ? timeSpan : null,
            };
            issues.Add(FitParseIssueFactory.SyntheticActivityCreated());
            usedSyntheticActivity = true;
        }

        // Recover missing Sessions
        if (sessions.Count == 0)
        {
            var lastDistance = records
                .Where(r => r.DistanceMeters.HasValue)
                .Select(r => r.DistanceMeters!.Value)
                .DefaultIfEmpty()
                .Max();

            var lastCalories = records
                .Where(r => r.Calories.HasValue)
                .Select(r => r.Calories!.Value)
                .DefaultIfEmpty()
                .Max();

            // Try to derive sport from FileId if available
            FitSport? sport = null;
            if (fileId?.FileType == FitFileType.Activity)
            {
                // FileId does not carry sport; leave null
            }

            sessions =
            [
                new FitSession
                {
                    Message = FitMessageIdentityFactory.Create(parseSequence: -2, isSynthetic: true),
                    SessionIndex = 0,
                    Range = new FitTimeRange
                    {
                        StartTimeUtc = firstTimestamp != default ? firstTimestamp : null,
                        EndTimeUtc = lastTimestamp != default ? lastTimestamp : null,
                        TotalElapsedTime = timeSpan > TimeSpan.Zero ? timeSpan : null,
                        TotalTimerTime = timeSpan > TimeSpan.Zero ? timeSpan : null,
                    },
                    Sport = sport,
                    Metrics = new FitSummaryMetrics
                    {
                        TotalDistanceMeters = lastDistance > 0 ? lastDistance : null,
                        TotalCalories = lastCalories > 0 ? lastCalories : null,
                        TotalElapsedTime = timeSpan > TimeSpan.Zero ? timeSpan : null,
                        TotalTimerTime = timeSpan > TimeSpan.Zero ? timeSpan : null,
                    },
                },
            ];
            issues.Add(FitParseIssueFactory.SyntheticSessionCreated());
            usedSyntheticSessions = true;
        }

        // Recover missing Laps
        if (laps.Count == 0)
        {
            laps =
            [
                new FitLap
                {
                    Message = FitMessageIdentityFactory.Create(parseSequence: -3, isSynthetic: true),
                    LapIndex = 0,
                    ParentSessionIndex = 0,
                    Range = new FitTimeRange
                    {
                        StartTimeUtc = firstTimestamp != default ? firstTimestamp : null,
                        EndTimeUtc = lastTimestamp != default ? lastTimestamp : null,
                        TotalElapsedTime = timeSpan > TimeSpan.Zero ? timeSpan : null,
                        TotalTimerTime = timeSpan > TimeSpan.Zero ? timeSpan : null,
                    },
                },
            ];
            issues.Add(FitParseIssueFactory.SyntheticLapCreated());
        }

        return new FitSyntheticRecoveryResult
        {
            Activity = activity,
            Sessions = sessions,
            Laps = laps,
            Issues = issues,
            UsedSyntheticActivity = usedSyntheticActivity,
            UsedSyntheticSessions = usedSyntheticSessions,
        };
    }
}
