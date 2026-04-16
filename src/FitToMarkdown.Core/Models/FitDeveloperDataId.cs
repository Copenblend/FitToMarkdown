using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized DeveloperDataIdMesg from a FIT file.
/// </summary>
public sealed record FitDeveloperDataId
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>Developer data index identifying the application or data source.</summary>
    public ushort DeveloperDataIndex { get; init; }

    /// <summary>Developer identifier as a hexadecimal string.</summary>
    public string? DeveloperIdHex { get; init; }

    /// <summary>Application identifier as a hexadecimal string.</summary>
    public string? ApplicationIdHex { get; init; }

    /// <summary>Manufacturer identifier from the FIT profile.</summary>
    public ushort? ManufacturerId { get; init; }

    /// <summary>Resolved manufacturer display name.</summary>
    public string? ManufacturerName { get; init; }

    /// <summary>Application version number.</summary>
    public uint? ApplicationVersion { get; init; }
}
