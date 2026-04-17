using FitToMarkdown.Cli.Configuration;
using FluentAssertions;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Commands;

public sealed class HelpOutputTests
{
    private static (CommandApp App, TestConsole Console) CreateApp()
    {
        var console = new TestConsole();
        console.Profile.Width = 120;
        var app = new CommandApp();
        app.Configure(config =>
        {
            config.Settings.Console = console;
            CliCommandConfiguration.Configure(config, "0.1.0-test");
        });
        return (app, console);
    }

    [Fact]
    public void Running_with_no_args_shows_help_listing_all_commands()
    {
        var (app, console) = CreateApp();

        app.Run(Array.Empty<string>());

        console.Output.Should().Contain("convert");
        console.Output.Should().Contain("info");
        console.Output.Should().Contain("version");
    }

    [Fact]
    public void Help_contains_expected_command_descriptions()
    {
        var (app, console) = CreateApp();

        app.Run(Array.Empty<string>());

        console.Output.Should().Contain("Convert one FIT file");
        console.Output.Should().Contain("Display FIT metadata");
        console.Output.Should().Contain("Display the installed application version");
    }

    [Fact]
    public void Version_flag_shows_application_version()
    {
        var (app, console) = CreateApp();

        app.Run(["--version"]);

        console.Output.Should().Contain("0.1.0-test");
    }
}
