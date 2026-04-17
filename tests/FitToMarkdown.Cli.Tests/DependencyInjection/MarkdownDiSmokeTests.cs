using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Markdown.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FitToMarkdown.Cli.Tests.DependencyInjection;

/// <summary>
/// Verifies that <see cref="MarkdownServiceCollectionExtensions.AddFitToMarkdownMarkdown"/>
/// correctly registers all markdown-layer services into the container.
/// </summary>
public sealed class MarkdownDiSmokeTests
{
    [Fact]
    public void Resolves_IFitMarkdownProjector()
    {
        using var provider = new ServiceCollection()
            .AddFitToMarkdownMarkdown()
            .BuildServiceProvider();

        var projector = provider.GetService<IFitMarkdownProjector>();

        projector.Should().NotBeNull();
    }

    [Fact]
    public void Resolves_IMarkdownDocumentGenerator()
    {
        using var provider = new ServiceCollection()
            .AddFitToMarkdownMarkdown()
            .BuildServiceProvider();

        var generator = provider.GetService<IMarkdownDocumentGenerator>();

        generator.Should().NotBeNull();
    }

    [Fact]
    public void FitMarkdownProjector_is_singleton()
    {
        using var provider = new ServiceCollection()
            .AddFitToMarkdownMarkdown()
            .BuildServiceProvider();

        var first = provider.GetRequiredService<IFitMarkdownProjector>();
        var second = provider.GetRequiredService<IFitMarkdownProjector>();

        first.Should().BeSameAs(second);
    }

    [Fact]
    public void MarkdownDocumentGenerator_is_singleton()
    {
        using var provider = new ServiceCollection()
            .AddFitToMarkdownMarkdown()
            .BuildServiceProvider();

        var first = provider.GetRequiredService<IMarkdownDocumentGenerator>();
        var second = provider.GetRequiredService<IMarkdownDocumentGenerator>();

        first.Should().BeSameAs(second);
    }
}
