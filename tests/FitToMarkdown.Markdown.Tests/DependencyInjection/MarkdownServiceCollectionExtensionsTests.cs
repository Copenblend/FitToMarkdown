using FluentAssertions;
using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Markdown.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FitToMarkdown.Markdown.Tests.DependencyInjection;

public sealed class MarkdownServiceCollectionExtensionsTests
{
    [Fact]
    public void Should_resolve_IFitMarkdownProjector()
    {
        using var provider = new ServiceCollection()
            .AddFitToMarkdownMarkdown()
            .BuildServiceProvider();

        var projector = provider.GetService<IFitMarkdownProjector>();

        projector.Should().NotBeNull();
    }

    [Fact]
    public void Should_resolve_IMarkdownDocumentGenerator()
    {
        using var provider = new ServiceCollection()
            .AddFitToMarkdownMarkdown()
            .BuildServiceProvider();

        var generator = provider.GetService<IMarkdownDocumentGenerator>();

        generator.Should().NotBeNull();
    }

    [Fact]
    public void Projector_should_be_singleton()
    {
        using var provider = new ServiceCollection()
            .AddFitToMarkdownMarkdown()
            .BuildServiceProvider();

        var first = provider.GetRequiredService<IFitMarkdownProjector>();
        var second = provider.GetRequiredService<IFitMarkdownProjector>();

        first.Should().BeSameAs(second);
    }

    [Fact]
    public void Generator_should_be_singleton()
    {
        using var provider = new ServiceCollection()
            .AddFitToMarkdownMarkdown()
            .BuildServiceProvider();

        var first = provider.GetRequiredService<IMarkdownDocumentGenerator>();
        var second = provider.GetRequiredService<IMarkdownDocumentGenerator>();

        first.Should().BeSameAs(second);
    }
}
