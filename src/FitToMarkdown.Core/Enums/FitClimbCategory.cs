namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Core-defined enum based on standard road cycling climb categorization. Not present in SDK v1.0.1.
/// Categories follow the Tour de France classification system.
/// </summary>
public enum FitClimbCategory : byte
{
    /// <summary>Uncategorized climb.</summary>
    Uncategorized = 0,

    /// <summary>Fourth category climb (easiest categorized).</summary>
    FourthCategory = 1,

    /// <summary>Third category climb.</summary>
    ThirdCategory = 2,

    /// <summary>Second category climb.</summary>
    SecondCategory = 3,

    /// <summary>First category climb.</summary>
    FirstCategory = 4,

    /// <summary>Hors catégorie climb (beyond categorization, most difficult).</summary>
    HorsCategory = 5,

    /// <summary>Unknown or invalid climb category.</summary>
    Unknown = 255,
}
