namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Core-defined enum for future FIT profile compatibility. Not present in SDK v1.0.1.
/// Models ClimbPro event types for climb detection features.
/// </summary>
public enum FitClimbProEvent : byte
{
    /// <summary>Approaching a climb.</summary>
    Approach = 0,

    /// <summary>Climb started.</summary>
    Start = 1,

    /// <summary>Climb completed.</summary>
    Complete = 2,

    /// <summary>Unknown or invalid ClimbPro event.</summary>
    Unknown = 255,
}
