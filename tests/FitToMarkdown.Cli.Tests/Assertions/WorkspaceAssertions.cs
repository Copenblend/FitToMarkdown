using FluentAssertions;

namespace FitToMarkdown.Cli.Tests.Assertions;

/// <summary>
/// Centralizes written-file assertions, output-root containment checks,
/// and unchanged-file checks for CLI integration tests.
/// </summary>
internal static class WorkspaceAssertions
{
    internal static void AssertAllFilesUnderRoot(string outputRoot, IReadOnlyList<string> writtenFiles)
    {
        var normalizedRoot = Path.GetFullPath(outputRoot);

        foreach (var file in writtenFiles)
        {
            var normalizedFile = Path.GetFullPath(file);
            normalizedFile.Should().StartWith(normalizedRoot,
                $"file '{file}' must be contained under output root '{outputRoot}'");
        }
    }

    internal static void AssertFileUnchanged(string filePath, string expectedContent)
    {
        File.Exists(filePath).Should().BeTrue($"file '{filePath}' should still exist");
        var actual = File.ReadAllText(filePath);
        actual.Should().Be(expectedContent,
            $"file '{filePath}' content should not have changed");
    }

    internal static void AssertNonEmptyMarkdownFile(string filePath)
    {
        File.Exists(filePath).Should().BeTrue($"markdown file '{filePath}' should exist");
        var content = File.ReadAllText(filePath);
        content.Should().NotBeNullOrWhiteSpace("markdown file should have content");
        content.Should().Contain("#", "markdown file should contain at least one heading");
    }
}
