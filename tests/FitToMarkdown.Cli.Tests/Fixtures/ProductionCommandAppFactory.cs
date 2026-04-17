using FitToMarkdown.Cli.Configuration;
using FitToMarkdown.Cli.DependencyInjection;
using FitToMarkdown.Cli.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace FitToMarkdown.Cli.Tests.Fixtures;

/// <summary>
/// Creates a real <see cref="CommandApp"/> backed by the production DI composition root.
/// Used for in-process integration tests that exercise the real pipeline.
/// </summary>
internal static class ProductionCommandAppFactory
{
    internal static CommandApp Create(TestConsole console)
    {
        var services = new ServiceCollection();
        services.AddFitToMarkdownApplication(console);
        var registrar = new TypeRegistrar(services);
        var app = new CommandApp(registrar);
        app.Configure(config =>
        {
            CliCommandConfiguration.Configure(config, "0.1.0-test");
            config.ConfigureConsole(console);
        });
        return app;
    }
}
