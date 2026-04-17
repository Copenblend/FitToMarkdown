using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.Parsing;

namespace FitToMarkdown.Cli.Tests.Fixtures;

internal static class MetadataInspectionResultFactory
{
    internal static FitMetadataInspectionResult CreateSuccessful()
    {
        return new FitMetadataInspectionResult
        {
            Summary = new FitFileInfoSummary
            {
                InputFilePath = @"C:\test\activity.fit",
                FileName = "activity.fit",
                FileType = "Activity",
                Sport = "Running",
                SubSport = "Generic",
                ManufacturerName = "Garmin",
                ProductName = "Forerunner 265",
                ProductId = 4257,
                SerialNumber = 3456789012,
                StartTimeUtc = new DateTimeOffset(2025, 6, 15, 8, 30, 0, TimeSpan.Zero),
                TotalTimerTime = TimeSpan.FromMinutes(45.5),
                TotalElapsedTime = TimeSpan.FromMinutes(48.0),
                TotalDistanceMeters = 8500.0,
                LapCount = 5,
                RecordCount = 2730,
            },
        };
    }

    internal static FitMetadataInspectionResult CreateWithDevices()
    {
        var result = CreateSuccessful();
        return result with
        {
            Summary = result.Summary! with
            {
                Devices =
                [
                    new FitDeviceSummary
                    {
                        DeviceType = "Watch",
                        ManufacturerName = "Garmin",
                        ProductName = "Forerunner 265",
                        ProductId = 4257,
                        SerialNumber = 3456789012,
                        BatteryStatus = "Good",
                        BatteryVoltage = 3.95,
                        Descriptor = "Primary",
                    },
                ],
            },
        };
    }

    internal static FitMetadataInspectionResult CreateFailed(string message)
    {
        return new FitMetadataInspectionResult
        {
            FatalError = new FitParseError
            {
                Code = "INSPECT_FAILED",
                Message = message,
            },
        };
    }
}
