namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Mirrors the Dynastream FIT profile LengthType enum. Numeric values are preserved from the FIT SDK.
/// </summary>
public enum FitLengthType : byte
{
    /// <summary>Idle length (rest between active lengths).</summary>
    Idle = 0,

    /// <summary>Active length (swimming).</summary>
    Active = 1,

    /// <summary>Unknown or invalid length type.</summary>
    Unknown = 255,
}
