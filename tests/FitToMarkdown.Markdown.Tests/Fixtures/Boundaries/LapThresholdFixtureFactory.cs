using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Markdown.Tests.Fixtures.Boundaries;

internal static class LapThresholdFixtureFactory
{
    private static readonly DateTimeOffset BaseTimestamp =
        new(2024, 6, 15, 8, 0, 0, TimeSpan.Zero);

    internal static FitFileDocument CreateWithExactly10Laps() => CreateWithLapCount(10);

    internal static FitFileDocument CreateWith11Laps() => CreateWithLapCount(11);

    internal static FitFileDocument CreateWith9Laps() => CreateWithLapCount(9);

    private static FitFileDocument CreateWithLapCount(int lapCount)
    {
        var laps = new List<FitLap>();
        for (int i = 0; i < lapCount; i++)
        {
            laps.Add(new FitLap
            {
                LapIndex = i,
                ParentSessionIndex = 0,
                Range = new FitTimeRange
                {
                    StartTimeUtc = BaseTimestamp.AddMinutes(i * 5),
                    EndTimeUtc = BaseTimestamp.AddMinutes((i + 1) * 5),
                },
                Metrics = new FitSummaryMetrics
                {
                    TotalDistanceMeters = 1000,
                    TotalTimerTime = TimeSpan.FromMinutes(5),
                    AverageHeartRateBpm = (byte)(140 + i),
                    EnhancedAverageSpeedMetersPerSecond = 3.33,
                },
            });
        }

        return new FitFileDocument
        {
            FileId = new FitFileId
            {
                FileType = FitFileType.Activity,
                ManufacturerName = "Garmin",
                ProductName = "Forerunner 265",
                TimeCreatedUtc = BaseTimestamp,
            },
            ActivityContent = new FitActivityContent
            {
                Sessions =
                [
                    new FitSession
                    {
                        SessionIndex = 0,
                        Sport = FitSport.Running,
                        SubSport = FitSubSport.Street,
                        Range = new FitTimeRange
                        {
                            StartTimeUtc = BaseTimestamp,
                            EndTimeUtc = BaseTimestamp.AddMinutes(lapCount * 5),
                            TotalTimerTime = TimeSpan.FromMinutes(lapCount * 5),
                        },
                        Metrics = new FitSummaryMetrics
                        {
                            TotalDistanceMeters = lapCount * 1000.0,
                            TotalTimerTime = TimeSpan.FromMinutes(lapCount * 5),
                            NumberOfLaps = (ushort)lapCount,
                        },
                        Laps = laps,
                    },
                ],
            },
        };
    }
}
