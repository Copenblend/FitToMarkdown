using FluentAssertions;
using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Summaries;
using FitToMarkdown.Markdown.Projection;
using FitToMarkdown.Markdown.Rendering;
using FitToMarkdown.Markdown.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Markdown.Tests;

public sealed class MarkdownRenderCoordinatorTests
{
    private static FitMarkdownDocument BuildRenderableDocument(bool includeFrontmatter = true)
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();
        var fm = FrontmatterProjector.Project(doc);
        var (title, headingTs, overview) = OverviewProjector.Project(doc);
        var options = MarkdownTestFixtures.CreateDefaultOptions() with { IncludeFrontmatter = includeFrontmatter };

        var sessionSection = new SessionSection
        {
            Session = doc.ActivityContent!.Sessions[0],
            LapRows =
            [
                new LapTableRow { LapNumber = 1, DistanceMeters = 3000, Duration = TimeSpan.FromMinutes(14) },
                new LapTableRow { LapNumber = 2, DistanceMeters = 3200, Duration = TimeSpan.FromMinutes(14) },
            ],
        };

        var sessions = new List<SessionSection> { sessionSection };
        var docDecisions = SectionDecisionProjector.ProjectDocumentLevel(
            options, doc, fm, overview, sessions, null, [], null, null, [], [], [], [], [], []);

        var sessionDecisions = SectionDecisionProjector.ProjectSessionLevel(options, sessionSection, 0);
        sessionSection = sessionSection with { SectionDecisions = sessionDecisions };
        sessions = [sessionSection];

        return new FitMarkdownDocument
        {
            Source = doc,
            Title = title,
            HeadingTimestampUtc = headingTs,
            Frontmatter = fm,
            Overview = overview,
            SessionSections = sessions,
            SectionDecisions = docDecisions,
        };
    }

    [Fact]
    public void Render_should_produce_valid_markdown_structure()
    {
        var coordinator = new MarkdownRenderCoordinator();
        var mdDoc = BuildRenderableDocument();

        var markdown = coordinator.Render(mdDoc, CancellationToken.None);

        markdown.Should().NotBeNullOrWhiteSpace();
        // Should have exactly one H1
        markdown.Split('\n').Count(l => l.StartsWith("# ")).Should().Be(1);
    }

    [Fact]
    public void Render_should_include_frontmatter_when_decision_allows()
    {
        var coordinator = new MarkdownRenderCoordinator();
        var mdDoc = BuildRenderableDocument(includeFrontmatter: true);

        var markdown = coordinator.Render(mdDoc, CancellationToken.None);

        markdown.Should().StartWith("---\n");
        markdown.Should().Contain("file_type:");
    }

    [Fact]
    public void Render_should_skip_sections_with_shouldrender_false()
    {
        var coordinator = new MarkdownRenderCoordinator();
        var mdDoc = BuildRenderableDocument(includeFrontmatter: false);

        var markdown = coordinator.Render(mdDoc, CancellationToken.None);

        markdown.Should().NotStartWith("---");
    }

    [Fact]
    public void Render_should_omit_frontmatter_when_disabled()
    {
        var coordinator = new MarkdownRenderCoordinator();
        var mdDoc = BuildRenderableDocument(includeFrontmatter: false);

        var markdown = coordinator.Render(mdDoc, CancellationToken.None);

        markdown.Should().NotContain("---\n");
    }
}
