namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Identifies every conditional markdown section.
/// </summary>
public enum FitMarkdownSectionKey : byte
{
    /// <summary>YAML frontmatter with file-level metadata.</summary>
    Frontmatter = 0,

    /// <summary>Activity overview and high-level stats.</summary>
    Overview = 1,

    /// <summary>Session summary table.</summary>
    SessionSummary = 2,

    /// <summary>Detailed per-session metrics.</summary>
    SessionDetails = 3,

    /// <summary>Per-lap split details.</summary>
    LapDetails = 4,

    /// <summary>Per-length details (pool swim).</summary>
    LengthDetails = 5,

    /// <summary>Aggregated record statistics.</summary>
    RecordSummary = 6,

    /// <summary>Down-sampled record time-series data.</summary>
    RecordTimeSeries = 7,

    /// <summary>Heart-rate zone distribution.</summary>
    HeartRateZones = 8,

    /// <summary>Heart-rate variability (HRV) data.</summary>
    HrvData = 9,

    /// <summary>Connected device and sensor information.</summary>
    Devices = 10,

    /// <summary>Timer, pause, and custom event timeline.</summary>
    Events = 11,

    /// <summary>Connect IQ / developer data fields.</summary>
    DeveloperFields = 12,

    /// <summary>Workout definition and steps.</summary>
    Workout = 13,

    /// <summary>Course route and course points.</summary>
    Course = 14,

    /// <summary>Segment lap comparison data.</summary>
    SegmentLaps = 15,

    /// <summary>User profile information.</summary>
    UserProfile = 16,

    /// <summary>Data quality and parse diagnostics.</summary>
    DataQuality = 17,
}
