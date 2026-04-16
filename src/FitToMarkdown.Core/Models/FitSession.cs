using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized SessionMesg and its grouped child messages.
/// </summary>
public sealed record FitSession
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>Zero-based session index within the activity.</summary>
    public int SessionIndex { get; init; }

    /// <summary>Start and end time range for this session.</summary>
    public FitTimeRange Range { get; init; } = new();

    /// <summary>Primary sport type.</summary>
    public FitSport? Sport { get; init; }

    /// <summary>Sub-sport type.</summary>
    public FitSubSport? SubSport { get; init; }

    /// <summary>Event kind associated with this session.</summary>
    public FitEventKind? Event { get; init; }

    /// <summary>Event action type.</summary>
    public FitEventAction? EventType { get; init; }

    /// <summary>Session trigger type.</summary>
    public FitSessionTrigger? Trigger { get; init; }

    /// <summary>Sport profile name from the device.</summary>
    public string? SportProfileName { get; init; }

    /// <summary>Whether this session represents a transition between sports.</summary>
    public bool IsTransition { get; init; }

    /// <summary>Starting position coordinate.</summary>
    public GeoCoordinate? StartPosition { get; init; }

    /// <summary>Geographic bounding box of the session.</summary>
    public GeoBounds? Bounds { get; init; }

    /// <summary>Aggregated summary metrics for the session.</summary>
    public FitSummaryMetrics Metrics { get; init; } = new();

    /// <summary>Laps belonging to this session.</summary>
    public IReadOnlyList<FitLap> Laps { get; init; } = [];

    /// <summary>Lengths belonging to this session.</summary>
    public IReadOnlyList<FitLength> Lengths { get; init; } = [];

    /// <summary>Records belonging to this session.</summary>
    public IReadOnlyList<FitRecord> Records { get; init; } = [];

    /// <summary>Developer field values attached to this message.</summary>
    public IReadOnlyList<FitDeveloperFieldValue> DeveloperFields { get; init; } = [];
}
