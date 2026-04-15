using FluentAssertions;
using FitToMarkdown.Cli;
using FitToMarkdown.Core;
using FitToMarkdown.Fit;
using FitToMarkdown.Markdown;
using Xunit;

namespace FitToMarkdown.Cli.Tests;

public sealed class ScaffoldingSmokeTests
{
    [Fact]
    public void CliAssemblyMarker_should_be_loadable_with_all_project_references_resolved()
    {
        var cliMarkerType = typeof(CliAssemblyMarker);

        cliMarkerType.Should().NotBeNull();
        cliMarkerType.Assembly.GetName().Name.Should().Be("FitToMarkdown.Cli");

        typeof(AssemblyMarker).Assembly.GetName().Name.Should().Be("FitToMarkdown.Core");
        typeof(FitAssemblyMarker).Assembly.GetName().Name.Should().Be("FitToMarkdown.Fit");
        typeof(MarkdownAssemblyMarker).Assembly.GetName().Name.Should().Be("FitToMarkdown.Markdown");
    }
}