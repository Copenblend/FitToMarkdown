using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Parsing;
using FitToMarkdown.Fit.Decoding;
using FitToMarkdown.Fit.Internal;

namespace FitToMarkdown.Fit.Validation;

/// <summary>
/// Applies configurable validation rules to a decoded FIT snapshot.
/// </summary>
internal sealed class FitValidationPolicy
{
    /// <summary>
    /// Validates the decoded snapshot against configured rules and parse options.
    /// </summary>
    /// <param name="snapshot">The decoded snapshot to validate.</param>
    /// <param name="options">The parse options governing validation strictness.</param>
    /// <returns>The validation result with any issues found.</returns>
    public FitValidationResult Validate(FitDecodeSnapshot snapshot, FitParseOptions options)
    {
        var issues = new List<FitParseIssue>();

        // Rule 1: Missing FileId
        if (snapshot.FileIdMesgs.Count == 0)
        {
            issues.Add(FitParseIssueFactory.MissingFileId());
        }

        // Rule 2: Truncation / decode fault detection
        if (snapshot.HadDecodeFault)
        {
            var fileType = FitFileTypeClassifier.Classify(snapshot);
            bool isActivity = fileType is FitFileType.Activity or FitFileType.ActivitySummary or null;

            if (options.ValidateIntegrityForNonActivityFiles && !isActivity)
            {
                // Non-activity files with decode faults: Error (not recoverable)
                issues.Add(new FitParseIssue
                {
                    Severity = FitParseIssueSeverity.Error,
                    Code = "FIT_DECODE_FAULT",
                    Message = $"Decode fault in non-activity file: {snapshot.DecodeFaultMessage}",
                    Recoverable = false,
                });
            }
            else
            {
                // Activity files: Warning (recoverable with AllowPartialExtraction)
                issues.Add(FitParseIssueFactory.DecodeFault(snapshot.DecodeFaultMessage ?? "Unknown decode fault"));
            }
        }

        bool hasBlockingError = issues.Any(i => i.Severity == FitParseIssueSeverity.Error);

        return new FitValidationResult
        {
            IsValid = !hasBlockingError,
            Issues = issues,
        };
    }
}
