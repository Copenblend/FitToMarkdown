using FluentAssertions;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Fit.Internal;
using Xunit;

namespace FitToMarkdown.Fit.Tests.Regression;

public sealed class IssueCodeConsistencyTests
{
    [Fact]
    public void InvalidHeader_HasCorrectCode()
    {
        var issue = FitParseIssueFactory.InvalidHeader();

        issue.Code.Should().Be("fit.invalid-header");
        issue.Severity.Should().Be(FitParseIssueSeverity.Error);
        issue.Recoverable.Should().BeFalse();
    }

    [Fact]
    public void DecodeFault_HasCorrectCode()
    {
        var issue = FitParseIssueFactory.DecodeFault("test message");

        issue.Code.Should().Be("fit.decode-fault");
        issue.Severity.Should().Be(FitParseIssueSeverity.Warning);
        issue.Recoverable.Should().BeTrue();
    }

    [Fact]
    public void IntegrityCheckFailed_HasCorrectCode()
    {
        var issue = FitParseIssueFactory.IntegrityCheckFailed();

        issue.Code.Should().Be("fit.integrity-failed");
    }

    [Fact]
    public void MissingFileId_HasCorrectCode()
    {
        var issue = FitParseIssueFactory.MissingFileId();

        issue.Code.Should().Be("fit.missing-file-id");
        issue.Severity.Should().Be(FitParseIssueSeverity.Error);
        issue.Recoverable.Should().BeFalse();
    }

    [Fact]
    public void SyntheticActivityCreated_HasCorrectCode()
    {
        var issue = FitParseIssueFactory.SyntheticActivityCreated();

        issue.Code.Should().Be("fit.synthetic-activity");
        issue.Severity.Should().Be(FitParseIssueSeverity.Warning);
        issue.Recoverable.Should().BeTrue();
    }

    [Fact]
    public void SyntheticSessionCreated_HasCorrectCode()
    {
        var issue = FitParseIssueFactory.SyntheticSessionCreated();

        issue.Code.Should().Be("fit.synthetic-session");
        issue.Severity.Should().Be(FitParseIssueSeverity.Warning);
        issue.Recoverable.Should().BeTrue();
    }

    [Fact]
    public void SyntheticLapCreated_HasCorrectCode()
    {
        var issue = FitParseIssueFactory.SyntheticLapCreated();

        issue.Code.Should().Be("fit.synthetic-lap");
    }

    [Fact]
    public void UnrecognizedFileType_HasCorrectCode()
    {
        var issue = FitParseIssueFactory.UnrecognizedFileType(99);

        issue.Code.Should().Be("fit.unknown-file-type");
    }

    [Fact]
    public void ChainedSegmentFault_HasCorrectCode()
    {
        var issue = FitParseIssueFactory.ChainedSegmentFault(1, "CRC mismatch in segment 1");

        issue.Code.Should().Be("fit.chained-segment-fault");
        issue.Severity.Should().Be(FitParseIssueSeverity.Warning);
        issue.Recoverable.Should().BeTrue();
    }

    [Fact]
    public void UnresolvedDeveloperField_HasCorrectCode()
    {
        var issue = FitParseIssueFactory.UnresolvedDeveloperField(0, 3);

        issue.Code.Should().Be("fit.unresolved-developer-field");
        issue.Severity.Should().Be(FitParseIssueSeverity.Warning);
        issue.Recoverable.Should().BeTrue();
    }

    [Fact]
    public void DuplicateDeveloperDefinition_HasCorrectCode()
    {
        var issue = FitParseIssueFactory.DuplicateDeveloperDefinition(0, 5);

        issue.Code.Should().Be("fit.duplicate-developer-definition");
        issue.Severity.Should().Be(FitParseIssueSeverity.Warning);
        issue.Recoverable.Should().BeTrue();
    }

    [Fact]
    public void GroupingAmbiguous_HasCorrectCode()
    {
        var issue = FitParseIssueFactory.GroupingAmbiguous("Multiple sessions with null timestamp");

        issue.Code.Should().Be("fit.grouping-ambiguous");
        issue.Severity.Should().Be(FitParseIssueSeverity.Warning);
        issue.Recoverable.Should().BeTrue();
    }

    [Fact]
    public void MonitoringIntegrityFailed_HasCorrectCode()
    {
        var issue = FitParseIssueFactory.MonitoringIntegrityFailed("CRC mismatch");

        issue.Code.Should().Be("fit.monitoring-integrity-failed");
        issue.Severity.Should().Be(FitParseIssueSeverity.Error);
        issue.Recoverable.Should().BeFalse();
    }

    [Fact]
    public void MetadataSummaryPartial_HasCorrectCode()
    {
        var issue = FitParseIssueFactory.MetadataSummaryPartial("Missing session data");

        issue.Code.Should().Be("fit.metadata-summary-partial");
        issue.Severity.Should().Be(FitParseIssueSeverity.Warning);
        issue.Recoverable.Should().BeTrue();
    }

    [Fact]
    public void PluginFallback_HasCorrectCode()
    {
        var issue = FitParseIssueFactory.PluginFallback("HRM-Tri merge skipped");

        issue.Code.Should().Be("fit.plugin-fallback");
        issue.Severity.Should().Be(FitParseIssueSeverity.Warning);
        issue.Recoverable.Should().BeTrue();
    }
}
