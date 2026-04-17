using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Fit.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FitToMarkdown.Cli.Tests.DependencyInjection;

/// <summary>
/// Verifies that <see cref="FitServiceCollectionExtensions.AddFitToMarkdownFit"/>
/// correctly registers all FIT-layer services into the container.
/// </summary>
public sealed class FitDiSmokeTests
{
    [Fact]
    public void Resolves_IFitFileParser()
    {
        using var provider = new ServiceCollection()
            .AddFitToMarkdownFit()
            .BuildServiceProvider();

        var parser = provider.GetService<IFitFileParser>();

        parser.Should().NotBeNull();
    }

    [Fact]
    public void Resolves_IFitMetadataInspector()
    {
        using var provider = new ServiceCollection()
            .AddFitToMarkdownFit()
            .BuildServiceProvider();

        var inspector = provider.GetService<IFitMetadataInspector>();

        inspector.Should().NotBeNull();
    }

    [Fact]
    public void FitFileParser_is_singleton()
    {
        using var provider = new ServiceCollection()
            .AddFitToMarkdownFit()
            .BuildServiceProvider();

        var first = provider.GetRequiredService<IFitFileParser>();
        var second = provider.GetRequiredService<IFitFileParser>();

        first.Should().BeSameAs(second);
    }

    [Fact]
    public void FitMetadataInspector_is_singleton()
    {
        using var provider = new ServiceCollection()
            .AddFitToMarkdownFit()
            .BuildServiceProvider();

        var first = provider.GetRequiredService<IFitMetadataInspector>();
        var second = provider.GetRequiredService<IFitMetadataInspector>();

        first.Should().BeSameAs(second);
    }
}
