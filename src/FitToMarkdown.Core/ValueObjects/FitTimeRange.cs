namespace FitToMarkdown.Core.ValueObjects;

/// <summary>
/// Represents start, end, and duration values for a session, lap, length, or monitoring span.
/// </summary>
public sealed record FitTimeRange
{
    /// <summary>The UTC start time of the span.</summary>
    public DateTimeOffset? StartTimeUtc { get; init; }

    /// <summary>The UTC end time of the span.</summary>
    public DateTimeOffset? EndTimeUtc { get; init; }

    /// <summary>Total elapsed time including pauses.</summary>
    public TimeSpan? TotalElapsedTime { get; init; }

    /// <summary>Total timer time excluding pauses.</summary>
    public TimeSpan? TotalTimerTime { get; init; }

    /// <summary>Total time spent moving.</summary>
    public TimeSpan? TotalMovingTime { get; init; }
}
