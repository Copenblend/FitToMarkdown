using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Summaries;

/// <summary>
/// Represents one markdown-ready device summary row.
/// </summary>
public sealed record DeviceSummaryRow
{
    /// <summary>Display order of this device row.</summary>
    public int Order { get; init; }

    /// <summary>Role of the device in the activity.</summary>
    public FitDeviceRole Role { get; init; }

    /// <summary>Name of the device manufacturer.</summary>
    public string? ManufacturerName { get; init; }

    /// <summary>Name of the product or device model.</summary>
    public string? ProductName { get; init; }

    /// <summary>Device serial number.</summary>
    public uint? SerialNumber { get; init; }

    /// <summary>Software version of the device firmware.</summary>
    public double? SoftwareVersion { get; init; }

    /// <summary>Battery snapshot for the device.</summary>
    public BatterySnapshot? Battery { get; init; }

    /// <summary>Human-readable descriptor for the device.</summary>
    public string? Descriptor { get; init; }

    /// <summary>Source type classification for the device.</summary>
    public string? SourceType { get; init; }
}
