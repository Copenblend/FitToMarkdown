using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Markdown.Formatting;
using FitToMarkdown.Markdown.Projection;
using FitToMarkdown.Markdown.Rendering;
using FitToMarkdown.Markdown.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace FitToMarkdown.Markdown.DependencyInjection;

/// <summary>
/// Registers markdown generation services into a <see cref="IServiceCollection"/>.
/// </summary>
public static class MarkdownServiceCollectionExtensions
{
    /// <summary>
    /// Adds the markdown generation layer services required by <see cref="IFitMarkdownProjector"/> and <see cref="IMarkdownDocumentGenerator"/>.
    /// </summary>
    /// <param name="services">The service collection to populate.</param>
    /// <returns>The same service collection for fluent composition.</returns>
    public static IServiceCollection AddFitToMarkdownMarkdown(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton(_ => new DocumentProjectionCoordinator());
        services.AddSingleton(_ => new MarkdownRenderCoordinator());
        services.AddSingleton(_ => new MarkdownValidationService());
        services.AddSingleton(_ => new SuggestedFileNameFactory());
        services.AddSingleton<IFitMarkdownProjector>(sp => new FitMarkdownProjector(
            sp.GetRequiredService<DocumentProjectionCoordinator>()));
        services.AddSingleton<IMarkdownDocumentGenerator>(sp => new MarkdownDocumentGenerator(
            sp.GetRequiredService<MarkdownRenderCoordinator>(),
            sp.GetRequiredService<MarkdownValidationService>(),
            sp.GetRequiredService<SuggestedFileNameFactory>()));

        return services;
    }
}
