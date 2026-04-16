using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized HrvMesg from a FIT file.
/// </summary>
public sealed record FitHrv
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>R-R interval values in seconds.</summary>
    public IReadOnlyList<double> RrIntervalsSeconds { get; init; } = [];
}
