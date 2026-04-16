namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Mirrors the Dynastream FIT profile BatteryStatus constants. The SDK defines this as a static class
/// with byte constants, but Core models it as an enum. Numeric values are preserved from the FIT SDK.
/// </summary>
public enum FitBatteryStatus : byte
{
    /// <summary>New battery.</summary>
    New = 1,

    /// <summary>Good battery level.</summary>
    Good = 2,

    /// <summary>OK battery level.</summary>
    Ok = 3,

    /// <summary>Low battery level.</summary>
    Low = 4,

    /// <summary>Critical battery level.</summary>
    Critical = 5,

    /// <summary>Battery is charging.</summary>
    Charging = 6,

    /// <summary>Unknown or invalid battery status.</summary>
    Unknown = 7,
}
