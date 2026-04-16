namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Mirrors the Dynastream FIT profile Event enum. Numeric values are preserved from the FIT SDK.
/// </summary>
public enum FitEventKind : byte
{
    /// <summary>Timer event (start, stop, pause).</summary>
    Timer = 0,

    /// <summary>Workout event.</summary>
    Workout = 3,

    /// <summary>Workout step event.</summary>
    WorkoutStep = 4,

    /// <summary>Power down event.</summary>
    PowerDown = 5,

    /// <summary>Power up event.</summary>
    PowerUp = 6,

    /// <summary>Off-course event.</summary>
    OffCourse = 7,

    /// <summary>Session event.</summary>
    Session = 8,

    /// <summary>Lap event.</summary>
    Lap = 9,

    /// <summary>Course point event.</summary>
    CoursePoint = 10,

    /// <summary>Battery event.</summary>
    Battery = 11,

    /// <summary>Virtual partner pace event.</summary>
    VirtualPartnerPace = 12,

    /// <summary>Heart rate high alert.</summary>
    HrHighAlert = 13,

    /// <summary>Heart rate low alert.</summary>
    HrLowAlert = 14,

    /// <summary>Speed high alert.</summary>
    SpeedHighAlert = 15,

    /// <summary>Speed low alert.</summary>
    SpeedLowAlert = 16,

    /// <summary>Cadence high alert.</summary>
    CadHighAlert = 17,

    /// <summary>Cadence low alert.</summary>
    CadLowAlert = 18,

    /// <summary>Power high alert.</summary>
    PowerHighAlert = 19,

    /// <summary>Power low alert.</summary>
    PowerLowAlert = 20,

    /// <summary>Recovery heart rate event.</summary>
    RecoveryHr = 21,

    /// <summary>Battery low event.</summary>
    BatteryLow = 22,

    /// <summary>Time duration alert.</summary>
    TimeDurationAlert = 23,

    /// <summary>Distance duration alert.</summary>
    DistanceDurationAlert = 24,

    /// <summary>Calorie duration alert.</summary>
    CalorieDurationAlert = 25,

    /// <summary>Activity event.</summary>
    Activity = 26,

    /// <summary>Fitness equipment event.</summary>
    FitnessEquipment = 27,

    /// <summary>Length event (pool swim).</summary>
    Length = 28,

    /// <summary>User marker event.</summary>
    UserMarker = 32,

    /// <summary>Sport point event.</summary>
    SportPoint = 33,

    /// <summary>Calibration event.</summary>
    Calibration = 36,

    /// <summary>Front gear change event.</summary>
    FrontGearChange = 42,

    /// <summary>Rear gear change event.</summary>
    RearGearChange = 43,

    /// <summary>Rider position change event.</summary>
    RiderPositionChange = 44,

    /// <summary>Elevation high alert.</summary>
    ElevHighAlert = 45,

    /// <summary>Elevation low alert.</summary>
    ElevLowAlert = 46,

    /// <summary>Communication timeout event.</summary>
    CommTimeout = 47,

    /// <summary>Unknown or invalid event kind.</summary>
    Unknown = 255,
}
