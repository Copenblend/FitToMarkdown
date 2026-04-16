using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized ZonesTargetMesg from a FIT file.
/// </summary>
public sealed record FitZonesTarget
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>Maximum heart rate in beats per minute.</summary>
    public byte? MaximumHeartRateBpm { get; init; }

    /// <summary>Threshold heart rate in beats per minute.</summary>
    public byte? ThresholdHeartRateBpm { get; init; }

    /// <summary>Functional threshold power in watts.</summary>
    public ushort? FunctionalThresholdPowerWatts { get; init; }

    /// <summary>Heart rate zone calculation type.</summary>
    public string? HeartRateCalculationType { get; init; }

    /// <summary>Power zone calculation type.</summary>
    public string? PowerCalculationType { get; init; }
}
