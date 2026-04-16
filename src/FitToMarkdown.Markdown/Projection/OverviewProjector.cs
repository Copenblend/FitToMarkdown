using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.Summaries;

namespace FitToMarkdown.Markdown.Projection;

internal static class OverviewProjector
{
    internal static (string Title, DateTimeOffset? HeadingTimestamp, FitOverviewMetrics Overview) Project(FitFileDocument document)
    {
        var sessions = document.ActivityContent?.Sessions ?? [];
        var firstSession = sessions.Count > 0 ? sessions[0] : null;

        string title = BuildTitle(document, firstSession);
        DateTimeOffset? headingTimestamp = firstSession?.Range.StartTimeUtc ?? document.FileId?.TimeCreatedUtc;
        FitOverviewMetrics overview = AggregateOverview(document, sessions, firstSession);

        return (title, headingTimestamp, overview);
    }

    private static string BuildTitle(FitFileDocument document, FitSession? firstSession)
    {
        if (document.Course is not null)
        {
            string courseName = document.Course.CourseName ?? "Unnamed";
            return $"Course: {courseName}";
        }

        if (document.Workout is not null)
        {
            string workoutName = document.Workout.WorkoutName ?? "Unnamed";
            return $"Workout: {workoutName}";
        }

        if (firstSession is null)
            return "FIT Activity";

        FitSport? sport = firstSession.Sport;
        FitSubSport? subSport = firstSession.SubSport;

        if (sport is null)
            return "FIT Activity";

        string sportName = FormatEnumName(sport.Value.ToString());

        if (subSport is null or FitSubSport.Generic)
            return $"{sportName} Activity";

        string subSportName = FormatEnumName(subSport.Value.ToString());
        return $"{sportName} — {subSportName} Activity";
    }

    private static FitOverviewMetrics AggregateOverview(
        FitFileDocument document,
        IReadOnlyList<FitSession> sessions,
        FitSession? firstSession)
    {
        if (sessions.Count == 0)
        {
            return new FitOverviewMetrics
            {
                PrimarySport = document.Course?.Sport ?? document.Workout?.Sport,
                PrimarySubSport = document.Workout?.SubSport,
            };
        }

        double totalDistance = 0;
        double totalElapsedSeconds = 0;
        double totalTimerSeconds = 0;
        int totalCalories = 0;
        int lapCount = 0;
        int lengthCount = 0;
        int recordCount = 0;

        double hrWeightedSum = 0;
        double hrWeightSeconds = 0;
        byte maxHr = 0;

        ushort totalAscent = 0;
        ushort totalDescent = 0;

        foreach (var session in sessions)
        {
            var m = session.Metrics;

            if (m.TotalDistanceMeters.HasValue)
                totalDistance += m.TotalDistanceMeters.Value;

            if (m.TotalElapsedTime.HasValue)
                totalElapsedSeconds += m.TotalElapsedTime.Value.TotalSeconds;

            if (m.TotalTimerTime.HasValue)
                totalTimerSeconds += m.TotalTimerTime.Value.TotalSeconds;

            if (m.TotalCalories.HasValue)
                totalCalories += m.TotalCalories.Value;

            if (m.AverageHeartRateBpm.HasValue && m.TotalTimerTime.HasValue)
            {
                double secs = m.TotalTimerTime.Value.TotalSeconds;
                hrWeightedSum += m.AverageHeartRateBpm.Value * secs;
                hrWeightSeconds += secs;
            }

            if (m.MaximumHeartRateBpm.HasValue && m.MaximumHeartRateBpm.Value > maxHr)
                maxHr = m.MaximumHeartRateBpm.Value;

            if (m.TotalAscentMeters.HasValue)
                totalAscent += m.TotalAscentMeters.Value;

            if (m.TotalDescentMeters.HasValue)
                totalDescent += m.TotalDescentMeters.Value;

            lapCount += session.Laps.Count;
            lengthCount += session.Lengths.Count;
            recordCount += session.Records.Count;
        }

        bool hasDistance = sessions.Any(s => s.Metrics.TotalDistanceMeters.HasValue);
        bool hasElapsed = sessions.Any(s => s.Metrics.TotalElapsedTime.HasValue);
        bool hasTimer = sessions.Any(s => s.Metrics.TotalTimerTime.HasValue);
        bool hasCalories = sessions.Any(s => s.Metrics.TotalCalories.HasValue);
        bool hasMaxHr = sessions.Any(s => s.Metrics.MaximumHeartRateBpm.HasValue);
        bool hasAscent = sessions.Any(s => s.Metrics.TotalAscentMeters.HasValue);
        bool hasDescent = sessions.Any(s => s.Metrics.TotalDescentMeters.HasValue);

        byte? avgHr = hrWeightSeconds > 0
            ? (byte)Math.Round(hrWeightedSum / hrWeightSeconds)
            : null;

        double? avgSpeed = hasDistance && hasTimer && totalTimerSeconds > 0
            ? totalDistance / totalTimerSeconds
            : null;

        double? avgPace = avgSpeed is > 0
            ? 1000.0 / avgSpeed.Value
            : null;

        return new FitOverviewMetrics
        {
            PrimarySport = firstSession?.Sport,
            PrimarySubSport = firstSession?.SubSport,
            IsMultiSport = document.ActivityContent?.IsMultiSport ?? false,
            StartTimeUtc = firstSession?.Range.StartTimeUtc,
            TotalElapsedTime = hasElapsed ? TimeSpan.FromSeconds(totalElapsedSeconds) : null,
            TotalTimerTime = hasTimer ? TimeSpan.FromSeconds(totalTimerSeconds) : null,
            TotalDistanceMeters = hasDistance ? totalDistance : null,
            Calories = hasCalories ? ClampToUShort(totalCalories) : null,
            AverageHeartRateBpm = avgHr,
            MaximumHeartRateBpm = hasMaxHr ? maxHr : null,
            AverageSpeedMetersPerSecond = avgSpeed,
            AveragePaceSecondsPerKilometer = avgPace,
            TotalAscentMeters = hasAscent ? totalAscent : null,
            TotalDescentMeters = hasDescent ? totalDescent : null,
            SessionCount = sessions.Count,
            LapCount = lapCount,
            LengthCount = lengthCount,
            RecordCount = recordCount,
        };
    }

    private static ushort ClampToUShort(int value)
    {
        return (ushort)Math.Clamp(value, ushort.MinValue, ushort.MaxValue);
    }

    private static string FormatEnumName(string enumValue)
    {
        // Insert spaces before capital letters in PascalCase names (e.g. "CrossCountrySkiing" → "Cross Country Skiing")
        var sb = new System.Text.StringBuilder(enumValue.Length + 4);
        for (int i = 0; i < enumValue.Length; i++)
        {
            char c = enumValue[i];
            if (i > 0 && char.IsUpper(c) && !char.IsUpper(enumValue[i - 1]))
                sb.Append(' ');
            sb.Append(c);
        }
        return sb.ToString();
    }
}
