using FitToMarkdown.Cli.Configuration;
using Spectre.Console.Cli;

namespace FitToMarkdown.Cli.Tests.Fixtures;

internal static class TestCommandAppFactory
{
    internal static CommandApp Create(ITypeRegistrar registrar)
    {
        var app = new CommandApp(registrar);
        app.Configure(config =>
        {
            config.PropagateExceptions();
            CliCommandConfiguration.Configure(config, "0.1.0-test");
        });
        return app;
    }
}
