using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Markdown.Tests.Fixtures.Core;

internal static class DeveloperFieldsFixtureFactory
{
    private static readonly DateTimeOffset BaseTimestamp =
        new(2024, 6, 15, 8, 0, 0, TimeSpan.Zero);

    internal static FitFileDocument CreateWithResolvedFields()
    {
        var devDataId = new FitDeveloperDataId
        {
            DeveloperDataIndex = 0,
            ApplicationIdHex = "AABBCCDD",
            ApplicationVersion = 1,
        };

        var fieldDesc = new FitFieldDescription
        {
            Key = new FitDeveloperFieldKey { DeveloperDataIndex = 0, FieldDefinitionNumber = 0 },
            FieldName = "ground_contact_time",
            Units = "ms",
            DefinitionResolved = true,
        };

        var records = new List<FitRecord>();
        for (int i = 0; i < 3; i++)
        {
            records.Add(new FitRecord
            {
                RecordIndex = i,
                SessionIndex = 0,
                TimestampUtc = BaseTimestamp.AddSeconds(i * 60),
                HeartRateBpm = (byte)(140 + i),
                DeveloperFields =
                [
                    new FitDeveloperFieldValue
                    {
                        Key = new FitDeveloperFieldKey { DeveloperDataIndex = 0, FieldDefinitionNumber = 0 },
                        DefinitionResolved = true,
                        FieldName = "ground_contact_time",
                        Units = "ms",
                        ValueKind = FitDeveloperValueKind.Numeric,
                        NumericValue = 245.0 + i,
                    },
                ],
            });
        }

        return new FitFileDocument
        {
            FileId = new FitFileId
            {
                FileType = FitFileType.Activity,
                ManufacturerName = "Garmin",
                ProductName = "Forerunner 955",
                TimeCreatedUtc = BaseTimestamp,
            },
            DeveloperDataIds = [devDataId],
            FieldDescriptions = [fieldDesc],
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
                            EndTimeUtc = BaseTimestamp.AddMinutes(10),
                            TotalTimerTime = TimeSpan.FromMinutes(10),
                        },
                        Metrics = new FitSummaryMetrics
                        {
                            TotalDistanceMeters = 2000,
                            TotalTimerTime = TimeSpan.FromMinutes(10),
                        },
                        Records = records,
                    },
                ],
            },
        };
    }

    internal static FitFileDocument CreateWithUnresolvedFields()
    {
        var devDataId = new FitDeveloperDataId
        {
            DeveloperDataIndex = 0,
            ApplicationIdHex = "AABBCCDD",
            ApplicationVersion = 1,
        };

        return new FitFileDocument
        {
            FileId = new FitFileId
            {
                FileType = FitFileType.Activity,
                ManufacturerName = "Garmin",
                TimeCreatedUtc = BaseTimestamp,
            },
            DeveloperDataIds = [devDataId],
            FieldDescriptions = [],
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
                            EndTimeUtc = BaseTimestamp.AddMinutes(10),
                            TotalTimerTime = TimeSpan.FromMinutes(10),
                        },
                        Metrics = new FitSummaryMetrics
                        {
                            TotalDistanceMeters = 2000,
                            TotalTimerTime = TimeSpan.FromMinutes(10),
                        },
                    },
                ],
            },
        };
    }
}
