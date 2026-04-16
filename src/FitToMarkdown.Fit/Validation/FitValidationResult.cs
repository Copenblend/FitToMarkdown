using FitToMarkdown.Core.Parsing;

namespace FitToMarkdown.Fit.Validation;

/// <summary>
/// Represents the outcome of snapshot validation with accumulated issues.
/// </summary>
internal sealed class FitValidationResult
{
    /// <summary>Whether validation passed without any blocking errors.</summary>
    public bool IsValid { get; init; }

    /// <summary>The collection of issues detected during validation.</summary>
    public IReadOnlyList<FitParseIssue> Issues { get; init; } = [];
}
