using FluentAssertions;
using FitToMarkdown.Markdown.Projection;
using FitToMarkdown.Markdown.Rendering;
using FitToMarkdown.Markdown.Validation;
using FitToMarkdown.Markdown.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Markdown.Tests.Validation;

public sealed class MarkdownStructureValidationTests
{
    private static string RenderFullMarkdown(Core.Documents.FitFileDocument doc)
    {
        var opts = MarkdownTestFixtures.CreateDefaultOptions();
        var projector = new FitMarkdownProjector();
        var generator = new MarkdownDocumentGenerator();
        var mdDoc = projector.ProjectAsync(doc, opts).GetAwaiter().GetResult();
        var result = generator.GenerateAsync(mdDoc).GetAwaiter().GetResult();
        return result.Content;
    }

    [Fact]
    public void Output_should_contain_no_html_blocks()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();

        var markdown = RenderFullMarkdown(doc);

        markdown.Should().NotContain("<div");
        markdown.Should().NotContain("<table");
        markdown.Should().NotContain("<span");
        markdown.Should().NotContain("<p>");
    }

    [Fact]
    public void Output_should_have_exactly_one_h1()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();

        var markdown = RenderFullMarkdown(doc);

        var lines = markdown.Split('\n');
        var h1Count = lines.Count(l => l.StartsWith("# ") && !l.StartsWith("## "));
        h1Count.Should().Be(1);
    }

    [Fact]
    public void Output_should_have_no_skipped_heading_levels()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();

        var markdown = RenderFullMarkdown(doc);

        var issues = MarkdownStructureInspector.Inspect(markdown);
        var skippedLevelIssues = issues.Where(i => i.Contains("skipped")).ToList();

        skippedLevelIssues.Should().BeEmpty("heading levels should not be skipped");
    }
}
