namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Mirrors the Dynastream FIT profile WktStepTarget enum. Numeric values are preserved from the FIT SDK.
/// </summary>
public enum FitWorkoutTargetType : byte
{
    /// <summary>Speed target.</summary>
    Speed = 0,

    /// <summary>Heart rate target.</summary>
    HeartRate = 1,

    /// <summary>Open (no target).</summary>
    Open = 2,

    /// <summary>Cadence target.</summary>
    Cadence = 3,

    /// <summary>Power target.</summary>
    Power = 4,

    /// <summary>Grade target.</summary>
    Grade = 5,

    /// <summary>Resistance target.</summary>
    Resistance = 6,

    /// <summary>3-second power target.</summary>
    Power3s = 7,

    /// <summary>10-second power target.</summary>
    Power10s = 8,

    /// <summary>30-second power target.</summary>
    Power30s = 9,

    /// <summary>Lap power target.</summary>
    PowerLap = 10,

    /// <summary>Swim stroke target.</summary>
    SwimStroke = 11,

    /// <summary>Lap speed target.</summary>
    SpeedLap = 12,

    /// <summary>Lap heart rate target.</summary>
    HeartRateLap = 13,

    /// <summary>Unknown or invalid workout target type.</summary>
    Unknown = 255,
}
