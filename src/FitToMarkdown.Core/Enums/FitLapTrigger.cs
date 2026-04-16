namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Mirrors the Dynastream FIT profile LapTrigger enum. Numeric values are preserved from the FIT SDK.
/// </summary>
public enum FitLapTrigger : byte
{
    /// <summary>Manually triggered lap.</summary>
    Manual = 0,

    /// <summary>Time-based auto lap.</summary>
    Time = 1,

    /// <summary>Distance-based auto lap.</summary>
    Distance = 2,

    /// <summary>Position start lap trigger.</summary>
    PositionStart = 3,

    /// <summary>Position lap trigger.</summary>
    PositionLap = 4,

    /// <summary>Position waypoint lap trigger.</summary>
    PositionWaypoint = 5,

    /// <summary>Position marked lap trigger.</summary>
    PositionMarked = 6,

    /// <summary>Session end lap trigger.</summary>
    SessionEnd = 7,

    /// <summary>Fitness equipment lap trigger.</summary>
    FitnessEquipment = 8,

    /// <summary>Unknown or invalid lap trigger.</summary>
    Unknown = 255,
}
