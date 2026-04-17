using FitToMarkdown.Cli.DependencyInjection;
using FitToMarkdown.Core.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.DependencyInjection;

/// <summary>
/// Verifies that the full <see cref="CliServiceCollectionExtensions.AddFitToMarkdownApplication"/>
/// composition root correctly wires up all layers (FIT, Markdown, CLI).
/// </summary>
public sealed class ApplicationCompositionSmokeTests
{
    [Fact]
    public void Full_composition_resolves_all_core_abstractions()
    {
        var console = new TestConsole();
        using var provider = new ServiceCollection()
            .AddFitToMarkdownApplication(console)
            .BuildServiceProvider();

        provider.GetService<IFitFileParser>().Should().NotBeNull();
        provider.GetService<IFitMetadataInspector>().Should().NotBeNull();
        provider.GetService<IFitMarkdownProjector>().Should().NotBeNull();
        provider.GetService<IMarkdownDocumentGenerator>().Should().NotBeNull();
    }

    [Fact]
    public void Full_composition_builds_without_exception()
    {
        var console = new TestConsole();
        var services = new ServiceCollection();
        services.AddFitToMarkdownApplication(console);

        var buildAction = () => services.BuildServiceProvider();

        buildAction.Should().NotThrow();
    }

    [Fact]
    public void Full_composition_resolves_core_services_from_provider()
    {
        var console = new TestConsole();
        using var provider = new ServiceCollection()
            .AddFitToMarkdownApplication(console)
            .BuildServiceProvider();

        provider.GetService<IFitFileParser>().Should().NotBeNull();
        provider.GetService<IFitMetadataInspector>().Should().NotBeNull();
        provider.GetService<IFitMarkdownProjector>().Should().NotBeNull();
        provider.GetService<IMarkdownDocumentGenerator>().Should().NotBeNull();
    }
}
