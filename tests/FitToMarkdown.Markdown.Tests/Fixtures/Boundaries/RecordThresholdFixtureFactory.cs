using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Markdown.Tests.Fixtures.Boundaries;

internal static class RecordThresholdFixtureFactory
{
    private static readonly DateTimeOffset BaseTimestamp =
        new(2024, 6, 15, 8, 0, 0, TimeSpan.Zero);

    internal static FitFileDocument CreateWithExactly10Records() => CreateWithRecordCount(10);

    internal static FitFileDocument CreateWith11Records() => CreateWithRecordCount(11);

    internal static FitFileDocument CreateWith5Records() => CreateWithRecordCount(5);

    private static FitFileDocument CreateWithRecordCount(int recordCount)
    {
        var records = new List<FitRecord>();
        for (int i = 0; i < recordCount; i++)
        {
            records.Add(new FitRecord
            {
                RecordIndex = i,
                SessionIndex = 0,
                TimestampUtc = BaseTimestamp.AddSeconds(i * 10),
                DistanceMeters = i * 100.0,
                EnhancedSpeedMetersPerSecond = 3.5 + (i * 0.02),
                HeartRateBpm = (byte)(130 + i),
                CadenceRpm = 85,
                AltitudeMeters = 100.0 + i,
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
                            EndTimeUtc = BaseTimestamp.AddSeconds(recordCount * 10),
                            TotalTimerTime = TimeSpan.FromSeconds(recordCount * 10),
                        },
                        Metrics = new FitSummaryMetrics
                        {
                            TotalDistanceMeters = recordCount * 100.0,
                            TotalTimerTime = TimeSpan.FromSeconds(recordCount * 10),
                        },
                        Records = records,
                    },
                ],
            },
        };
    }
}
