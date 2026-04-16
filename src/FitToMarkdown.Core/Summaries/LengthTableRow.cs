using FitToMarkdown.Core.Enums;

namespace FitToMarkdown.Core.Summaries;

/// <summary>
/// Represents one row in the pool-swim length table.
/// </summary>
public sealed record LengthTableRow
{
    /// <summary>One-based length number.</summary>
    public int LengthNumber { get; init; }

    /// <summary>Type of the swim length.</summary>
    public FitLengthType? LengthType { get; init; }

    /// <summary>Swim stroke used for this length.</summary>
    public FitSwimStroke? SwimStroke { get; init; }

    /// <summary>Duration of the length.</summary>
    public TimeSpan? Duration { get; init; }

    /// <summary>Total number of strokes.</summary>
    public ushort? TotalStrokes { get; init; }

    /// <summary>SWOLF score for the length.</summary>
    public ushort? Swolf { get; init; }

    /// <summary>Pace in seconds per 100 meters.</summary>
    public double? PaceSecondsPer100Meters { get; init; }

    /// <summary>Distance per stroke in meters.</summary>
    public double? DistancePerStrokeMeters { get; init; }

    /// <summary>One-based parent lap number this length belongs to.</summary>
    public int? ParentLapNumber { get; init; }
}
