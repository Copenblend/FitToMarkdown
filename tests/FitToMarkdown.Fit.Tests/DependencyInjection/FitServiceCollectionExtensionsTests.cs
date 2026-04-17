using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Fit.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FitToMarkdown.Fit.Tests.DependencyInjection;

public sealed class FitServiceCollectionExtensionsTests
{
    [Fact]
    public void AddFitToMarkdownFit_Resolves_IFitFileParser()
    {
        using var provider = new ServiceCollection()
            .AddFitToMarkdownFit()
            .BuildServiceProvider();

        var parser = provider.GetService<IFitFileParser>();

        parser.Should().NotBeNull();
    }

    [Fact]
    public void AddFitToMarkdownFit_Resolves_IFitMetadataInspector()
    {
        using var provider = new ServiceCollection()
            .AddFitToMarkdownFit()
            .BuildServiceProvider();

        var inspector = provider.GetService<IFitMetadataInspector>();

        inspector.Should().NotBeNull();
    }

    [Fact]
    public void Parser_IsSingleton()
    {
        using var provider = new ServiceCollection()
            .AddFitToMarkdownFit()
            .BuildServiceProvider();

        var first = provider.GetRequiredService<IFitFileParser>();
        var second = provider.GetRequiredService<IFitFileParser>();

        first.Should().BeSameAs(second);
    }

    [Fact]
    public void Inspector_IsSingleton()
    {
        using var provider = new ServiceCollection()
            .AddFitToMarkdownFit()
            .BuildServiceProvider();

        var first = provider.GetRequiredService<IFitMetadataInspector>();
        var second = provider.GetRequiredService<IFitMetadataInspector>();

        first.Should().BeSameAs(second);
    }
}
