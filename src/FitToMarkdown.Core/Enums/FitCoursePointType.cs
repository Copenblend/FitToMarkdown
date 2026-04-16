namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Mirrors the Dynastream FIT profile CoursePoint enum. Numeric values are preserved from the FIT SDK.
/// </summary>
public enum FitCoursePointType : byte
{
    /// <summary>Generic course point.</summary>
    Generic = 0,

    /// <summary>Summit.</summary>
    Summit = 1,

    /// <summary>Valley.</summary>
    Valley = 2,

    /// <summary>Water stop.</summary>
    Water = 3,

    /// <summary>Food stop.</summary>
    Food = 4,

    /// <summary>Danger zone.</summary>
    Danger = 5,

    /// <summary>Turn left.</summary>
    Left = 6,

    /// <summary>Turn right.</summary>
    Right = 7,

    /// <summary>Go straight.</summary>
    Straight = 8,

    /// <summary>First aid station.</summary>
    FirstAid = 9,

    /// <summary>Fourth category climb.</summary>
    FourthCategory = 10,

    /// <summary>Third category climb.</summary>
    ThirdCategory = 11,

    /// <summary>Second category climb.</summary>
    SecondCategory = 12,

    /// <summary>First category climb.</summary>
    FirstCategory = 13,

    /// <summary>Hors catégorie climb.</summary>
    HorsCategory = 14,

    /// <summary>Sprint point.</summary>
    Sprint = 15,

    /// <summary>Left fork.</summary>
    LeftFork = 16,

    /// <summary>Right fork.</summary>
    RightFork = 17,

    /// <summary>Middle fork.</summary>
    MiddleFork = 18,

    /// <summary>Slight left turn.</summary>
    SlightLeft = 19,

    /// <summary>Sharp left turn.</summary>
    SharpLeft = 20,

    /// <summary>Slight right turn.</summary>
    SlightRight = 21,

    /// <summary>Sharp right turn.</summary>
    SharpRight = 22,

    /// <summary>U-turn.</summary>
    UTurn = 23,

    /// <summary>Segment start.</summary>
    SegmentStart = 24,

    /// <summary>Segment end.</summary>
    SegmentEnd = 25,

    /// <summary>Unknown or invalid course point type.</summary>
    Unknown = 255,
}
