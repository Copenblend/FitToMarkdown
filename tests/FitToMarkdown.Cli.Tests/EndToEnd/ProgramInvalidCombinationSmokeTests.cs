using FitToMarkdown.Cli.Tests.Fixtures;
using FluentAssertions;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.EndToEnd;

public sealed class ProgramInvalidCombinationSmokeTests
{
    [Fact]
    public void NoInteraction_with_ask_each_returns_nonzero_exit()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        var exitCode = app.Run(["convert", "--no-interaction", "--overwrite", "ask-each", "somefile.fit"]);

        exitCode.Should().NotBe(0);
    }

    [Fact]
    public void Whitespace_output_returns_nonzero_exit()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        var exitCode = app.Run(["convert", "--output", "  ", "somefile.fit"]);

        exitCode.Should().NotBe(0);
    }
}
