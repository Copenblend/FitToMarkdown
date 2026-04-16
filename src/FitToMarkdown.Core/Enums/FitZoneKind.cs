namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Heart-rate and training-zone bucket names.
/// </summary>
public enum FitZoneKind : byte
{
    /// <summary>Zone 1 – recovery / warm-up intensity.</summary>
    Zone1 = 0,

    /// <summary>Zone 2 – easy / aerobic base.</summary>
    Zone2 = 1,

    /// <summary>Zone 3 – moderate / tempo effort.</summary>
    Zone3 = 2,

    /// <summary>Zone 4 – hard / threshold effort.</summary>
    Zone4 = 3,

    /// <summary>Zone 5 – maximum / anaerobic effort.</summary>
    Zone5 = 4,

    /// <summary>Zone could not be determined.</summary>
    Unknown = 255,
}
