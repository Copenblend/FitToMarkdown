namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Stroke-count bucket names used by session and lap summaries.
/// </summary>
public enum FitStrokeType : byte
{
    /// <summary>Front crawl / freestyle stroke.</summary>
    Freestyle = 0,

    /// <summary>Backstroke.</summary>
    Backstroke = 1,

    /// <summary>Breaststroke.</summary>
    Breaststroke = 2,

    /// <summary>Butterfly stroke.</summary>
    Butterfly = 3,

    /// <summary>Drill or technique work, not a competitive stroke.</summary>
    Drill = 4,

    /// <summary>Mixed strokes within the same interval.</summary>
    Mixed = 5,

    /// <summary>Individual medley combining all four strokes.</summary>
    IndividualMedley = 6,

    /// <summary>Stroke type could not be determined.</summary>
    Unknown = 255,
}
