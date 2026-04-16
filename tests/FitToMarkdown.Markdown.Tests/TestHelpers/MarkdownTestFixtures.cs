using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.Summaries;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Markdown.Tests.TestHelpers;

internal static class MarkdownTestFixtures
{
    private static readonly DateTimeOffset BaseTimestamp =
        new(2024, 6, 15, 8, 0, 0, TimeSpan.Zero);

    internal static FitFileDocument CreateMinimalActivityDocument()
    {
        return new FitFileDocument
        {
            FileId = new FitFileId
            {
                FileType = FitFileType.Activity,
                ManufacturerName = "Garmin",
                ProductName = "Forerunner 265",
                SerialNumber = 345678901,
                TimeCreatedUtc = BaseTimestamp,
            },
            ActivityContent = new FitActivityContent
            {
                Activity = new FitActivity
                {
                    TimestampUtc = BaseTimestamp.AddMinutes(30),
                    NumberOfSessions = 1,
                },
                Sessions =
                [
                    new FitSession
                    {
                        SessionIndex = 0,
                        Sport = FitSport.Running,
                        SubSport = FitSubSport.Generic,
                        Range = new FitTimeRange
                        {
                            StartTimeUtc = BaseTimestamp,
                            EndTimeUtc = BaseTimestamp.AddMinutes(30),
                            TotalElapsedTime = TimeSpan.FromMinutes(30),
                            TotalTimerTime = TimeSpan.FromMinutes(28),
                        },
                        Metrics = new FitSummaryMetrics
                        {
                            TotalDistanceMeters = 6200,
                            TotalElapsedTime = TimeSpan.FromMinutes(30),
                            TotalTimerTime = TimeSpan.FromMinutes(28),
                            TotalCalories = 280,
                            AverageHeartRateBpm = 152,
                            MaximumHeartRateBpm = 170,
                            EnhancedAverageSpeedMetersPerSecond = 3.69,
                        },
                        Laps = [],
                        Records = [],
                    },
                ],
            },
        };
    }

