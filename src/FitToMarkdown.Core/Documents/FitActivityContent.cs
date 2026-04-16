using FitToMarkdown.Core.Models;

namespace FitToMarkdown.Core.Documents;

/// <summary>
/// Represents the activity-family payload for an activity FIT file.
/// </summary>
public sealed record FitActivityContent
{
    /// <summary>The activity message, if present.</summary>
    public FitActivity? Activity { get; init; }

    /// <summary>User profile, if present in the file.</summary>
    public FitUserProfile? UserProfile { get; init; }

    /// <summary>Zones target, if present in the file.</summary>
    public FitZonesTarget? ZonesTarget { get; init; }

    /// <summary>Sport profiles parsed from the file.</summary>
    public IReadOnlyList<FitSportProfile> Sports { get; init; } = [];

    /// <summary>All sessions parsed from the activity file, grouped with their child messages.</summary>
    public IReadOnlyList<FitSession> Sessions { get; init; } = [];

    /// <summary>All event messages parsed from the file.</summary>
    public IReadOnlyList<FitEvent> Events { get; init; } = [];

    /// <summary>All HRV messages parsed from the file.</summary>
    public IReadOnlyList<FitHrv> HrvMessages { get; init; } = [];

    /// <summary>All compressed HR messages parsed from the file.</summary>
    public IReadOnlyList<FitHr> HrMessages { get; init; } = [];

    /// <summary>Segment laps parsed from the file.</summary>
    public IReadOnlyList<FitSegmentLap> SegmentLaps { get; init; } = [];

    /// <summary>ClimbPro messages parsed from the file.</summary>
    public IReadOnlyList<FitClimbPro> ClimbProMessages { get; init; } = [];

    /// <summary>Whether this activity contains multiple sport sessions.</summary>
    public bool IsMultiSport { get; init; }

    /// <summary>Whether this activity contains pool-swim data.</summary>
    public bool HasPoolSwim { get; init; }
}
