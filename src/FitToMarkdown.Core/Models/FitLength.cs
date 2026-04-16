using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized LengthMesg from a FIT file.
/// </summary>
public sealed record FitLength
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>Zero-based length index within the activity.</summary>
    public int LengthIndex { get; init; }

    /// <summary>Session index this length was assigned to.</summary>
    public int? SessionIndex { get; init; }

    /// <summary>Zero-based lap index this length belongs to.</summary>
    public int? ParentLapIndex { get; init; }

    /// <summary>Start and end time range for this length.</summary>
    public FitTimeRange Range { get; init; } = new();

    /// <summary>Length type (active or idle).</summary>
    public FitLengthType? LengthType { get; init; }

    /// <summary>Swim stroke type.</summary>
    public FitSwimStroke? SwimStroke { get; init; }

    /// <summary>Total number of strokes in this length.</summary>
    public ushort? TotalStrokes { get; init; }

    /// <summary>Average speed in meters per second.</summary>
    public double? AverageSpeedMetersPerSecond { get; init; }

    /// <summary>Average swimming cadence in strokes per minute.</summary>
    public double? AverageSwimmingCadenceSpm { get; init; }

    /// <summary>Total calories for this length.</summary>
    public ushort? TotalCalories { get; init; }

    /// <summary>Event kind associated with this length.</summary>
    public FitEventKind? Event { get; init; }

    /// <summary>Event action type.</summary>
    public FitEventAction? EventType { get; init; }

    /// <summary>Developer field values attached to this message.</summary>
    public IReadOnlyList<FitDeveloperFieldValue> DeveloperFields { get; init; } = [];
}
