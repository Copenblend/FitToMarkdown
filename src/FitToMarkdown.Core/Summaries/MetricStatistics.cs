namespace FitToMarkdown.Core.Summaries;

/// <summary>
/// Represents one metric row within a record summary table.
/// </summary>
public sealed record MetricStatistics
{
    /// <summary>Unique key identifying the metric.</summary>
    public string MetricKey { get; init; } = string.Empty;

    /// <summary>Human-readable display name for the metric.</summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>Unit symbol for the metric values.</summary>
    public string UnitSymbol { get; init; } = string.Empty;

    /// <summary>Number of non-null samples for this metric.</summary>
    public int SampleCount { get; init; }

    /// <summary>Minimum observed value.</summary>
    public double? Minimum { get; init; }

    /// <summary>Average observed value.</summary>
    public double? Average { get; init; }

    /// <summary>Maximum observed value.</summary>
    public double? Maximum { get; init; }

    /// <summary>Standard deviation of observed values.</summary>
    public double? StandardDeviation { get; init; }
}
