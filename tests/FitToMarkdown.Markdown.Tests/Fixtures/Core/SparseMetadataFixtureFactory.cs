using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Markdown.Tests.Fixtures.Core;

internal static class SparseMetadataFixtureFactory
{
    private static readonly DateTimeOffset BaseTimestamp =
        new(2024, 6, 15, 8, 0, 0, TimeSpan.Zero);

    internal static FitFileDocument CreateWithMissingManufacturer()
    {
        return new FitFileDocument
        {
            FileId = new FitFileId
            {
                FileType = FitFileType.Activity,
                ManufacturerName = null,
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
                            EndTimeUtc = BaseTimestamp.AddMinutes(30),
                            TotalTimerTime = TimeSpan.FromMinutes(28),
                        },
                        Metrics = new FitSummaryMetrics
                        {
                            TotalDistanceMeters = 5000,
                            TotalTimerTime = TimeSpan.FromMinutes(28),
                        },
                    },
                ],
            },
        };
    }

    internal static FitFileDocument CreateWithMissingProduct()
    {
        return new FitFileDocument
        {
            FileId = new FitFileId
            {
                FileType = FitFileType.Activity,
                ManufacturerName = "Garmin",
                ProductName = null,
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
                            EndTimeUtc = BaseTimestamp.AddMinutes(30),
                            TotalTimerTime = TimeSpan.FromMinutes(28),
                        },
                        Metrics = new FitSummaryMetrics
                        {
                            TotalDistanceMeters = 5000,
                            TotalTimerTime = TimeSpan.FromMinutes(28),
                        },
                    },
                ],
            },
        };
    }

    internal static FitFileDocument CreateMinimalFileIdOnly()
    {
        return new FitFileDocument
        {
            FileId = new FitFileId
            {
                FileType = FitFileType.Activity,
            },
            ActivityContent = new FitActivityContent
            {
                Sessions =
                [
                    new FitSession
                    {
                        SessionIndex = 0,
                        Range = new FitTimeRange(),
                        Metrics = new FitSummaryMetrics(),
                    },
                ],
            },
        };
    }
}
