using FitToMarkdown.Cli.Tests.Fixtures;
using FluentAssertions;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Integration;

public sealed class ConvertInvalidInputIntegrationTests
{
    [Fact]
    public void Convert_with_non_existent_file_returns_error()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        var exitCode = app.Run(["convert", "--no-interaction", @"C:\nonexistent\file.fit"]);

        exitCode.Should().BeOneOf(2, 3);
    }

    [Fact]
    public void Convert_with_empty_directory_returns_error()
    {
        using var workspace = new TemporaryWorkspace();
        var emptyDir = workspace.CreateSubdirectory("empty");

        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        var exitCode = app.Run(["convert", "--no-interaction", emptyDir]);

        exitCode.Should().BeOneOf(2, 3);
    }

    [Fact]
    public void Convert_with_invalid_overwrite_mode_returns_error()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        var exitCode = app.Run(["convert", "--no-interaction", ".", "--overwrite", "invalid-mode"]);

        exitCode.Should().NotBe(0);
    }
}
