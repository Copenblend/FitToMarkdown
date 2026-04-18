using FitToMarkdown.Cli.Tests.Fixtures;
using FluentAssertions;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Integration;

public sealed class VersionAndHelpRegressionTests
{
    [Fact]
    public void Root_help_lists_all_commands()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        var exitCode = app.Run(["--help"]);

        exitCode.Should().Be(0);
        console.Output.Should().Contain("convert");
        console.Output.Should().Contain("info");
        console.Output.Should().Contain("progression");
        console.Output.Should().Contain("version");
    }

    [Fact]
    public void Root_help_contains_expected_command_names()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        app.Run(["--help"]);

        console.Output.Should().Contain("convert");
        console.Output.Should().Contain("info");
        console.Output.Should().Contain("progression");
        console.Output.Should().Contain("version");
    }

    [Fact]
    public void Application_name_is_ftm()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        app.Run(["--help"]);

        console.Output.Should().Contain("ftm");
    }

    [Fact]
    public void Version_command_outputs_app_name()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        var exitCode = app.Run(["version"]);

        exitCode.Should().Be(0);
        console.Output.Should().Contain("ftm");
    }
}
