using FitToMarkdown.Cli.Configuration;
using FitToMarkdown.Cli.DependencyInjection;
using FitToMarkdown.Cli.Hosting;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Integration;

/// <summary>
/// End-to-end integration tests that exercise the version command
/// through the real CLI pipeline with full DI composition.
/// </summary>
public sealed class VersionCommandIntegrationTests
{
    [Fact]
    public void Version_command_returns_success()
    {
        var console = new TestConsole();
        var app = CreateTestApp(console);

        var exitCode = app.Run(["version"]);

        exitCode.Should().Be(0);
    }

    [Fact]
    public void Version_command_outputs_application_name()
    {
        var console = new TestConsole();
        var app = CreateTestApp(console);

        app.Run(["version"]);

        console.Output.Should().Contain("ftm");
    }

    [Fact]
    public void Version_command_help_returns_success()
    {
        var console = new TestConsole();
        var app = CreateTestApp(console);

        var exitCode = app.Run(["version", "--help"]);

        exitCode.Should().Be(0);
    }

    private static CommandApp CreateTestApp(TestConsole console)
    {
        var services = new ServiceCollection();
        services.AddFitToMarkdownApplication(console);
        var registrar = new TypeRegistrar(services);
        var app = new CommandApp(registrar);
        app.Configure(config =>
        {
            CliCommandConfiguration.Configure(config, "1.0.0-test");
            config.ConfigureConsole(console);
        });
        return app;
    }
}
