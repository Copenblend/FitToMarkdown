using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Fit.Decoding;
using FitToMarkdown.Fit.Grouping;
using FitToMarkdown.Fit.Mapping;
using FitToMarkdown.Fit.Parsing;
using FitToMarkdown.Fit.Recovery;
using FitToMarkdown.Fit.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace FitToMarkdown.Fit.DependencyInjection;

/// <summary>
/// Registers FIT parsing services into a <see cref="IServiceCollection"/>.
/// </summary>
public static class FitServiceCollectionExtensions
{
    /// <summary>
    /// Adds the FIT parsing layer services required by <see cref="IFitFileParser"/> and <see cref="IFitMetadataInspector"/>.
    /// </summary>
    /// <param name="services">The service collection to populate.</param>
    /// <returns>The same service collection for fluent composition.</returns>
    public static IServiceCollection AddFitToMarkdownFit(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton(_ => new FitValidationPolicy());
        services.AddSingleton(sp => new FitDocumentBuilder(sp.GetRequiredService<FitValidationPolicy>()));
        services.AddSingleton(_ => new FitMetadataSummaryBuilder());
        services.AddSingleton(_ => new FitDecodeCoordinator());
        services.AddSingleton(_ => new FitActivityGroupingService());
        services.AddSingleton(_ => new FitSyntheticRecoveryService());
        services.AddSingleton<IFitFileParser>(sp => new FitFileParser(
            sp.GetRequiredService<FitDecodeCoordinator>(),
            sp.GetRequiredService<FitDocumentBuilder>()));
        services.AddSingleton<IFitMetadataInspector>(sp => new FitMetadataInspector(
            sp.GetRequiredService<FitDecodeCoordinator>(),
            sp.GetRequiredService<FitMetadataSummaryBuilder>()));

        return services;
    }
}
