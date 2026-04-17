using FluentAssertions;
using FitToMarkdown.Core.Abstractions;

namespace FitToMarkdown.Markdown.Tests.Fixtures;

/// <summary>
/// Centralizes markdown-contract assertions so frontmatter, section order, omission,
/// and token-budget checks stay readable across test classes.
/// </summary>
internal static class MarkdownArtifactAssertions
{
    internal static void AssertFrontmatterKeyOrder(string markdown, params string[] expectedKeys)
    {
        markdown.Should().StartWith("---", "markdown should begin with YAML frontmatter delimiter");

        var endIndex = markdown.IndexOf("---", 3, StringComparison.Ordinal);
        endIndex.Should().BeGreaterThan(3, "closing frontmatter delimiter should exist");

        var frontmatter = markdown[3..endIndex];
        var lines = frontmatter.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var keys = lines
            .Where(l => l.Contains(':'))
            .Select(l => l[..l.IndexOf(':')].Trim())
            .ToList();

        for (int i = 0; i < expectedKeys.Length; i++)
        {
            var keyIndex = keys.IndexOf(expectedKeys[i]);
            keyIndex.Should().BeGreaterOrEqualTo(0,
                $"frontmatter should contain key '{expectedKeys[i]}'");

            if (i > 0)
            {
                var prevIndex = keys.IndexOf(expectedKeys[i - 1]);
                keyIndex.Should().BeGreaterThan(prevIndex,
                    $"key '{expectedKeys[i]}' should appear after '{expectedKeys[i - 1]}'");
            }
        }
    }

    internal static void AssertSectionPresent(string markdown, string heading)
    {
        markdown.Should().Contain($"# {heading}",
            $"section '{heading}' should be present in the markdown");
    }

    internal static void AssertSectionAbsent(string markdown, string heading)
    {
        markdown.Should().NotContain($"# {heading}",
            $"section '{heading}' should not be present in the markdown");
    }

    internal static void AssertTokenBudget(string markdown, int maximumTokens)
    {
        if (maximumTokens <= 0) return;

        // Rough token estimate: chars / 4
        var estimatedTokens = markdown.Length / 4;
        estimatedTokens.Should().BeLessOrEqualTo(maximumTokens,
            $"markdown should stay within {maximumTokens} estimated token budget");
    }

    internal static void AssertDeterministic(MarkdownDocumentResult first, MarkdownDocumentResult second)
    {
        first.Content.Should().Be(second.Content,
            "two renders of the same input should produce identical content");
        first.SuggestedFileName.Should().Be(second.SuggestedFileName,
            "suggested file name should be deterministic");
        first.RenderedSections.Should().BeEquivalentTo(second.RenderedSections,
            "rendered section list should be identical");
    }

    internal static void AssertWellFormedMarkdown(string markdown)
    {
        markdown.Should().NotBeNullOrWhiteSpace("rendered markdown must not be empty");

        // No HTML blocks
        markdown.Should().NotContain("<div", "markdown should not contain HTML div blocks");
        markdown.Should().NotContain("<table", "markdown should not contain HTML table blocks");

        // Consistent line endings
        if (markdown.Contains('\r'))
        {
            markdown.Should().NotContain("\r\n\r\n\r\n",
                "markdown should not have triple blank lines");
        }
    }
}
