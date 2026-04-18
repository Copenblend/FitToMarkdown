using FitToMarkdown.Cli.Commands.Convert;
using FitToMarkdown.Cli.Commands.Info;
using FitToMarkdown.Cli.Commands.Version;
using FitToMarkdown.Cli.Rendering;
using FitToMarkdown.Cli.Services;
using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Fit.DependencyInjection;
using FitToMarkdown.Markdown.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace FitToMarkdown.Cli.DependencyInjection;

internal static class CliServiceCollectionExtensions
{
    internal static IServiceCollection AddFitToMarkdownCli(
        this IServiceCollection services,
        IAnsiConsole? console = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var ansiConsole = console ?? AnsiConsole.Console;

        services.AddSingleton(ansiConsole);
        services.AddSingleton<ICliFileSystem>(_ => new SystemCliFileSystem());
        services.AddSingleton(sp => new InteractivePathBrowser(
            sp.GetRequiredService<IAnsiConsole>(),
            sp.GetRequiredService<ICliFileSystem>()));
        services.AddSingleton(sp => new InputPathResolver(
            sp.GetRequiredService<ICliFileSystem>(),
            sp.GetRequiredService<InteractivePathBrowser>()));
        services.AddSingleton(sp => new FitFileDiscoveryService(
            sp.GetRequiredService<ICliFileSystem>()));
        services.AddSingleton(sp => new ConvertPromptService(
            sp.GetRequiredService<IAnsiConsole>()));
        services.AddSingleton(_ => new FitParseOptionsFactory());
        services.AddSingleton(_ => new MarkdownOptionsFactory());
        services.AddSingleton(_ => new OutputPathResolver());
        services.AddSingleton(_ => new CliVersionProvider());
        services.AddSingleton(sp => new InfoTableRenderer(
            sp.GetRequiredService<IAnsiConsole>()));
        services.AddSingleton(sp => new ConvertSummaryRenderer(
            sp.GetRequiredService<IAnsiConsole>()));
        services.AddSingleton(sp => new CliExceptionRenderer(
            sp.GetRequiredService<IAnsiConsole>()));
        services.AddSingleton(_ => new BatchConcurrencyPolicy());

        services.AddTransient(sp => new ConversionBatchRunner(
            sp.GetRequiredService<IFitFileParser>(),
            sp.GetRequiredService<IFitMarkdownProjector>(),
            sp.GetRequiredService<IMarkdownDocumentGenerator>(),
            sp.GetRequiredService<ICliFileSystem>(),
            sp.GetRequiredService<FitParseOptionsFactory>(),
            sp.GetRequiredService<MarkdownOptionsFactory>(),
            sp.GetRequiredService<OutputPathResolver>(),
            sp.GetRequiredService<ConvertPromptService>()));
        services.AddTransient<IConvertCommandWorkflow>(sp => new ConvertCommandWorkflow(
            sp.GetRequiredService<IAnsiConsole>(),
            sp.GetRequiredService<ICliFileSystem>(),
            sp.GetRequiredService<InputPathResolver>(),
            sp.GetRequiredService<FitFileDiscoveryService>(),
            sp.GetRequiredService<ConvertPromptService>(),
            sp.GetRequiredService<MarkdownOptionsFactory>(),
            sp.GetRequiredService<OutputPathResolver>(),
            sp.GetRequiredService<ConversionBatchRunner>(),
            sp.GetRequiredService<ConvertSummaryRenderer>(),
            sp.GetRequiredService<CliExceptionRenderer>(),
            sp.GetRequiredService<BatchConcurrencyPolicy>()));
        services.AddTransient<IInfoCommandWorkflow>(sp => new InfoCommandWorkflow(
            sp.GetRequiredService<IAnsiConsole>(),
            sp.GetRequiredService<ICliFileSystem>(),
            sp.GetRequiredService<IFitMetadataInspector>(),
            sp.GetRequiredService<InfoTableRenderer>(),
            sp.GetRequiredService<CliExceptionRenderer>()));
        services.AddTransient<IVersionCommandWorkflow>(sp => new VersionCommandWorkflow(
            sp.GetRequiredService<IAnsiConsole>(),
            sp.GetRequiredService<CliVersionProvider>()));

        return services;
    }

    internal static IServiceCollection AddFitToMarkdownApplication(
        this IServiceCollection services,
        IAnsiConsole? console = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddFitToMarkdownFit();
        services.AddFitToMarkdownMarkdown();
        services.AddFitToMarkdownCli(console);

        return services;
    }
}
