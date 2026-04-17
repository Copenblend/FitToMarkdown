using FitToMarkdown.Cli.Tests.Fixtures;
using FluentAssertions;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Integration;

public sealed class InfoCommandIntegrationTests
{
    [Fact]
    public void Info_help_returns_success()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        var exitCode = app.Run(["info", "--help"]);

        exitCode.Should().Be(0);
        console.Output.Should().Contain("info");
    }

    [Fact]
    public void Info_with_missing_file_returns_error_exit_code()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        var exitCode = app.Run(["info", @"C:\nonexistent\missing.fit"]);

        exitCode.Should().BeOneOf(2, 3);
    }

    [Fact]
    public void Info_command_is_registered_and_recognized()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        var exitCode = app.Run(["info", "--help"]);

        exitCode.Should().Be(0);
        console.Output.Should().Contain("FIT metadata");
    }
}
