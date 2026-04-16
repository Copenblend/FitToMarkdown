using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Summaries;

namespace FitToMarkdown.Markdown.Projection;

internal static class DeviceProjector
{
    internal static IReadOnlyList<DeviceSummaryRow> Project(FitFileDocument document)
    {
        if (document.DeviceInfos.Count == 0)
            return [];

        // Group by identity key, taking the latest entry per group
        var latest = document.DeviceInfos
            .GroupBy(d => (d.ManufacturerName, d.ProductName, d.SerialNumber))
            .Select(g => g
                .OrderByDescending(d => d.TimestampUtc ?? DateTimeOffset.MinValue)
                .First())
            .OrderBy(d => d.IsCreator ? 0 : 1)
            .ThenBy(d => (int)(d.Role ?? FitDeviceRole.Unknown))
            .ThenBy(d => d.Descriptor ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var rows = new List<DeviceSummaryRow>(latest.Count);
        for (int i = 0; i < latest.Count; i++)
        {
            var d = latest[i];
            rows.Add(new DeviceSummaryRow
            {
                Order = i + 1,
                Role = d.Role ?? FitDeviceRole.Unknown,
                ManufacturerName = d.ManufacturerName,
                ProductName = d.ProductName,
                SerialNumber = d.SerialNumber,
                SoftwareVersion = d.SoftwareVersion,
                Battery = d.Battery,
                Descriptor = d.Descriptor,
                SourceType = d.SourceType,
            });
        }

        return rows;
    }
}
