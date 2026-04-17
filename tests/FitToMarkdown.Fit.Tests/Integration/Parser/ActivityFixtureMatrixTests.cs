using FitToMarkdown.Fit.Tests.Fixtures;
using Xunit;

namespace FitToMarkdown.Fit.Tests.Integration.Parser;

public sealed class ActivityFixtureMatrixTests
{
    [Fact(Skip = "Requires real FIT test data files")]
    public void Clean_activity_parses_successfully()
    {
        // Will be enabled when TestData is populated
        var sample = FitSampleCatalog.Get("activity-clean");
        var path = FitSampleCatalog.GetAbsolutePath(sample);
        // Parse and assert...
    }

    [Fact(Skip = "Requires real FIT test data files")]
    public void Summary_first_activity_parses_successfully()
    {
        var sample = FitSampleCatalog.Get("activity-summary-first");
        var path = FitSampleCatalog.GetAbsolutePath(sample);
        // Parse and assert...
    }

    [Fact(Skip = "Requires real FIT test data files")]
    public void Truncated_recoverable_activity_parses_partially()
    {
        var sample = FitSampleCatalog.Get("truncated-recoverable");
        var path = FitSampleCatalog.GetAbsolutePath(sample);
        // Parse and assert partial success...
    }

    [Fact(Skip = "Requires real FIT test data files")]
    public void Truncated_unrecoverable_activity_fails()
    {
        var sample = FitSampleCatalog.Get("truncated-unrecoverable");
        var path = FitSampleCatalog.GetAbsolutePath(sample);
        // Parse and assert failure...
    }
}
