using FitToMarkdown.Cli.Models;
using FitToMarkdown.Cli.Services;
using FitToMarkdown.Core.Abstractions;
using FluentAssertions;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Regression;

public sealed class OutputPathResolverSafetyTests
{
    private readonly OutputPathResolver _resolver = new();

    [Fact]
    public void Suggested_name_with_path_separator_falls_back_to_input_filename()
    {
        var mdResult = new MarkdownDocumentResult
        {
            Content = "# Test",
            SuggestedFileName = @"subdir\evil.md",
        };

        var result = _resolver.ResolveOutputFilePath(@"C:\output", @"C:\data\activity.fit", mdResult);

        result.Should().Be(@"C:\output\activity.md");
    }

    [Fact]
    public void Suggested_name_with_traversal_falls_back_to_input_filename()
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
    public void Normal_suggested_name_is_used_as_is()
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
    public void Empty_suggested_name_falls_back_to_input_filename()
    {
        var mdResult = new MarkdownDocumentResult
        {
            Content = "# Test",
            SuggestedFileName = "",
        };

        var result = _resolver.ResolveOutputFilePath(@"C:\output", @"C:\data\activity.fit", mdResult);

        result.Should().Be(@"C:\output\activity.md");
    }

    [Fact]
    public void Null_suggested_name_falls_back_to_input_filename()
    {
        var mdResult = new MarkdownDocumentResult
        {
            Content = "# Test",
            SuggestedFileName = null!,
        };

        var result = _resolver.ResolveOutputFilePath(@"C:\output", @"C:\data\activity.fit", mdResult);

        result.Should().Be(@"C:\output\activity.md");
    }
}
