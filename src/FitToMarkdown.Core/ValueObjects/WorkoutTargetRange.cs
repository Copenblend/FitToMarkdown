namespace FitToMarkdown.Core.ValueObjects;

/// <summary>
/// Represents normalized workout target values.
/// </summary>
public sealed record WorkoutTargetRange
{
    /// <summary>The primary target value.</summary>
    public double? TargetValue { get; init; }

    /// <summary>The lower bound of the custom target range.</summary>
    public double? CustomLow { get; init; }

    /// <summary>The upper bound of the custom target range.</summary>
    public double? CustomHigh { get; init; }

    /// <summary>The unit of measurement for the target values.</summary>
    public string? Unit { get; init; }
}
