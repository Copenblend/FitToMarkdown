namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Mirrors the Dynastream FIT profile EventType enum. Numeric values are preserved from the FIT SDK.
/// </summary>
public enum FitEventAction : byte
{
    /// <summary>Start event.</summary>
    Start = 0,

    /// <summary>Stop event.</summary>
    Stop = 1,

    /// <summary>Consecutive (deprecated).</summary>
    ConsecutiveDeprecated = 2,

    /// <summary>Marker event.</summary>
    Marker = 3,

    /// <summary>Stop all event.</summary>
    StopAll = 4,

    /// <summary>Begin (deprecated).</summary>
    BeginDeprecated = 5,

    /// <summary>End (deprecated).</summary>
    EndDeprecated = 6,

    /// <summary>End all (deprecated).</summary>
    EndAllDeprecated = 7,

    /// <summary>Stop and disable event.</summary>
    StopDisable = 8,

    /// <summary>Stop and disable all events.</summary>
    StopDisableAll = 9,

    /// <summary>Unknown or invalid event action.</summary>
    Unknown = 255,
}
