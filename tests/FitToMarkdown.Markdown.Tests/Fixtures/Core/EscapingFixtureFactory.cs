using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Markdown.Tests.Fixtures.Core;

internal static class EscapingFixtureFactory
{
    private static readonly DateTimeOffset BaseTimestamp =
        new(2024, 6, 15, 8, 0, 0, TimeSpan.Zero);

    internal static FitFileDocument CreateWithPipeCharacters()
    {
        return new FitFileDocument
        {
            FileId = new FitFileId
            {
                FileType = FitFileType.Activity,
                ManufacturerName = "Garmin|Edge",
                ProductName = "Edge 1040|Solar",
                TimeCreatedUtc = BaseTimestamp,
            },
            DeviceInfos =
            [
                new FitDeviceInfo
                {
                    DeviceIndex = 0,
                    Role = FitDeviceRole.Creator,
                    ManufacturerName = "Garmin|Edge",
                    ProductName = "Edge 1040|Solar",
                    Descriptor = "Head Unit | Primary",
                    IsCreator = true,
                },
            ],
            ActivityContent = new FitActivityContent
            {
                Sessions =
                [
                    new FitSession
                    {
                        SessionIndex = 0,
                        Sport = FitSport.Cycling,
                        SportProfileName = "Road|Gravel",
                        Range = new FitTimeRange
                        {
                            StartTimeUtc = BaseTimestamp,
                            EndTimeUtc = BaseTimestamp.AddMinutes(60),
                            TotalTimerTime = TimeSpan.FromMinutes(58),
                        },
                        Metrics = new FitSummaryMetrics
                        {
                            TotalDistanceMeters = 30000,
                            TotalTimerTime = TimeSpan.FromMinutes(58),
                        },
                    },
                ],
            },
        };
    }

    internal static FitFileDocument CreateWithSpecialCharacters()
    {
        return new FitFileDocument
        {
            FileId = new FitFileId
            {
                FileType = FitFileType.Activity,
                ManufacturerName = "Garmin #1",
                ProductName = "FR [265] : \"Pro\"",
                TimeCreatedUtc = BaseTimestamp,
            },
            DeviceInfos =
            [
                new FitDeviceInfo
                {
                    DeviceIndex = 0,
                    Role = FitDeviceRole.Creator,
                    ManufacturerName = "Garmin #1",
                    ProductName = "FR [265] : \"Pro\"",
                    Descriptor = "Main\nDevice",
                    IsCreator = true,
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
                        SportProfileName = "Trail's Best",
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
}
