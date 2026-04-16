namespace FitToMarkdown.Core.ValueObjects;

/// <summary>
/// Represents normalized left/right balance percentages.
/// </summary>
public sealed record FitLeftRightBalance
{
    /// <summary>The left-side percentage of the balance.</summary>
    public double? LeftPercent { get; init; }

    /// <summary>The right-side percentage of the balance.</summary>
    public double? RightPercent { get; init; }

    /// <summary>Indicates whether the right side is the reference for balance calculation.</summary>
    public bool UsesRightReference { get; init; }
}
