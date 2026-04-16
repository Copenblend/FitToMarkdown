namespace FitToMarkdown.Core.Summaries;

/// <summary>
/// Represents one derived pause interval.
/// </summary>
public sealed record PauseInterval
{
    /// <summary>Display order of this pause interval.</summary>
    public int Order { get; init; }

    /// <summary>UTC timestamp when the pause started.</summary>
    public DateTimeOffset StartUtc { get; init; }

    /// <summary>UTC timestamp when the pause ended.</summary>
    public DateTimeOffset EndUtc { get; init; }

    /// <summary>Duration of the pause.</summary>
    public TimeSpan Duration { get; init; }
}
