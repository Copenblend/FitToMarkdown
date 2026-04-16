using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Summaries;

namespace FitToMarkdown.Markdown.Projection;

internal static class FrontmatterProjector
{
    internal static FitFrontmatter Project(FitFileDocument document)
    {
        var fileId = document.FileId;
        var sessions = document.ActivityContent?.Sessions;
        var firstSession = sessions is { Count: > 0 } ? sessions[0] : null;
        var metrics = firstSession?.Metrics;

        FitSport? sport = firstSession?.Sport
            ?? document.Course?.Sport
            ?? document.Workout?.Sport;

        FitSubSport? subSport = firstSession?.SubSport
            ?? document.Workout?.SubSport;

        int lapCount = 0;
        int recordCount = 0;

        if (sessions is not null)
        {
            foreach (var session in sessions)
            {
                lapCount += session.Laps.Count;
                recordCount += session.Records.Count;
            }
        }

        return new FitFrontmatter
        {
            FileType = fileId?.FileType,
            ManufacturerName = fileId?.ManufacturerName,
            ProductName = fileId?.ProductName,
            SerialNumber = fileId?.SerialNumber,
            TimeCreatedUtc = fileId?.TimeCreatedUtc,
            Sport = sport,
            SubSport = subSport,
            DistanceMeters = metrics?.TotalDistanceMeters,
            DurationSeconds = metrics?.TotalTimerTime?.TotalSeconds,
            AverageHeartRateBpm = metrics?.AverageHeartRateBpm,
            MaximumHeartRateBpm = metrics?.MaximumHeartRateBpm,
            AverageSpeedMetersPerSecond = metrics?.EnhancedAverageSpeedMetersPerSecond ?? metrics?.AverageSpeedMetersPerSecond,
            AveragePowerWatts = metrics?.AveragePowerWatts,
            TotalAscentMeters = metrics?.TotalAscentMeters,
            TotalDescentMeters = metrics?.TotalDescentMeters,
            TotalCalories = metrics?.TotalCalories,
            SessionCount = sessions?.Count ?? 0,
            LapCount = lapCount,
            RecordCount = recordCount,
            PoolLengthMeters = metrics?.PoolLengthMeters,
        };
    }
}
