using FluentAssertions;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Markdown.Projection;
using FitToMarkdown.Markdown.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Markdown.Tests;

public sealed class RecordSummaryProjectorTests
{
    [Fact]
    public void ProjectFromRecords_should_compute_statistics()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();
        var records = doc.ActivityContent!.Sessions[0].Records;

        var result = RecordSummaryProjector.ProjectFromRecords(records);

        result.Should().NotBeNull();
        result!.RecordCount.Should().Be(5);
        result.Metrics.Should().Contain(m => m.MetricKey == "heart_rate");
        result.Metrics.Should().Contain(m => m.MetricKey == "speed");

        var hrMetric = result.Metrics.First(m => m.MetricKey == "heart_rate");
        hrMetric.Minimum.Should().Be(140);
        hrMetric.Maximum.Should().Be(160);
        hrMetric.SampleCount.Should().Be(5);
    }

    [Fact]
    public void ProjectFromRecords_should_return_null_for_empty_records()
    {
        var result = RecordSummaryProjector.ProjectFromRecords([]);

        result.Should().BeNull();
    }

    [Fact]
    public void ProjectGlobal_should_aggregate_across_sessions()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();

        var result = RecordSummaryProjector.ProjectGlobal(doc);

        result.Should().NotBeNull();
        result!.RecordCount.Should().Be(5);
    }

    [Fact]
    public void ProjectForSession_should_scope_to_session_records()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();
        var session = doc.ActivityContent!.Sessions[0];

        var result = RecordSummaryProjector.ProjectForSession(session);

        result.Should().NotBeNull();
        result!.RecordCount.Should().Be(5);
    }

    [Fact]
    public void ProjectGlobal_should_return_null_for_no_activity()
    {
        var doc = MarkdownTestFixtures.CreateCourseDocument();

        var result = RecordSummaryProjector.ProjectGlobal(doc);

        result.Should().BeNull();
    }
}
