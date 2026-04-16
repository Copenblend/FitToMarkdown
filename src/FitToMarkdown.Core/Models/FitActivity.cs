using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized ActivityMesg from a FIT file.
/// </summary>
public sealed record FitActivity
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>Activity timestamp in UTC.</summary>
    public DateTimeOffset? TimestampUtc { get; init; }

    /// <summary>Total elapsed timer time.</summary>
    public TimeSpan? TotalTimerTime { get; init; }

    /// <summary>Number of sessions in the activity.</summary>
    public ushort? NumberOfSessions { get; init; }

    /// <summary>Activity type.</summary>
    public FitActivityType? Type { get; init; }

    /// <summary>Event kind associated with the activity.</summary>
    public FitEventKind? Event { get; init; }

    /// <summary>Event action type.</summary>
    public FitEventAction? EventType { get; init; }

    /// <summary>Local timestamp from the device.</summary>
    public DateTimeOffset? LocalTimestamp { get; init; }

    /// <summary>Offset between local time and UTC.</summary>
    public TimeSpan? LocalTimeOffset { get; init; }
}
