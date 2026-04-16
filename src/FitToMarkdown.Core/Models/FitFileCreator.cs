using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized FileCreatorMesg from a FIT file.
/// </summary>
public sealed record FitFileCreator
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>Software version of the device that created the file.</summary>
    public double? SoftwareVersion { get; init; }

    /// <summary>Hardware version of the device that created the file.</summary>
    public byte? HardwareVersion { get; init; }
}
