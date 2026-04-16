namespace FitToMarkdown.Core.Summaries;

/// <summary>
/// Represents the summary statistics rendered in the HRV section.
/// </summary>
public sealed record HrvSummary
{
    /// <summary>Number of HRV samples.</summary>
    public int SampleCount { get; init; }

    /// <summary>Average RR interval in milliseconds.</summary>
    public double? AverageRrMilliseconds { get; init; }

    /// <summary>Root mean square of successive differences in milliseconds.</summary>
    public double? RmssdMilliseconds { get; init; }

    /// <summary>Standard deviation of NN intervals in milliseconds.</summary>
    public double? SdnnMilliseconds { get; init; }

    /// <summary>Minimum RR interval in milliseconds.</summary>
    public double? MinimumRrMilliseconds { get; init; }

    /// <summary>Maximum RR interval in milliseconds.</summary>
    public double? MaximumRrMilliseconds { get; init; }
}
