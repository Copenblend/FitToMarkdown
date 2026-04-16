using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized DeviceInfoMesg from a FIT file.
/// </summary>
public sealed record FitDeviceInfo
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>Timestamp of the device info message in UTC.</summary>
    public DateTimeOffset? TimestampUtc { get; init; }

    /// <summary>Device index within the activity.</summary>
    public ushort? DeviceIndex { get; init; }

    /// <summary>Classified device role.</summary>
    public FitDeviceRole? Role { get; init; }

    /// <summary>ANT+ device type identifier.</summary>
    public byte? DeviceType { get; init; }

    /// <summary>Manufacturer identifier from the FIT profile.</summary>
    public ushort? ManufacturerId { get; init; }

    /// <summary>Resolved manufacturer display name.</summary>
    public string? ManufacturerName { get; init; }

    /// <summary>Device serial number.</summary>
    public uint? SerialNumber { get; init; }

    /// <summary>Product identifier.</summary>
    public ushort? ProductId { get; init; }

    /// <summary>Resolved product display name.</summary>
    public string? ProductName { get; init; }

    /// <summary>Software version.</summary>
    public double? SoftwareVersion { get; init; }

    /// <summary>Hardware version.</summary>
    public byte? HardwareVersion { get; init; }

    /// <summary>Battery status and voltage snapshot.</summary>
    public BatterySnapshot? Battery { get; init; }

    /// <summary>Sensor body position descriptor.</summary>
    public string? SensorPosition { get; init; }

    /// <summary>Free-text device descriptor.</summary>
    public string? Descriptor { get; init; }

    /// <summary>ANT transmission type.</summary>
    public byte? AntTransmissionType { get; init; }

    /// <summary>ANT device number.</summary>
    public ushort? AntDeviceNumber { get; init; }

    /// <summary>ANT network type.</summary>
    public string? AntNetwork { get; init; }

    /// <summary>Source type identifier.</summary>
    public string? SourceType { get; init; }

    /// <summary>Whether this device info represents the file creator device.</summary>
    public bool IsCreator { get; init; }

    /// <summary>Developer field values attached to this message.</summary>
    public IReadOnlyList<FitDeveloperFieldValue> DeveloperFields { get; init; } = [];
}
