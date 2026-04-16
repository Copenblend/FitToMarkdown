using FitToMarkdown.Core.Enums;

namespace FitToMarkdown.Core.Summaries;

/// <summary>
/// Represents one markdown-ready event timeline item.
/// </summary>
public sealed record EventTimelineItem
{
    /// <summary>Display order of this event.</summary>
    public int Order { get; init; }

    /// <summary>UTC timestamp when the event occurred.</summary>
    public DateTimeOffset? TimestampUtc { get; init; }

    /// <summary>Kind of event.</summary>
    public FitEventKind? Event { get; init; }

    /// <summary>Action type of the event.</summary>
    public FitEventAction? EventType { get; init; }

    /// <summary>Human-readable detail text for the event.</summary>
    public string? DetailText { get; init; }

    /// <summary>Whether this event is considered significant for display.</summary>
    public bool IsSignificant { get; init; }

    /// <summary>Zero-based session index this event belongs to.</summary>
    public int? SessionIndex { get; init; }
}
