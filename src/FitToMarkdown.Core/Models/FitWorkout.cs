using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized WorkoutMesg from a FIT file.
/// </summary>
public sealed record FitWorkout
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>Name of the workout.</summary>
    public string? WorkoutName { get; init; }

    /// <summary>Primary sport type.</summary>
    public FitSport? Sport { get; init; }

    /// <summary>Sub-sport type.</summary>
    public FitSubSport? SubSport { get; init; }

    /// <summary>Number of valid steps in the workout.</summary>
    public ushort? NumberOfValidSteps { get; init; }

    /// <summary>Ordered list of workout steps.</summary>
    public IReadOnlyList<FitWorkoutStep> Steps { get; init; } = [];

    /// <summary>Developer field values attached to this message.</summary>
    public IReadOnlyList<FitDeveloperFieldValue> DeveloperFields { get; init; } = [];
}
