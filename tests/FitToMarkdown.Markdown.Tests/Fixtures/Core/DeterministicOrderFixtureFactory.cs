using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Markdown.Tests.Fixtures.Core;

internal static class DeterministicOrderFixtureFactory
{
    private static readonly DateTimeOffset BaseTimestamp =
        new(2024, 6, 15, 8, 0, 0, TimeSpan.Zero);

    internal static FitFileDocument CreateShuffledSessions()
    {
        return new FitFileDocument
        {
            FileId = new FitFileId
            {
                FileType = FitFileType.Activity,
                ManufacturerName = "Garmin",
                ProductName = "Forerunner 955",
                TimeCreatedUtc = BaseTimestamp,
            },
            ActivityContent = new FitActivityContent
            {
                IsMultiSport = true,
                Sessions =
                [
                    new FitSession
                    {
                        SessionIndex = 2,
                        Sport = FitSport.Running,
                        SubSport = FitSubSport.Street,
                        Range = new FitTimeRange
                        {
                            StartTimeUtc = BaseTimestamp.AddMinutes(100),
                            EndTimeUtc = BaseTimestamp.AddMinutes(140),
                            TotalTimerTime = TimeSpan.FromMinutes(38),
                        },
                        Metrics = new FitSummaryMetrics
                        {
                            TotalDistanceMeters = 10000,
                            TotalTimerTime = TimeSpan.FromMinutes(38),
                        },
                    },
                    new FitSession
                    {
                        SessionIndex = 0,
                        Sport = FitSport.Swimming,
                        SubSport = FitSubSport.OpenWater,
                        Range = new FitTimeRange
                        {
                            StartTimeUtc = BaseTimestamp,
                            EndTimeUtc = BaseTimestamp.AddMinutes(30),
                            TotalTimerTime = TimeSpan.FromMinutes(28),
                        },
                        Metrics = new FitSummaryMetrics
                        {
                            TotalDistanceMeters = 1500,
                            TotalTimerTime = TimeSpan.FromMinutes(28),
                        },
                    },
                    new FitSession
                    {
                        SessionIndex = 1,
                        Sport = FitSport.Cycling,
                        SubSport = FitSubSport.Road,
                        Range = new FitTimeRange
                        {
                            StartTimeUtc = BaseTimestamp.AddMinutes(35),
                            EndTimeUtc = BaseTimestamp.AddMinutes(95),
                            TotalTimerTime = TimeSpan.FromMinutes(58),
                        },
                        Metrics = new FitSummaryMetrics
                        {
                            TotalDistanceMeters = 40000,
                            TotalTimerTime = TimeSpan.FromMinutes(58),
                        },
                    },
                ],
            },
        };
    }

    internal static FitFileDocument CreateShuffledRecords()
    {
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
                        Range = new FitTimeRange
                        {
                            StartTimeUtc = BaseTimestamp,
                            EndTimeUtc = BaseTimestamp.AddMinutes(5),
                            TotalTimerTime = TimeSpan.FromMinutes(5),
                        },
                        Metrics = new FitSummaryMetrics
                        {
                            TotalDistanceMeters = 1000,
                            TotalTimerTime = TimeSpan.FromMinutes(5),
                        },
                        Records =
                        [
                            new FitRecord
                            {
                                RecordIndex = 2,
                                SessionIndex = 0,
                                TimestampUtc = BaseTimestamp.AddSeconds(120),
                                HeartRateBpm = 155,
                            },
                            new FitRecord
                            {
                                RecordIndex = 0,
                                SessionIndex = 0,
                                TimestampUtc = BaseTimestamp,
                                HeartRateBpm = 140,
                            },
                            new FitRecord
                            {
                                RecordIndex = 1,
                                SessionIndex = 0,
                                TimestampUtc = BaseTimestamp.AddSeconds(60),
                                HeartRateBpm = 148,
                            },
                        ],
                    },
                ],
            },
        };
    }
}
