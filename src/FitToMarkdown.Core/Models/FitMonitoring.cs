using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized MonitoringMesg from a FIT file.
/// </summary>
public sealed record FitMonitoring
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>Timestamp of the monitoring message in UTC.</summary>
    public DateTimeOffset? TimestampUtc { get; init; }

    /// <summary>Device index within the monitoring file.</summary>
    public ushort? DeviceIndex { get; init; }

    /// <summary>Accumulated calories.</summary>
    public ushort? Calories { get; init; }

    /// <summary>Accumulated distance in meters.</summary>
    public double? DistanceMeters { get; init; }

    /// <summary>Accumulated cycles (steps, strokes, etc.).</summary>
    public double? Cycles { get; init; }

    /// <summary>Accumulated active time.</summary>
    public TimeSpan? ActiveTime { get; init; }

    /// <summary>Activity type for this monitoring sample.</summary>
    public FitActivityType? ActivityType { get; init; }

    /// <summary>Activity subtype descriptor.</summary>
    public string? ActivitySubtype { get; init; }

    /// <summary>Activity level descriptor.</summary>
    public string? ActivityLevel { get; init; }

    /// <summary>Distance in 1/16 meter resolution.</summary>
    public double? Distance16Meters { get; init; }

    /// <summary>Cycles in 1/16 resolution.</summary>
    public double? Cycles16 { get; init; }

    /// <summary>Active time in 1/16 second resolution.</summary>
    public TimeSpan? ActiveTime16 { get; init; }

    /// <summary>Local timestamp from the device.</summary>
    public DateTimeOffset? LocalTimestamp { get; init; }

    /// <summary>Temperature in degrees Celsius.</summary>
    public double? TemperatureCelsius { get; init; }

    /// <summary>Minimum temperature in degrees Celsius.</summary>
    public double? MinimumTemperatureCelsius { get; init; }

    /// <summary>Maximum temperature in degrees Celsius.</summary>
    public double? MaximumTemperatureCelsius { get; init; }

    /// <summary>Activity time duration.</summary>
    public TimeSpan? ActivityTime { get; init; }

    /// <summary>Active calories burned.</summary>
    public ushort? ActiveCalories { get; init; }

    /// <summary>Current activity type and intensity descriptor.</summary>
    public string? CurrentActivityTypeIntensity { get; init; }

    /// <summary>Timestamp minute offset (8-bit).</summary>
    public byte? TimestampMin8 { get; init; }

    /// <summary>Timestamp 16-bit value.</summary>
    public ushort? Timestamp16 { get; init; }

    /// <summary>Heart rate in beats per minute.</summary>
    public byte? HeartRateBpm { get; init; }

    /// <summary>Intensity descriptor.</summary>
    public string? Intensity { get; init; }

    /// <summary>Duration in minutes.</summary>
    public ushort? DurationMin { get; init; }

    /// <summary>Duration as a time span.</summary>
    public TimeSpan? Duration { get; init; }

    /// <summary>Total ascent in meters.</summary>
    public ushort? AscentMeters { get; init; }

    /// <summary>Total descent in meters.</summary>
    public ushort? DescentMeters { get; init; }

    /// <summary>Moderate activity minutes.</summary>
    public ushort? ModerateActivityMinutes { get; init; }

    /// <summary>Vigorous activity minutes.</summary>
    public ushort? VigorousActivityMinutes { get; init; }

    /// <summary>Developer field values attached to this message.</summary>
    public IReadOnlyList<FitDeveloperFieldValue> DeveloperFields { get; init; } = [];
}
