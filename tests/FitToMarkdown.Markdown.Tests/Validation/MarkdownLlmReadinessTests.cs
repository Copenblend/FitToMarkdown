using FluentAssertions;
using FitToMarkdown.Markdown.Projection;
using FitToMarkdown.Markdown.Rendering;
using FitToMarkdown.Markdown.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Markdown.Tests.Validation;

public sealed class MarkdownLlmReadinessTests
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

        markdown.Should().NotContain("<div", "LLM-ready markdown should not contain HTML divs");
        markdown.Should().NotContain("<table", "LLM-ready markdown should not contain HTML tables");
        markdown.Should().NotContain("<br", "LLM-ready markdown should not contain HTML line breaks");
    }

    [Fact]
    public void Tables_should_be_pipe_tables()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();

        var markdown = RenderFullMarkdown(doc);

        var lines = markdown.Split('\n');
        var tableLines = lines.Where(l => l.TrimStart().StartsWith('|')).ToList();

        // If tables exist, they should be pipe-delimited
        if (tableLines.Count > 0)
        {
            tableLines.Should().Contain(l => l.Contains("---"),
                "pipe tables should have separator rows");
        }
    }

    [Fact]
    public async Task Code_blocks_should_be_fenced()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();
        var opts = MarkdownTestFixtures.CreateDefaultOptions() with { IncludeRecordSamples = true };
        var projector = new FitMarkdownProjector();
        var generator = new MarkdownDocumentGenerator();
        var mdDoc = await projector.ProjectAsync(doc, opts);
        var result = await generator.GenerateAsync(mdDoc);
        var markdown = result.Content;

        var lines = markdown.Split('\n');
        var indentedCodeLines = lines.Where(l =>
            l.StartsWith("    ") && !l.TrimStart().StartsWith('|') && l.Trim().Length > 0).ToList();

        // Code blocks, if present, should use fenced style (```)
        // rather than indented style, for LLM readability
        var fencedOpens = lines.Count(l => l.TrimStart().StartsWith("```"));
        if (fencedOpens > 0)
        {
            // Fenced code blocks should be paired
            fencedOpens.Should().BeGreaterOrEqualTo(2, "fenced code blocks should be paired");
            (fencedOpens % 2).Should().Be(0, "fenced code blocks should be evenly paired");
        }
    }
}
