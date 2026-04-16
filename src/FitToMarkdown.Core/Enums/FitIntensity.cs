namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Mirrors the Dynastream FIT profile Intensity enum. Numeric values are preserved from the FIT SDK.
/// </summary>
public enum FitIntensity : byte
{
    /// <summary>Active intensity.</summary>
    Active = 0,

    /// <summary>Rest intensity.</summary>
    Rest = 1,

    /// <summary>Warm-up intensity.</summary>
    Warmup = 2,

    /// <summary>Cooldown intensity.</summary>
    Cooldown = 3,

    /// <summary>Unknown or invalid intensity.</summary>
    Unknown = 255,
}
