using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized SportMesg from a FIT file.
/// </summary>
public sealed record FitSportProfile
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>Primary sport type.</summary>
    public FitSport? Sport { get; init; }

    /// <summary>Sub-sport type.</summary>
    public FitSubSport? SubSport { get; init; }

    /// <summary>Sport profile name.</summary>
    public string? Name { get; init; }
}
