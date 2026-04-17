using FitToMarkdown.Cli.Tests.Fixtures;
using FluentAssertions;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Integration;

public sealed class ConvertNoInteractionIntegrationTests
{
    [Fact]
    public void Convert_no_interaction_missing_path_returns_InvalidInput()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        var exitCode = app.Run(["convert", "--no-interaction"]);

        exitCode.Should().Be(3);
    }

    [Fact]
    public void Convert_no_interaction_non_existent_path_returns_error()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        var exitCode = app.Run(["convert", "--no-interaction", @"C:\nonexistent\path.fit"]);

        exitCode.Should().BeOneOf(2, 3);
    }

    [Fact]
    public void Convert_help_returns_success()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        var exitCode = app.Run(["convert", "--help"]);

        exitCode.Should().Be(0);
    }
}