    internal static FitFileDocument CreateStandardRunningActivity()
    {
        var records = new List<FitRecord>();
        for (int i = 0; i < 5; i++)
        {
            records.Add(new FitRecord
            {
                RecordIndex = i,
                SessionIndex = 0,
                TimestampUtc = BaseTimestamp.AddSeconds(i * 60),
                DistanceMeters = i * 250.0,
                EnhancedSpeedMetersPerSecond = 3.7 + (i * 0.05),
                HeartRateBpm = (byte)(140 + i * 5),
                CadenceRpm = 88,
                AltitudeMeters = 100 + i * 2,
                TemperatureCelsius = 18.5,
            });
        }

        return new FitFileDocument
        {
            FileId = new FitFileId
            {
                FileType = FitFileType.Activity,
                ManufacturerName = "Garmin",
                ProductName = "Forerunner 265",
                SerialNumber = 345678901,
                TimeCreatedUtc = BaseTimestamp,
            },
            ActivityContent = new FitActivityContent
            {
                Activity = new FitActivity
                {
                    TimestampUtc = BaseTimestamp.AddMinutes(30),
                    NumberOfSessions = 1,
                },
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
                            EndTimeUtc = BaseTimestamp.AddMinutes(28),
                            TotalElapsedTime = TimeSpan.FromMinutes(30),
                            TotalTimerTime = TimeSpan.FromMinutes(28),
                        },
                        Metrics = new FitSummaryMetrics
                        {
                            TotalDistanceMeters = 6200,
                            TotalElapsedTime = TimeSpan.FromMinutes(30),
                            TotalTimerTime = TimeSpan.FromMinutes(28),
                            TotalCalories = 280,
                            AverageHeartRateBpm = 152,
                            MaximumHeartRateBpm = 165,
                            EnhancedAverageSpeedMetersPerSecond = 3.69,
                            TotalAscentMeters = 45,
                            TotalDescentMeters = 42,
                        },
                        Laps =
                        [
                            new FitLap
                            {
                                LapIndex = 0,
                                ParentSessionIndex = 0,
                                Range = new FitTimeRange
                                {
                                    StartTimeUtc = BaseTimestamp,
                                    EndTimeUtc = BaseTimestamp.AddMinutes(14),
                                },
                                Metrics = new FitSummaryMetrics
                                {
                                    TotalDistanceMeters = 3000,
                                    TotalTimerTime = TimeSpan.FromMinutes(14),
                                    AverageHeartRateBpm = 148,
                                    MaximumHeartRateBpm = 158,
                                    EnhancedAverageSpeedMetersPerSecond = 3.57,
                                },
                            },
                            new FitLap
                            {
                                LapIndex = 1,
                                ParentSessionIndex = 0,
                                Range = new FitTimeRange
                                {
                                    StartTimeUtc = BaseTimestamp.AddMinutes(14),
                                    EndTimeUtc = BaseTimestamp.AddMinutes(28),
                                },
                                Metrics = new FitSummaryMetrics
                                {
                                    TotalDistanceMeters = 3200,
                                    TotalTimerTime = TimeSpan.FromMinutes(14),
                                    AverageHeartRateBpm = 156,
                                    MaximumHeartRateBpm = 165,
                                    EnhancedAverageSpeedMetersPerSecond = 3.81,
                                },
                            },
                        ],
                        Records = records,
                    },
                ],
            },
        };
    }

    internal static FitFileDocument CreateMultiSportActivity()
    {
        return new FitFileDocument
        {
            FileId = new FitFileId
            {
                FileType = FitFileType.Activity,
                ManufacturerName = "Garmin",
                ProductName = "Forerunner 955",
                SerialNumber = 987654321,
                TimeCreatedUtc = BaseTimestamp,
            },
            ActivityContent = new FitActivityContent
            {
                IsMultiSport = true,
                Sessions =
                [
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
                            TotalCalories = 300,
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
                            TotalCalories = 800,
                            AverageHeartRateBpm = 145,
                            MaximumHeartRateBpm = 168,
                        },
                    },
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
                            TotalCalories = 500,
                            AverageHeartRateBpm = 160,
                            MaximumHeartRateBpm = 178,
                        },
                    },
                ],
            },
        };
    }

    internal static FitFileDocument CreatePoolSwimActivity()
    {
        return new FitFileDocument
        {
            FileId = new FitFileId
            {
                FileType = FitFileType.Activity,
                ManufacturerName = "Garmin",
                ProductName = "Swim 2",
                TimeCreatedUtc = BaseTimestamp,
            },
            ActivityContent = new FitActivityContent
            {
                HasPoolSwim = true,
                Sessions =
                [
                    new FitSession
                    {
                        SessionIndex = 0,
                        Sport = FitSport.Swimming,
                        SubSport = FitSubSport.LapSwimming,
                        Range = new FitTimeRange
                        {
                            StartTimeUtc = BaseTimestamp,
                            EndTimeUtc = BaseTimestamp.AddMinutes(45),
                            TotalTimerTime = TimeSpan.FromMinutes(40),
                        },
                        Metrics = new FitSummaryMetrics
                        {
                            TotalDistanceMeters = 2000,
                            TotalTimerTime = TimeSpan.FromMinutes(40),
                            TotalCalories = 400,
                            PoolLengthMeters = 25.0,
                        },
                    },
                ],
            },
        };
    }

    internal static FitFileDocument CreateWorkoutDocument()
    {
        return new FitFileDocument
        {
            FileId = new FitFileId
            {
                FileType = FitFileType.Workout,
                ManufacturerName = "Garmin",
                TimeCreatedUtc = BaseTimestamp,
            },
            Workout = new FitWorkout
            {
                WorkoutName = "Tempo Run",
                Sport = FitSport.Running,
                SubSport = FitSubSport.Street,
                NumberOfValidSteps = 3,
            },
        };
    }

    internal static FitFileDocument CreateCourseDocument()
    {
        return new FitFileDocument
        {
            FileId = new FitFileId
            {
                FileType = FitFileType.Course,
                ManufacturerName = "Garmin",
                TimeCreatedUtc = BaseTimestamp,
            },
            Course = new FitCourse
            {
                CourseName = "Park Loop",
                Sport = FitSport.Running,
            },
        };
    }

    internal static FitFileDocument CreateDocumentWithDevices()
    {
        return new FitFileDocument
        {
            FileId = new FitFileId
            {
                FileType = FitFileType.Activity,
                ManufacturerName = "Garmin",
                ProductName = "Forerunner 265",
                SerialNumber = 345678901,
                TimeCreatedUtc = BaseTimestamp,
            },
            DeviceInfos =
            [
                new FitDeviceInfo
                {
                    DeviceIndex = 0,
                    Role = FitDeviceRole.Creator,
                    ManufacturerName = "Garmin",
                    ProductName = "Forerunner 265",
                    SerialNumber = 345678901,
                    SoftwareVersion = 12.50,
                    Battery = new BatterySnapshot
                    {
                        Status = FitBatteryStatus.Good,
                        VoltageVolts = 3.95,
                        ChargePercent = 82,
                    },
                },
            ],
            ActivityContent = new FitActivityContent
            {
                Sessions =
                [
                    new FitSession
                    {
                        SessionIndex = 0,
                        Sport = FitSport.Running,
                        Range = new FitTimeRange { StartTimeUtc = BaseTimestamp },
                        Metrics = new FitSummaryMetrics { TotalDistanceMeters = 5000 },
                    },
                ],
            },
        };
    }

    internal static FitFileDocument CreateDocumentWithEvents()
    {
        return new FitFileDocument
        {
            FileId = new FitFileId
            {
                FileType = FitFileType.Activity,
                ManufacturerName = "Garmin",
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
                        Range = new FitTimeRange { StartTimeUtc = BaseTimestamp },
                        Metrics = new FitSummaryMetrics { TotalDistanceMeters = 5000 },
                    },
                ],
                Events =
                [
                    new FitEvent
                    {
                        EventIndex = 0,
                        TimestampUtc = BaseTimestamp,
                        Event = FitEventKind.Timer,
                        EventType = FitEventAction.Start,
                    },
                    new FitEvent
                    {
                        EventIndex = 1,
                        TimestampUtc = BaseTimestamp.AddMinutes(30),
                        Event = FitEventKind.Timer,
                        EventType = FitEventAction.StopAll,
                    },
                ],
            },
        };
    }

    internal static FitFileDocument CreateDocumentWithHrv()
    {
        return new FitFileDocument
        {
            FileId = new FitFileId
            {
                FileType = FitFileType.Activity,
                ManufacturerName = "Garmin",
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
                        Range = new FitTimeRange { StartTimeUtc = BaseTimestamp },
                        Metrics = new FitSummaryMetrics { TotalDistanceMeters = 5000 },
                    },
                ],
                HrvMessages =
                [
                    new FitHrv { RrIntervalsSeconds = [0.800, 0.812, 0.795, 0.830] },
                    new FitHrv { RrIntervalsSeconds = [0.810, 0.820] },
                ],
            },
        };
    }

    internal static MarkdownDocumentOptions CreateDefaultOptions() => new()
    {
        IncludeFrontmatter = true,
        IncludeDeveloperFields = true,
        IncludeRecordSamples = true,
        IncludeDataQualitySection = false,
        PreferCompactTables = false,
        CollapseSingleLapSections = true,
        MaximumRecordSampleRows = 50,
        ApproximateTokenBudget = 0,
    };
}
