namespace FitToMarkdown.Cli.Models;

/// <summary>
/// Represents the result of building or updating a Progression document.
/// </summary>
internal sealed record ProgressionBuildResult
{
    /// <summary>The sport name for the progression.</summary>
    internal required string SportName { get; init; }

    /// <summary>The output file path of the progression document.</summary>
    internal required string OutputPath { get; init; }

    /// <summary>The number of activities included in the progression.</summary>
    internal required int ActivityCount { get; init; }

    /// <summary>The number of new activities added (for add-to mode).</summary>
    internal int NewActivitiesAdded { get; init; }

    /// <summary>The total elapsed time for the build.</summary>
    internal required TimeSpan ElapsedTime { get; init; }

    /// <summary>Warning messages encountered during the build.</summary>
    internal IReadOnlyList<string> Warnings { get; init; } = [];

    /// <summary>Files that failed to parse.</summary>
    internal IReadOnlyList<string> FailedFiles { get; init; } = [];
}
