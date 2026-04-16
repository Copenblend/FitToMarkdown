namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Distinguishes which property family is populated on FitDeveloperFieldValue.
/// </summary>
public enum FitDeveloperValueKind : byte
{
    /// <summary>A floating-point numeric value.</summary>
    Numeric = 0,

    /// <summary>An integer numeric value.</summary>
    Integer = 1,

    /// <summary>A boolean value.</summary>
    Boolean = 2,

    /// <summary>A text / string value.</summary>
    Text = 3,

    /// <summary>An array of floating-point numeric values.</summary>
    NumericArray = 4,

    /// <summary>An array of integer values.</summary>
    IntegerArray = 5,

    /// <summary>An array of text / string values.</summary>
    TextArray = 6,

    /// <summary>Value kind could not be determined.</summary>
    Unknown = 255,
}
