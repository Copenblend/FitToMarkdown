namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Mirrors the Dynastream FIT profile DisplayMeasure enum for pool length units. Numeric values are preserved from the FIT SDK.
/// </summary>
public enum FitLengthUnit : byte
{
    /// <summary>Metric units (meters).</summary>
    Metric = 0,

    /// <summary>Statute units (yards).</summary>
    Statute = 1,

    /// <summary>Nautical units.</summary>
    Nautical = 2,

    /// <summary>Unknown or invalid length unit.</summary>
    Unknown = 255,
}
