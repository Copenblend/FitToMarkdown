using FitToMarkdown.Cli.Commands.Version;
using FitToMarkdown.Cli.Configuration;
using FitToMarkdown.Cli.Services;
using FluentAssertions;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Commands;

public sealed class VersionCommandTests
{
    [Fact]
    public void VersionCommandSettings_always_validates_successfully()
    {
        var settings = new VersionCommandSettings();

        var result = settings.Validate();

        result.Successful.Should().BeTrue();
    }

    [Fact]
    public void Version_workflow_renders_application_name_and_version()
    {
        var console = new TestConsole();
        console.Profile.Width = 120;
        var versionProvider = new CliVersionProvider();
        var workflow = new VersionCommandWorkflow(console, versionProvider);

        var exitCode = workflow.Execute();

        exitCode.Should().Be(CliExitCodes.Success);
        console.Output.Should().Contain("ftm");
    }
}
