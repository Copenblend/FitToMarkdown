using FluentAssertions;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Markdown.Projection;
using FitToMarkdown.Markdown.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Markdown.Tests;

public sealed class FrontmatterProjectorTests
{
    [Fact]
    public void Project_should_extract_file_metadata()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();

        var fm = FrontmatterProjector.Project(doc);

        fm.FileType.Should().Be(FitFileType.Activity);
        fm.ManufacturerName.Should().Be("Garmin");
        fm.ProductName.Should().Be("Forerunner 265");
        fm.SerialNumber.Should().Be(345678901u);
        fm.TimeCreatedUtc.Should().NotBeNull();
    }

    [Fact]
    public void Project_should_extract_session_metrics()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();

        var fm = FrontmatterProjector.Project(doc);

        fm.Sport.Should().Be(FitSport.Running);
        fm.SubSport.Should().Be(FitSubSport.Street);
        fm.DistanceMeters.Should().Be(6200);
        fm.AverageHeartRateBpm.Should().Be(152);
        fm.MaximumHeartRateBpm.Should().Be(165);
        fm.TotalCalories.Should().Be(280);
        fm.SessionCount.Should().Be(1);
        fm.LapCount.Should().Be(2);
        fm.RecordCount.Should().Be(5);
    }

    [Fact]
    public void Project_should_handle_document_with_no_sessions()
    {
        var doc = MarkdownTestFixtures.CreateCourseDocument();

        var fm = FrontmatterProjector.Project(doc);

        fm.FileType.Should().Be(FitFileType.Course);
        fm.Sport.Should().Be(FitSport.Running);
        fm.SessionCount.Should().Be(0);
        fm.DistanceMeters.Should().BeNull();
    }

    [Fact]
    public void Project_should_extract_pool_length()
    {
        var doc = MarkdownTestFixtures.CreatePoolSwimActivity();

        var fm = FrontmatterProjector.Project(doc);

        fm.PoolLengthMeters.Should().Be(25.0);
        fm.Sport.Should().Be(FitSport.Swimming);
    }
}
