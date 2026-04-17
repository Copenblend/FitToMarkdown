using FitToMarkdown.Cli.Tests.Fixtures;
using FluentAssertions;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Integration;

public sealed class ConvertCommandIntegrationTests
{
    [Fact]
    public void Convert_help_returns_success()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        var exitCode = app.Run(["convert", "--help"]);

        exitCode.Should().Be(0);
        console.Output.Should().Contain("convert");
    }

    [Fact]
    public void Version_help_via_production_app_returns_success()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        var exitCode = app.Run(["version", "--help"]);

        exitCode.Should().Be(0);
    }

    [Fact]
    public void Info_help_via_production_app_returns_success()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        var exitCode = app.Run(["info", "--help"]);

        exitCode.Should().Be(0);
    }

    [Fact]
    public void Convert_with_no_args_and_no_interaction_returns_invalid_input()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        var exitCode = app.Run(["convert", "--no-interaction"]);

        exitCode.Should().Be(3);
    }
}
