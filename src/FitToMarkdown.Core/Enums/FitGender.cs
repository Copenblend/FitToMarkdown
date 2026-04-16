namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Mirrors the Dynastream FIT profile Gender enum. Numeric values are preserved from the FIT SDK.
/// </summary>
public enum FitGender : byte
{
    /// <summary>Female.</summary>
    Female = 0,

    /// <summary>Male.</summary>
    Male = 1,

    /// <summary>Unknown or invalid gender.</summary>
    Unknown = 255,
}
