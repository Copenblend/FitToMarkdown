namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Mirrors the Dynastream FIT profile SwimStroke enum. Numeric values are preserved from the FIT SDK.
/// </summary>
public enum FitSwimStroke : byte
{
    /// <summary>Freestyle stroke.</summary>
    Freestyle = 0,

    /// <summary>Backstroke.</summary>
    Backstroke = 1,

    /// <summary>Breaststroke.</summary>
    Breaststroke = 2,

    /// <summary>Butterfly stroke.</summary>
    Butterfly = 3,

    /// <summary>Drill.</summary>
    Drill = 4,

    /// <summary>Mixed strokes.</summary>
    Mixed = 5,

    /// <summary>Individual medley (IM).</summary>
    Im = 6,

    /// <summary>Unknown or invalid swim stroke.</summary>
    Unknown = 255,
}
