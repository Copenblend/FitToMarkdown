using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Documents;

/// <summary>
/// Represents the monitoring-family payload for a monitoring FIT file.
/// </summary>
public sealed record FitMonitoringContent
{
    /// <summary>Time range covering all monitoring samples.</summary>
    public FitTimeRange Range { get; init; } = new();

    /// <summary>All monitoring messages parsed from the file.</summary>
    public IReadOnlyList<FitMonitoring> Samples { get; init; } = [];

    /// <summary>Whether the monitoring content contains a daily summary.</summary>
    public bool ContainsDailySummary { get; init; }
}
