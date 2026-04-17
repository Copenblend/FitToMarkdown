using FitToMarkdown.Core.Enums;

namespace FitToMarkdown.Fit.Tests.Fixtures;

/// <summary>
/// Describes one canonical real FIT sample and the parser invariants the sample is expected to exercise.
/// </summary>
internal sealed record FitSampleDefinition(
    string Key,
    string RelativePath,
    string Scenario,
    string Category,
    FitParseStatus ExpectedStatus,
    FitMessageOrderingMode ExpectedOrderingMode,
    bool ExpectsSyntheticActivity,
    bool ExpectsSyntheticSessions);
