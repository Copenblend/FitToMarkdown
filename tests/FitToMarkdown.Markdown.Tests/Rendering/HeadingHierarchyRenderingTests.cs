using FluentAssertions;
using FitToMarkdown.Markdown.Projection;
using FitToMarkdown.Markdown.Rendering;
using FitToMarkdown.Markdown.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Markdown.Tests.Rendering;

public sealed class HeadingHierarchyRenderingTests
{
    private static string RenderMarkdown(Core.Documents.FitFileDocument doc)
    {
        var opts = MarkdownTestFixtures.CreateDefaultOptions();
        var coordinator = new DocumentProjectionCoordinator();
        var mdDoc = coordinator.Project(doc, opts, CancellationToken.None);
        var renderer = new MarkdownRenderCoordinator();
        return renderer.Render(mdDoc, CancellationToken.None);
    }

    [Fact]
    public void Document_should_have_exactly_one_h1()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();

        var markdown = RenderMarkdown(doc);

        var lines = markdown.Split('\n');
        var h1Count = lines.Count(l => l.StartsWith("# ") && !l.StartsWith("## "));
        h1Count.Should().Be(1);
    }

    [Fact]
    public void No_heading_levels_should_be_skipped()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();

        var markdown = RenderMarkdown(doc);

        var lines = markdown.Split('\n');
        int previousLevel = 0;
        foreach (var line in lines)
        {
            if (!line.StartsWith('#')) continue;

            int level = 0;
            foreach (char c in line)
            {
                if (c == '#') level++;
                else break;
            }

            if (previousLevel > 0)
                level.Should().BeLessOrEqualTo(previousLevel + 1,
                    $"heading level {level} should not skip from {previousLevel}");
            previousLevel = level;
        }
    }

    [Fact]
    public void Multi_session_should_use_correct_heading_levels()
    {
        var doc = MarkdownTestFixtures.CreateMultiSportActivity();

        var markdown = RenderMarkdown(doc);

        // Should contain H2 for sessions
        var lines = markdown.Split('\n');
        var h2Lines = lines.Where(l => l.StartsWith("## ") && !l.StartsWith("### ")).ToList();
        h2Lines.Should().NotBeEmpty("multi-session documents should have H2 session headings");
    }
}
