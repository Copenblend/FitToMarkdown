using FluentAssertions;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.ValueObjects;
using FitToMarkdown.Markdown.Projection;
using FitToMarkdown.Markdown.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Markdown.Tests.Projection;

public sealed class CourseProjectorTests
{
    [Fact]
    public void Project_course_document_should_produce_points()
    {
        var doc = MarkdownTestFixtures.CreateCourseDocument();
        doc = doc with
        {
            Course = doc.Course! with
            {
                Points =
                [
                    new FitCoursePoint
                    {
                        PointIndex = 0,
                        PointName = "Start",
                        PointType = FitCoursePointType.Generic,
                        DistanceMeters = 0,
                    },
                    new FitCoursePoint
                    {
                        PointIndex = 1,
                        PointName = "Water Stop",
                        PointType = FitCoursePointType.Water,
                        DistanceMeters = 5000,
                    },
                ],
            },
        };

        var result = CourseProjector.Project(doc);

        result.Should().HaveCount(2);
        result[0].PointName.Should().Be("Start");
        result[1].PointName.Should().Be("Water Stop");
    }

    [Fact]
    public void Project_non_course_document_should_return_empty()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();

        var result = CourseProjector.Project(doc);

        result.Should().BeEmpty();
    }

    [Fact]
    public void Project_course_with_points_should_produce_correct_count()
    {
        var doc = MarkdownTestFixtures.CreateCourseDocument();
        doc = doc with
        {
            Course = doc.Course! with
            {
                Points =
                [
                    new FitCoursePoint { PointIndex = 0, PointName = "A", DistanceMeters = 0 },
                    new FitCoursePoint { PointIndex = 1, PointName = "B", DistanceMeters = 2000 },
                    new FitCoursePoint { PointIndex = 2, PointName = "C", DistanceMeters = 5000 },
                ],
            },
        };

        var result = CourseProjector.Project(doc);

        result.Should().HaveCount(3);
        result[0].Order.Should().Be(1);
        result[2].Order.Should().Be(3);
    }
}
