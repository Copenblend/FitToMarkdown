using FitToMarkdown.Core.Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace FitToMarkdown.Cli.Rendering;

internal sealed class InfoTableRenderer
{
    private readonly IAnsiConsole _console;

    internal InfoTableRenderer(IAnsiConsole console)
    {
        _console = console;
    }

    internal IRenderable RenderSummary(FitFileInfoSummary summary)
    {
        var table = new Table()
            .RoundedBorder()
            .BorderColor(CliTheme.PrimaryColor)
            .HideHeaders()
            .AddColumn("Key")
            .AddColumn("Value");

        table.AddRow("File Type", FormatValue(summary.FileType));
        table.AddRow("Sport", FormatValue(summary.Sport));
        table.AddRow("Sub-Sport", FormatValue(summary.SubSport));
        table.AddRow("Manufacturer", FormatValue(summary.ManufacturerName));
        table.AddRow("Product", FormatProduct(summary.ProductName, summary.ProductId));
        table.AddRow("Serial Number", FormatValue(summary.SerialNumber?.ToString()));
        table.AddRow("Start Time", FormatValue(summary.StartTimeUtc?.ToString("yyyy-MM-dd HH:mm:ss 'UTC'")));
        table.AddRow("Duration", FormatDuration(summary.TotalTimerTime, summary.TotalElapsedTime));
        table.AddRow("Distance", FormatDistance(summary.TotalDistanceMeters));
        table.AddRow("Lap Count", FormatValue(summary.LapCount?.ToString()));
        table.AddRow("Record Count", FormatValue(summary.RecordCount?.ToString()));

        return table;
    }

    internal IRenderable? RenderDevices(IReadOnlyList<FitDeviceSummary> devices)
    {
        if (devices.Count == 0)
            return null;

        var table = new Table()
            .RoundedBorder()
            .BorderColor(CliTheme.PrimaryColor)
            .AddColumn("Device Type")
            .AddColumn("Manufacturer")
            .AddColumn("Product")
            .AddColumn("Serial Number")
            .AddColumn("Battery")
            .AddColumn("Descriptor");

        foreach (var device in devices)
        {
            table.AddRow(
                FormatValue(device.DeviceType),
                FormatValue(device.ManufacturerName),
                FormatProduct(device.ProductName, device.ProductId),
                FormatValue(device.SerialNumber?.ToString()),
                FormatBattery(device.BatteryStatus, device.BatteryVoltage),
                FormatValue(device.Descriptor));
        }

        return table;
    }

    private static string FormatValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? CliMarkup.DimOrFallback(null)
            : CliMarkup.Escape(value);
    }

    private static string FormatProduct(string? productName, ushort? productId)
    {
        if (string.IsNullOrWhiteSpace(productName))
            return CliMarkup.DimOrFallback(null);

        if (productId.HasValue)
            return $"{CliMarkup.Escape(productName)} [dim]({productId.Value})[/]";

        return CliMarkup.Escape(productName);
    }

    private static string FormatDuration(TimeSpan? timerTime, TimeSpan? elapsedTime)
    {
        if (!timerTime.HasValue)
            return CliMarkup.DimOrFallback(null);

        var formatted = FormatTimeSpan(timerTime.Value);

        if (elapsedTime.HasValue && elapsedTime.Value != timerTime.Value)
            formatted += $" [dim](elapsed: {FormatTimeSpan(elapsedTime.Value)})[/]";

        return formatted;
    }

    private static string FormatTimeSpan(TimeSpan ts)
    {
        return $"{(int)ts.TotalHours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}";
    }

    private static string FormatDistance(double? distanceMeters)
    {
        if (!distanceMeters.HasValue)
            return CliMarkup.DimOrFallback(null);

        var km = distanceMeters.Value / 1000.0;
        return $"{km:F2} km";
    }

    private static string FormatBattery(string? status, double? voltage)
    {
        if (string.IsNullOrWhiteSpace(status) && !voltage.HasValue)
            return CliMarkup.DimOrFallback(null);

        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(status))
            parts.Add(CliMarkup.Escape(status));
        if (voltage.HasValue)
            parts.Add($"({voltage.Value:F2}V)");

        return string.Join(" ", parts);
    }
}
