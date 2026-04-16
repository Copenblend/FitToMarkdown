namespace FitToMarkdown.Core.Summaries;

/// <summary>
/// Represents min, average, max, and standard deviation statistics over a record series.
/// </summary>
public sealed record RecordStatisticsSummary
{
    /// <summary>Total number of records in the series.</summary>
    public int RecordCount { get; init; }

    /// <summary>UTC timestamp of the first record.</summary>
    public DateTimeOffset? FirstTimestampUtc { get; init; }

    /// <summary>UTC timestamp of the last record.</summary>
    public DateTimeOffset? LastTimestampUtc { get; init; }

    /// <summary>Duration of the record series.</summary>
    public TimeSpan? Duration { get; init; }

    /// <summary>Statistics for each tracked metric.</summary>
    public IReadOnlyList<MetricStatistics> Metrics { get; init; } = [];
}
