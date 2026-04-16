using FluentAssertions;
using FitToMarkdown.Markdown.Projection;
using FitToMarkdown.Markdown.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Markdown.Tests;

public sealed class OverviewProjectorTests
{
    [Fact]
    public void Project_should_build_running_title()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();

        var (title, headingTimestamp, overview) = OverviewProjector.Project(doc);

        title.Should().Contain("Running");
        title.Should().Contain("Street");
        headingTimestamp.Should().NotBeNull();
        overview.TotalDistanceMeters.Should().Be(6200);
    }

    [Fact]
    public void Project_should_aggregate_multi_session_metrics()
    {
        var doc = MarkdownTestFixtures.CreateMultiSportActivity();

        var (title, _, overview) = OverviewProjector.Project(doc);

        title.Should().Contain("Swimming");
        overview.TotalDistanceMeters.Should().Be(51500);
        overview.SessionCount.Should().Be(3);
        overview.IsMultiSport.Should().BeTrue();
    }

    [Fact]
    public void Project_should_handle_course_document()
    {
        var doc = MarkdownTestFixtures.CreateCourseDocument();

        var (title, headingTimestamp, overview) = OverviewProjector.Project(doc);

        title.Should().Contain("Course");
        title.Should().Contain("Park Loop");
        overview.TotalDistanceMeters.Should().BeNull();
    }

    [Fact]
    public void Project_should_use_file_created_timestamp_as_fallback()
    {
        var doc = MarkdownTestFixtures.CreateCourseDocument();

        var (_, headingTimestamp, _) = OverviewProjector.Project(doc);

        headingTimestamp.Should().Be(doc.FileId!.TimeCreatedUtc);
    }
}
