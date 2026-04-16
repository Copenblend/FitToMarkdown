using System.Globalization;
using System.Text;
using FitToMarkdown.Core.Summaries;
using FitToMarkdown.Markdown.Formatting;

namespace FitToMarkdown.Markdown.Rendering.Sections;

internal static class DevicesRenderer
{
    internal static void Render(IReadOnlyList<DeviceSummaryRow> devices, StringBuilder sb)
    {
        var writer = new MarkdownTextWriter();
        writer.AppendHeading(2, "Devices");

        var headers = new[] { "#", "Role", "Manufacturer", "Product", "Serial", "Software", "Battery" };
        var rows = new List<string[]>();

        foreach (var device in devices)
        {
            rows.Add(
            [
                device.Order.ToString(CultureInfo.InvariantCulture),
                device.Role.ToString(),
                MarkdownEscaper.SanitizeFitString(device.ManufacturerName),
                MarkdownEscaper.SanitizeFitString(device.ProductName),
                device.SerialNumber?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
                MarkdownValueFormatter.FormatSoftwareVersion(device.SoftwareVersion),
                MarkdownValueFormatter.FormatBattery(device.Battery),
            ]);
        }

        writer.AppendTable(headers, rows);
        sb.Append(writer.ToString());
    }
}
