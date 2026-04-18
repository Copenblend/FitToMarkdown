using FitToMarkdown.Cli.Services;
using FitToMarkdown.Core.Abstractions;
using FluentAssertions;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Integration;

public sealed class CanonicalPathSafetyIntegrationTests
{
    private readonly OutputPathResolver _resolver = new();

    [Fact]
    public void Reserved_windows_name_CON_falls_back_to_input_filename()
    {
        var mdResult = new MarkdownDocumentResult
        {
            Content = "# Test",
            SuggestedFileName = "CON.md",
        };

        var result = _resolver.ResolveOutputFilePath(@"C:\output", @"C:\data\activity.fit", mdResult);

        result.Should().Be(@"C:\output\activity.md");
    }

    [Fact]
    public void Suggested_filename_with_trailing_dot_falls_back()
    {
        var mdResult = new MarkdownDocumentResult
        {
            Content = "# Test",
            SuggestedFileName = "myfile.",
        };

        var result = _resolver.ResolveOutputFilePath(@"C:\output", @"C:\data\activity.fit", mdResult);

        result.Should().Be(@"C:\output\activity.md");
    }

    [Fact]
    public void Suggested_filename_with_traversal_falls_back()
    {
        var mdResult = new MarkdownDocumentResult
        {
            Content = "# Test",
            SuggestedFileName = "..\\..\\etc\\evil.md",
        };

        var result = _resolver.ResolveOutputFilePath(@"C:\output", @"C:\data\activity.fit", mdResult);

        result.Should().Be(@"C:\output\activity.md");
    }

    [Fact]
    public void Normal_suggested_filename_is_used_as_is()
    {
        var mdResult = new MarkdownDocumentResult
        {
            Content = "# Test",
            SuggestedFileName = "2025-06-15_Running_8.5km.md",
        };

        var result = _resolver.ResolveOutputFilePath(@"C:\output", @"C:\data\activity.fit", mdResult);

        result.Should().Be(@"C:\output\2025-06-15_Running_8.5km.md");
    }

    [Fact]
    public void Reserved_name_NUL_falls_back_to_input_filename()
    {
        var mdResult = new MarkdownDocumentResult
        {
            Content = "# Test",
            SuggestedFileName = "NUL.md",
        };

        var result = _resolver.ResolveOutputFilePath(@"C:\output", @"C:\data\run.fit", mdResult);

        result.Should().Be(@"C:\output\run.md");
    }

    [Fact]
    public void Suggested_filename_with_trailing_space_falls_back()
    {
        var mdResult = new MarkdownDocumentResult
        {
            Content = "# Test",
            SuggestedFileName = "myfile.md ",
        };

        var result = _resolver.ResolveOutputFilePath(@"C:\output", @"C:\data\activity.fit", mdResult);

        result.Should().Be(@"C:\output\activity.md");
    }
}
