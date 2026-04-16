using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized WorkoutStepMesg from a FIT file.
/// </summary>
public sealed record FitWorkoutStep
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>Zero-based step index within the workout.</summary>
    public int StepIndex { get; init; }

    /// <summary>Name of the workout step.</summary>
    public string? StepName { get; init; }

    /// <summary>Duration type for this step.</summary>
    public FitWorkoutStepDurationType? DurationType { get; init; }

    /// <summary>Duration value associated with the duration type.</summary>
    public double? DurationValue { get; init; }

    /// <summary>Target type for this step.</summary>
    public FitWorkoutTargetType? TargetType { get; init; }

    /// <summary>Target value range for this step.</summary>
    public WorkoutTargetRange? TargetRange { get; init; }

    /// <summary>Step intensity level.</summary>
    public FitIntensity? Intensity { get; init; }

    /// <summary>Freeform notes for this step.</summary>
    public string? Notes { get; init; }

    /// <summary>Equipment descriptor for this step.</summary>
    public string? Equipment { get; init; }

    /// <summary>Developer field values attached to this message.</summary>
    public IReadOnlyList<FitDeveloperFieldValue> DeveloperFields { get; init; } = [];
}
