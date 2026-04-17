using FluentAssertions;
using FitToMarkdown.Markdown.Projection;
using FitToMarkdown.Markdown.Tests.Fixtures.Core;
using FitToMarkdown.Markdown.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Markdown.Tests.Projection;

public sealed class ProjectionDeterminismTests
{
    [Fact]
    public void Same_input_projected_twice_should_produce_identical_documents()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();
        var opts = MarkdownTestFixtures.CreateDefaultOptions();
        var coordinator = new DocumentProjectionCoordinator();

        var first = coordinator.Project(doc, opts, CancellationToken.None);
        var second = coordinator.Project(doc, opts, CancellationToken.None);

        first.Title.Should().Be(second.Title);
        first.HeadingTimestampUtc.Should().Be(second.HeadingTimestampUtc);
        first.SessionSections.Should().HaveCount(second.SessionSections.Count);
        first.SectionDecisions.Should().HaveCount(second.SectionDecisions.Count);
    }

    [Fact]
    public void Shuffled_records_should_produce_consistent_projection()
    {
        var doc = DeterministicOrderFixtureFactory.CreateShuffledRecords();
        var opts = MarkdownTestFixtures.CreateDefaultOptions();
        var coordinator = new DocumentProjectionCoordinator();

        var first = coordinator.Project(doc, opts, CancellationToken.None);
        var second = coordinator.Project(doc, opts, CancellationToken.None);

        first.Title.Should().Be(second.Title);
        first.SessionSections.Should().HaveCount(second.SessionSections.Count);
    }

    [Fact]
    public void Different_options_should_produce_predictable_differences()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();
        var coordinator = new DocumentProjectionCoordinator();

        var withFrontmatter = coordinator.Project(doc,
            MarkdownTestFixtures.CreateDefaultOptions() with { IncludeFrontmatter = true },
            CancellationToken.None);

        var withoutFrontmatter = coordinator.Project(doc,
            MarkdownTestFixtures.CreateDefaultOptions() with { IncludeFrontmatter = false },
            CancellationToken.None);

        var fmDecisionWith = withFrontmatter.SectionDecisions
            .First(d => d.Section == Core.Enums.FitMarkdownSectionKey.Frontmatter);
        var fmDecisionWithout = withoutFrontmatter.SectionDecisions
            .First(d => d.Section == Core.Enums.FitMarkdownSectionKey.Frontmatter);

        fmDecisionWith.ShouldRender.Should().BeTrue();
        fmDecisionWithout.ShouldRender.Should().BeFalse();
    }
}
