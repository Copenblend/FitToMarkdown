namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Mirrors the Dynastream FIT profile File enum. Numeric values are preserved from the FIT SDK.
/// </summary>
public enum FitFileType : byte
{
    /// <summary>Device file.</summary>
    Device = 1,

    /// <summary>Settings file.</summary>
    Settings = 2,

    /// <summary>Sport file.</summary>
    Sport = 3,

    /// <summary>Activity file containing sensor data and events.</summary>
    Activity = 4,

    /// <summary>Workout file containing structured activity parameters.</summary>
    Workout = 5,

    /// <summary>Course file containing route data.</summary>
    Course = 6,

    /// <summary>Schedules file.</summary>
    Schedules = 7,

    /// <summary>Weight file.</summary>
    Weight = 9,

    /// <summary>Totals file.</summary>
    Totals = 10,

    /// <summary>Goals file.</summary>
    Goals = 11,

    /// <summary>Blood pressure file.</summary>
    BloodPressure = 14,

    /// <summary>Monitoring file (type A).</summary>
    MonitoringA = 15,

    /// <summary>Activity summary file.</summary>
    ActivitySummary = 20,

    /// <summary>Daily monitoring file.</summary>
    MonitoringDaily = 28,

    /// <summary>Monitoring file (type B).</summary>
    MonitoringB = 32,

    /// <summary>Segment file.</summary>
    Segment = 34,

    /// <summary>Segment list file.</summary>
    SegmentList = 35,

    /// <summary>Extended display configuration file.</summary>
    ExdConfiguration = 40,

    /// <summary>Manufacturer-specific range minimum.</summary>
    MfgRangeMin = 247,

    /// <summary>Manufacturer-specific range maximum.</summary>
    MfgRangeMax = 254,

    /// <summary>Unknown or invalid file type.</summary>
    Unknown = 255,
}
