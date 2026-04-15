using FluentAssertions;
using FitToMarkdown.Markdown;
using Xunit;

namespace FitToMarkdown.Markdown.Tests;

public sealed class ScaffoldingSmokeTests
{
    [Fact]
    public void MarkdownAssemblyMarker_should_be_loadable()
    {
        var markerType = typeof(MarkdownAssemblyMarker);

        markerType.Should().NotBeNull();
        markerType.Assembly.GetName().Name.Should().Be("FitToMarkdown.Markdown");
    }
}