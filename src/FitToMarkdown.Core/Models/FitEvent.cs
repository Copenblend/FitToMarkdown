using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized EventMesg with dynamic event data flattened into stable properties.
/// </summary>
public sealed record FitEvent
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>Zero-based event index within the activity.</summary>
    public int EventIndex { get; init; }

    /// <summary>Session index this event was assigned to.</summary>
    public int? SessionIndex { get; init; }

    /// <summary>Timestamp of the event in UTC.</summary>
    public DateTimeOffset? TimestampUtc { get; init; }

    /// <summary>Event kind.</summary>
    public FitEventKind? Event { get; init; }

    /// <summary>Event action type.</summary>
    public FitEventAction? EventType { get; init; }

    /// <summary>Raw event data field.</summary>
    public uint? Data { get; init; }

    /// <summary>Raw 16-bit event data field.</summary>
    public ushort? Data16 { get; init; }

    /// <summary>Timer trigger descriptor.</summary>
    public string? TimerTrigger { get; init; }

    /// <summary>Course point index associated with the event.</summary>
    public ushort? CoursePointIndex { get; init; }

    /// <summary>Battery level as a percentage.</summary>
    public double? BatteryLevelPercent { get; init; }

    /// <summary>Virtual partner speed in meters per second.</summary>
    public double? VirtualPartnerSpeedMetersPerSecond { get; init; }

    /// <summary>Heart rate high alert threshold in BPM.</summary>
    public byte? HeartRateHighAlertBpm { get; init; }

    /// <summary>Heart rate low alert threshold in BPM.</summary>
    public byte? HeartRateLowAlertBpm { get; init; }

    /// <summary>Speed high alert threshold in meters per second.</summary>
    public double? SpeedHighAlertMetersPerSecond { get; init; }

    /// <summary>Speed low alert threshold in meters per second.</summary>
    public double? SpeedLowAlertMetersPerSecond { get; init; }

    /// <summary>Cadence high alert threshold in RPM.</summary>
    public byte? CadenceHighAlertRpm { get; init; }

    /// <summary>Cadence low alert threshold in RPM.</summary>
    public byte? CadenceLowAlertRpm { get; init; }

    /// <summary>Power high alert threshold in watts.</summary>
    public ushort? PowerHighAlertWatts { get; init; }

    /// <summary>Power low alert threshold in watts.</summary>
    public ushort? PowerLowAlertWatts { get; init; }

    /// <summary>Time duration alert threshold.</summary>
    public TimeSpan? TimeDurationAlert { get; init; }

    /// <summary>Distance duration alert threshold in meters.</summary>
    public double? DistanceDurationAlertMeters { get; init; }

    /// <summary>Calorie duration alert threshold.</summary>
    public ushort? CalorieDurationAlert { get; init; }

    /// <summary>Fitness equipment state descriptor.</summary>
    public string? FitnessEquipmentState { get; init; }

    /// <summary>Sport point value.</summary>
    public uint? SportPoint { get; init; }

    /// <summary>Front gear number.</summary>
    public byte? FrontGearNumber { get; init; }

    /// <summary>Front gear teeth count.</summary>
    public ushort? FrontGearTeeth { get; init; }

    /// <summary>Rear gear number.</summary>
    public byte? RearGearNumber { get; init; }

    /// <summary>Rear gear teeth count.</summary>
    public ushort? RearGearTeeth { get; init; }

    /// <summary>Rider position descriptor.</summary>
    public string? RiderPosition { get; init; }

    /// <summary>Communication timeout duration.</summary>
    public TimeSpan? CommunicationTimeout { get; init; }

    /// <summary>Radar threat alert type descriptor.</summary>
    public string? RadarThreatAlertType { get; init; }

    /// <summary>Developer field values attached to this message.</summary>
    public IReadOnlyList<FitDeveloperFieldValue> DeveloperFields { get; init; } = [];
}
