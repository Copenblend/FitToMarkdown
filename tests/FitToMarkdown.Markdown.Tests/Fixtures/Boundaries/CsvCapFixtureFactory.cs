using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Markdown.Tests.Fixtures.Boundaries;

internal static class CsvCapFixtureFactory
{
    private static readonly DateTimeOffset BaseTimestamp =
        new(2024, 6, 15, 8, 0, 0, TimeSpan.Zero);

    internal static FitFileDocument CreateWith50SampledRows() => CreateWithRecordCount(50);

    internal static FitFileDocument CreateWith61SampledRows() => CreateWithRecordCount(61);

    internal static FitFileDocument CreateWith30SampledRows() => CreateWithRecordCount(30);

    private static FitFileDocument CreateWithRecordCount(int recordCount)
    {
        var records = new List<FitRecord>();
        for (int i = 0; i < recordCount; i++)
        {
            records.Add(new FitRecord
            {
                RecordIndex = i,
                SessionIndex = 0,
                TimestampUtc = BaseTimestamp.AddSeconds(i * 5),
                DistanceMeters = i * 50.0,
                EnhancedSpeedMetersPerSecond = 3.0 + (i * 0.01),
                HeartRateBpm = (byte)(120 + (i % 40)),
                CadenceRpm = 80 + (i % 10),
                AltitudeMeters = 100.0 + (i * 0.5),
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
                            EndTimeUtc = BaseTimestamp.AddSeconds(recordCount * 5),
                            TotalTimerTime = TimeSpan.FromSeconds(recordCount * 5),
                        },
                        Metrics = new FitSummaryMetrics
                        {
                            TotalDistanceMeters = recordCount * 50.0,
                            TotalTimerTime = TimeSpan.FromSeconds(recordCount * 5),
                        },
                        Records = records,
                    },
                ],
            },
        };
    }
}
