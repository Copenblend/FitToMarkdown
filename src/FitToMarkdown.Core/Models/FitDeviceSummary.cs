namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the compact device summary consumed by the CLI info command.
/// </summary>
public sealed record FitDeviceSummary
{
    /// <summary>Classified device type descriptor.</summary>
    public string? DeviceType { get; init; }

    /// <summary>Resolved manufacturer display name.</summary>
    public string? ManufacturerName { get; init; }

    /// <summary>Product identifier.</summary>
    public ushort? ProductId { get; init; }

    /// <summary>Resolved product display name.</summary>
    public string? ProductName { get; init; }

    /// <summary>Device serial number.</summary>
    public uint? SerialNumber { get; init; }

    /// <summary>Battery voltage in volts.</summary>
    public double? BatteryVoltage { get; init; }

    /// <summary>Battery status descriptor.</summary>
    public string? BatteryStatus { get; init; }

    /// <summary>Free-text device descriptor.</summary>
    public string? Descriptor { get; init; }
}
