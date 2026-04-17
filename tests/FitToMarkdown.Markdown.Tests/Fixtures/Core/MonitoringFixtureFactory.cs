using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Markdown.Tests.Fixtures.Core;

internal static class MonitoringFixtureFactory
{
    private static readonly DateTimeOffset BaseTimestamp =
        new(2024, 6, 15, 0, 0, 0, TimeSpan.Zero);

    internal static FitFileDocument CreateWithSamples()
    {
        var samples = new List<FitMonitoring>();
        for (int i = 0; i < 24; i++)
        {
            samples.Add(new FitMonitoring
            {
                TimestampUtc = BaseTimestamp.AddHours(i),
                Calories = (ushort)(100 + i * 10),
                DistanceMeters = i * 500.0,
                Cycles = i * 200.0,
            });
        }

        return new FitFileDocument
        {
            FileId = new FitFileId
            {
                FileType = FitFileType.MonitoringDaily,
                ManufacturerName = "Garmin",
                ProductName = "Venu 3",
                TimeCreatedUtc = BaseTimestamp,
            },
            MonitoringContent = new FitMonitoringContent
            {
                Range = new FitTimeRange
                {
                    StartTimeUtc = BaseTimestamp,
                    EndTimeUtc = BaseTimestamp.AddHours(24),
                },
                Samples = samples,
                ContainsDailySummary = true,
            },
        };
    }
}
