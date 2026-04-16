using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Summaries;

/// <summary>
/// Represents one markdown-ready workout step row.
/// </summary>
public sealed record WorkoutStepRow
{
    /// <summary>Display order of this workout step.</summary>
    public int Order { get; init; }

    /// <summary>Name of the workout step.</summary>
    public string? StepName { get; init; }

    /// <summary>Duration type for the step.</summary>
    public FitWorkoutStepDurationType? DurationType { get; init; }

    /// <summary>Duration value associated with the duration type.</summary>
    public double? DurationValue { get; init; }

    /// <summary>Target type for the step.</summary>
    public FitWorkoutTargetType? TargetType { get; init; }

    /// <summary>Target range values for the step.</summary>
    public WorkoutTargetRange? TargetRange { get; init; }

    /// <summary>Intensity level of the step.</summary>
    public FitIntensity? Intensity { get; init; }

    /// <summary>Additional notes for the step.</summary>
    public string? Notes { get; init; }

    /// <summary>Equipment required for the step.</summary>
    public string? Equipment { get; init; }
}
