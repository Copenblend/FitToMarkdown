using FitToMarkdown.Cli.Tests.Fixtures;
using FluentAssertions;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Regression;

public sealed class NonInteractiveCombinationTests
{
    [Fact]
    public void NoInteraction_with_overwrite_ask_each_returns_validation_error()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        var exitCode = app.Run(["convert", "--no-interaction", "--overwrite", "ask-each", "somefile.fit"]);

        exitCode.Should().NotBe(0);
    }

    [Fact]
    public void NoInteraction_with_overwrite_skip_does_not_fail_validation()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        // This won't fail on settings validation — it may fail on missing file but not on option combination
        var exitCode = app.Run(["convert", "--no-interaction", "--overwrite", "skip", @"C:\nonexistent\path.fit"]);

        // The error should be about the missing file, not about the option combination
        console.Output.Should().NotContain("Cannot use '--overwrite ask-each'");
    }

    [Fact]
    public void Empty_output_directory_returns_validation_error()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        var exitCode = app.Run(["convert", "--output", "  ", "somefile.fit"]);

        exitCode.Should().NotBe(0);
    }

    [Fact]
    public void NoInteraction_alone_does_not_fail_settings_validation()
    {
        var console = new TestConsole();
        var app = ProductionCommandAppFactory.Create(console);

        // --no-interaction alone doesn't cause a settings validation error
        // It may fail later due to missing path, but not in Validate()
        var exitCode = app.Run(["convert", "--no-interaction"]);

        // Should fail with InvalidInput (3) due to no path, not with a settings validation error
        console.Output.Should().NotContain("Cannot use '--overwrite ask-each'");
        console.Output.Should().NotContain("Output directory cannot be empty");
    }
}
