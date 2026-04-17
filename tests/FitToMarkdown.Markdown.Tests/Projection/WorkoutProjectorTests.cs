using FluentAssertions;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Markdown.Projection;
using FitToMarkdown.Markdown.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Markdown.Tests.Projection;

public sealed class WorkoutProjectorTests
{
    [Fact]
    public void Project_workout_document_should_produce_steps()
    {
        var doc = MarkdownTestFixtures.CreateWorkoutDocument();
        doc = doc with
        {
            Workout = doc.Workout! with
            {
                Steps =
                [
                    new FitWorkoutStep { StepIndex = 0, StepName = "Warm Up" },
                    new FitWorkoutStep { StepIndex = 1, StepName = "Tempo" },
                    new FitWorkoutStep { StepIndex = 2, StepName = "Cool Down" },
                ],
            },
        };

        var result = WorkoutProjector.Project(doc);

        result.Should().HaveCount(3);
        result[0].StepName.Should().Be("Warm Up");
        result[1].StepName.Should().Be("Tempo");
        result[2].StepName.Should().Be("Cool Down");
    }

    [Fact]
    public void Project_non_workout_document_should_return_empty()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();

        var result = WorkoutProjector.Project(doc);

        result.Should().BeEmpty();
    }

    [Fact]
    public void Project_workout_with_steps_should_produce_correct_count()
    {
        var doc = MarkdownTestFixtures.CreateWorkoutDocument();
        doc = doc with
        {
            Workout = doc.Workout! with
            {
                Steps =
                [
                    new FitWorkoutStep { StepIndex = 0, StepName = "Run" },
                    new FitWorkoutStep { StepIndex = 1, StepName = "Walk" },
                ],
            },
        };

        var result = WorkoutProjector.Project(doc);

        result.Should().HaveCount(2);
        result[0].Order.Should().Be(1);
        result[1].Order.Should().Be(2);
    }
}
