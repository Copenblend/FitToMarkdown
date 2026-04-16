using FluentAssertions;
using FitToMarkdown.Markdown.Validation;
using Xunit;

namespace FitToMarkdown.Markdown.Tests;

public sealed class MarkdownValidationServiceTests
{
    [Fact]
    public void Validate_should_pass_for_valid_markdown()
    {
        var validator = new MarkdownValidationService();
        var markdown = "# Title\n\nSome paragraph text.\n\n## Section\n\nMore text.\n";

        var result = validator.Validate(markdown, out var issues);

        result.Should().BeTrue();
        issues.Should().BeEmpty();
    }

    [Fact]
    public void Validate_should_detect_duplicate_h1()
    {
        var validator = new MarkdownValidationService();
        var markdown = "# Title One\n\n# Title Two\n\nText.\n";

        var result = validator.Validate(markdown, out var issues);

        result.Should().BeFalse();
        issues.Should().Contain(i => i.Contains("Multiple H1"));
    }

    [Fact]
    public void StructureInspector_should_detect_heading_skip()
    {
        // H1 followed immediately by H3 (skipping H2)
        var markdown = "# Title\n\n### Sub-sub\n\nText.\n";

        var issues = MarkdownStructureInspector.Inspect(markdown);

        issues.Should().Contain(i => i.Contains("skipped"));
    }

    [Fact]
    public void Validate_should_pass_for_markdown_with_frontmatter()
    {
        var validator = new MarkdownValidationService();
        var markdown = "---\nsport: Running\n---\n\n# Title\n\nParagraph.\n";

        var result = validator.Validate(markdown, out var issues);

        result.Should().BeTrue();
        issues.Should().BeEmpty();
    }
}
