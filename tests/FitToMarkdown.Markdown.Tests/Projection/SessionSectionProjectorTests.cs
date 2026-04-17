using FluentAssertions;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Markdown.Projection;
using FitToMarkdown.Markdown.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Markdown.Tests.Projection;

public sealed class SessionSectionProjectorTests
{
    [Fact]
    public void Project_single_session_with_laps_should_produce_lap_rows()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();
        var opts = MarkdownTestFixtures.CreateDefaultOptions();

        var result = SessionSectionProjector.Project(doc, opts);

        result.Should().HaveCount(1);
        result[0].LapRows.Should().HaveCount(2);
        result[0].LapRows[0].LapNumber.Should().Be(1);
        result[0].LapRows[1].LapNumber.Should().Be(2);
    }

    [Fact]
    public void Project_multi_session_document_should_produce_multiple_sections()
    {
        var doc = MarkdownTestFixtures.CreateMultiSportActivity();
        var opts = MarkdownTestFixtures.CreateDefaultOptions();

        var result = SessionSectionProjector.Project(doc, opts);

        result.Should().HaveCount(3);
        result[0].Session.Sport.Should().Be(FitSport.Swimming);
        result[1].Session.Sport.Should().Be(FitSport.Cycling);
        result[2].Session.Sport.Should().Be(FitSport.Running);
    }

    [Fact]
    public void Project_pool_swim_session_should_produce_length_rows()
    {
        var doc = MarkdownTestFixtures.CreatePoolSwimActivity();
        var opts = MarkdownTestFixtures.CreateDefaultOptions();

        var result = SessionSectionProjector.Project(doc, opts);

        result.Should().HaveCount(1);
        result[0].Session.Sport.Should().Be(FitSport.Swimming);
        result[0].Session.SubSport.Should().Be(FitSubSport.LapSwimming);
    }

    [Fact]
    public void Project_session_with_no_laps_should_produce_empty_lap_collection()
    {
        var doc = MarkdownTestFixtures.CreateMinimalActivityDocument();
        var opts = MarkdownTestFixtures.CreateDefaultOptions();

        var result = SessionSectionProjector.Project(doc, opts);

        result.Should().HaveCount(1);
        result[0].LapRows.Should().BeEmpty();
    }

    [Fact]
    public void Project_single_lap_with_collapse_should_have_section_decisions()
    {
        var doc = MarkdownTestFixtures.CreateMinimalActivityDocument();
        var opts = MarkdownTestFixtures.CreateDefaultOptions() with { CollapseSingleLapSections = true };

        var result = SessionSectionProjector.Project(doc, opts);

        result.Should().HaveCount(1);
        result[0].SectionDecisions.Should().NotBeEmpty();
    }

    [Fact]
    public void Project_document_with_no_activity_should_return_empty()
    {
        var doc = MarkdownTestFixtures.CreateCourseDocument();
        var opts = MarkdownTestFixtures.CreateDefaultOptions();

        var result = SessionSectionProjector.Project(doc, opts);

        result.Should().BeEmpty();
    }
}
