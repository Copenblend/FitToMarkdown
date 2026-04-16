using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized FileIdMesg from a FIT file.
/// </summary>
public sealed record FitFileId
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>FIT file type.</summary>
    public FitFileType? FileType { get; init; }

    /// <summary>Manufacturer identifier from the FIT profile.</summary>
    public ushort? ManufacturerId { get; init; }

    /// <summary>Resolved manufacturer display name.</summary>
    public string? ManufacturerName { get; init; }

    /// <summary>Product identifier.</summary>
    public ushort? ProductId { get; init; }

    /// <summary>Resolved product display name.</summary>
    public string? ProductName { get; init; }

    /// <summary>Device serial number.</summary>
    public uint? SerialNumber { get; init; }

    /// <summary>File creation timestamp in UTC.</summary>
    public DateTimeOffset? TimeCreatedUtc { get; init; }

    /// <summary>File number.</summary>
    public ushort? Number { get; init; }
}
