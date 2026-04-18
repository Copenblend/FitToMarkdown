namespace FitToMarkdown.Cli.Models;

/// <summary>
/// Represents a group of FIT files sharing the same sport and sub-sport combination.
/// </summary>
internal sealed record SportGroup
{
    /// <summary>The primary sport name as reported by FIT metadata.</summary>
    internal required string SportName { get; init; }

    /// <summary>The sub-sport name, or null if none.</summary>
    internal string? SubSportName { get; init; }

    /// <summary>Display label in the format "Sport (SubSport)" or just "Sport".</summary>
    internal string DisplayName => SubSportName is not null
        ? $"{SportName} ({SubSportName})"
        : SportName;

    /// <summary>The FIT files associated with this sport/sub-sport, ordered by timestamp.</summary>
    internal required IReadOnlyList<DiscoveredFitFile> Files { get; init; }
}
