using FluentAssertions;
using FitToMarkdown.Markdown.Validation;
using Xunit;

namespace FitToMarkdown.Markdown.Tests.Validation;

public sealed class ValidationDiagnosticsTests
{
    private readonly MarkdownValidationService _validator = new();

    [Fact]
    public void Valid_markdown_returns_no_issues()
    {
        const string markdown = "# My Activity\n\n## Overview\n\nSome content here.\n";

        bool isValid = _validator.Validate(markdown, out var issues);

        isValid.Should().BeTrue();
        issues.Should().BeEmpty();
    }

    [Fact]
    public void Multiple_H1_headings_returns_Structure_prefix()
    {
        const string markdown = "# First Title\n\n# Second Title\n\nSome content.\n";

        bool isValid = _validator.Validate(markdown, out var issues);

        isValid.Should().BeFalse();
        issues.Should().Contain(i => i.StartsWith("[Structure]"),
            "validation issue for multiple H1s should have [Structure] prefix");
        issues.Should().Contain(i => i.Contains("Multiple H1"));
    }

    [Fact]
    public void Skipped_heading_level_returns_Structure_prefix()
    {
        const string markdown = "# Title\n\n#### Skipped to H4\n\nSome content.\n";

        bool isValid = _validator.Validate(markdown, out var issues);

        isValid.Should().BeFalse();
        issues.Should().Contain(i => i.StartsWith("[Structure]"),
            "validation issue for skipped heading should have [Structure] prefix");
        issues.Should().Contain(i => i.Contains("Heading level skipped"));
    }

    [Fact]
    public void Empty_document_returns_Content_prefix()
    {
        const string markdown = "";

        bool isValid = _validator.Validate(markdown, out var issues);

        isValid.Should().BeFalse();
        issues.Should().Contain(i => i.StartsWith("[Content]") || i.StartsWith("[Structure]"),
            "validation issue for empty document should have a rule prefix");
    }

    [Fact]
    public void Missing_H1_returns_Structure_prefix()
    {
        const string markdown = "## Only H2\n\nSome content.\n";

        bool isValid = _validator.Validate(markdown, out var issues);

        isValid.Should().BeFalse();
        issues.Should().Contain(i => i.StartsWith("[Structure]") && i.Contains("Missing H1"),
            "validation issue for missing H1 should have [Structure] prefix");
    }
}
