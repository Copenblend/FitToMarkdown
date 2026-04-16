using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized LapMesg from a FIT file.
/// </summary>
public sealed record FitLap
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>Zero-based lap index within the activity.</summary>
    public int LapIndex { get; init; }

    /// <summary>Zero-based session index this lap belongs to.</summary>
    public int ParentSessionIndex { get; init; }

    /// <summary>Start and end time range for this lap.</summary>
    public FitTimeRange Range { get; init; } = new();

    /// <summary>Event kind associated with this lap.</summary>
    public FitEventKind? Event { get; init; }

    /// <summary>Event action type.</summary>
    public FitEventAction? EventType { get; init; }

    /// <summary>Lap trigger type.</summary>
    public FitLapTrigger? LapTrigger { get; init; }

    /// <summary>Workout step index associated with this lap.</summary>
    public ushort? WorkoutStepIndex { get; init; }

    /// <summary>Starting position coordinate.</summary>
    public GeoCoordinate? StartPosition { get; init; }

    /// <summary>Ending position coordinate.</summary>
    public GeoCoordinate? EndPosition { get; init; }

    /// <summary>Geographic bounding box of the lap.</summary>
    public GeoBounds? Bounds { get; init; }

    /// <summary>Aggregated summary metrics for the lap.</summary>
    public FitSummaryMetrics Metrics { get; init; } = new();

    /// <summary>Developer field values attached to this message.</summary>
    public IReadOnlyList<FitDeveloperFieldValue> DeveloperFields { get; init; } = [];
}
