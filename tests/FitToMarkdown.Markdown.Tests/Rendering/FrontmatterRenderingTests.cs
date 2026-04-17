using FluentAssertions;
using FitToMarkdown.Markdown.Projection;
using FitToMarkdown.Markdown.Rendering;
using FitToMarkdown.Markdown.Tests.Fixtures.Core;
using FitToMarkdown.Markdown.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Markdown.Tests.Rendering;

public sealed class FrontmatterRenderingTests
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
    public void Frontmatter_should_start_and_end_with_delimiters()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();

        var markdown = RenderMarkdown(doc);

        markdown.Should().StartWith("---\n");
        var secondDelimiter = markdown.IndexOf("---", 3, StringComparison.Ordinal);
        secondDelimiter.Should().BeGreaterThan(3);
    }

    [Fact]
    public void Frontmatter_key_order_should_be_deterministic()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();

        var first = RenderMarkdown(doc);
        var second = RenderMarkdown(doc);

        var firstFm = ExtractFrontmatter(first);
        var secondFm = ExtractFrontmatter(second);

        firstFm.Should().Be(secondFm);
    }

    [Fact]
    public void Missing_optional_fields_should_be_omitted_not_blank()
    {
        var doc = SparseMetadataFixtureFactory.CreateWithMissingManufacturer();

        var markdown = RenderMarkdown(doc);
        var fm = ExtractFrontmatter(markdown);

        fm.Should().NotContain("manufacturer: \n");
        fm.Should().NotContain("manufacturer:\n");
    }

    [Fact]
    public void Frontmatter_should_contain_no_html()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();

        var markdown = RenderMarkdown(doc);
        var fm = ExtractFrontmatter(markdown);

        fm.Should().NotContain("<");
        fm.Should().NotContain(">");
    }

    private static string ExtractFrontmatter(string markdown)
    {
        if (!markdown.StartsWith("---"))
            return string.Empty;

        var endIndex = markdown.IndexOf("---", 3, StringComparison.Ordinal);
        return endIndex > 3 ? markdown[..(endIndex + 3)] : string.Empty;
    }
}
