using FluentAssertions;
using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Summaries;
using FitToMarkdown.Markdown.Projection;
using FitToMarkdown.Markdown.Rendering;
using FitToMarkdown.Markdown.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Markdown.Tests;

public sealed class MarkdownDocumentGeneratorTests
{
    private static FitMarkdownDocument BuildTestDocument()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();
        var fm = FrontmatterProjector.Project(doc);
        var (title, headingTs, overview) = OverviewProjector.Project(doc);

        var sessionSection = new SessionSection
        {
            Session = doc.ActivityContent!.Sessions[0],
            LapRows =
            [
                new LapTableRow { LapNumber = 1, DistanceMeters = 3000, Duration = TimeSpan.FromMinutes(14) },
                new LapTableRow { LapNumber = 2, DistanceMeters = 3200, Duration = TimeSpan.FromMinutes(14) },
            ],
        };

        var options = MarkdownTestFixtures.CreateDefaultOptions();
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
    public async Task GenerateAsync_should_return_non_empty_markdown()
    {
        var gen = new MarkdownDocumentGenerator();
        var mdDoc = BuildTestDocument();

        var result = await gen.GenerateAsync(mdDoc);

        result.Content.Should().NotBeNullOrWhiteSpace();
        result.Content.Should().Contain("Running");
    }

    [Fact]
    public async Task GenerateAsync_should_include_suggested_filename()
    {
        var gen = new MarkdownDocumentGenerator();
        var mdDoc = BuildTestDocument();

        var result = await gen.GenerateAsync(mdDoc);

        result.SuggestedFileName.Should().NotBeNullOrEmpty();
        result.SuggestedFileName.Should().EndWith(".md");
    }

    [Fact]
    public async Task GenerateAsync_should_estimate_token_count()
    {
        var gen = new MarkdownDocumentGenerator();
        var mdDoc = BuildTestDocument();

        var result = await gen.GenerateAsync(mdDoc);

        result.ApproximateTokenCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public void GenerateAsync_should_throw_on_null_document()
    {
        var gen = new MarkdownDocumentGenerator();

        var act = () => gen.GenerateAsync(null!).AsTask();

        act.Should().ThrowAsync<ArgumentNullException>();
    }
}
