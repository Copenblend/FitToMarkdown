using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized UserProfileMesg from a FIT file.
/// </summary>
public sealed record FitUserProfile
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>User-assigned friendly name.</summary>
    public string? FriendlyName { get; init; }

    /// <summary>Gender of the user.</summary>
    public FitGender? Gender { get; init; }

    /// <summary>Age of the user in years.</summary>
    public byte? AgeYears { get; init; }

    /// <summary>Height of the user in centimeters.</summary>
    public double? HeightCentimeters { get; init; }

    /// <summary>Weight of the user in kilograms.</summary>
    public double? WeightKilograms { get; init; }

    /// <summary>Language setting.</summary>
    public string? Language { get; init; }

    /// <summary>Elevation display setting.</summary>
    public string? ElevationSetting { get; init; }

    /// <summary>Weight display setting.</summary>
    public string? WeightSetting { get; init; }

    /// <summary>Resting heart rate in beats per minute.</summary>
    public byte? RestingHeartRateBpm { get; init; }

    /// <summary>Default maximum running heart rate in beats per minute.</summary>
    public byte? DefaultMaxRunningHeartRateBpm { get; init; }

    /// <summary>Default maximum biking heart rate in beats per minute.</summary>
    public byte? DefaultMaxBikingHeartRateBpm { get; init; }

    /// <summary>Default maximum heart rate in beats per minute.</summary>
    public byte? DefaultMaxHeartRateBpm { get; init; }

    /// <summary>Heart rate display setting.</summary>
    public string? HeartRateSetting { get; init; }

    /// <summary>Speed display setting.</summary>
    public string? SpeedSetting { get; init; }

    /// <summary>Distance display setting.</summary>
    public string? DistanceSetting { get; init; }

    /// <summary>Activity class of the user.</summary>
    public byte? ActivityClass { get; init; }

    /// <summary>Position display setting.</summary>
    public string? PositionSetting { get; init; }

    /// <summary>Temperature display setting.</summary>
    public string? TemperatureSetting { get; init; }

    /// <summary>Height display setting.</summary>
    public string? HeightSetting { get; init; }
}
