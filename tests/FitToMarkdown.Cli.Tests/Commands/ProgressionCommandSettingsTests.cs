using FitToMarkdown.Cli.Commands.Progression;
using FluentAssertions;
using Spectre.Console;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Commands;

public sealed class ProgressionCommandSettingsTests
{
    [Fact]
    public void Validate_returns_success_for_build_mode()
    {
        var settings = new ProgressionCommandSettings
        {
            Path = "activities",
            Sport = "Running",
        };

        var result = settings.Validate();

        result.Successful.Should().BeTrue();
    }

    [Fact]
    public void Validate_returns_success_for_add_mode()
    {
        var settings = new ProgressionCommandSettings
        {
            AddFile = "new-run.fit",
            ProgressionFile = "Running_Progression.md",
        };

        var result = settings.Validate();

        result.Successful.Should().BeTrue();
    }

    [Fact]
    public void Validate_fails_when_add_file_without_progression_file()
    {
        var settings = new ProgressionCommandSettings
        {
            AddFile = "new-run.fit",
        };

        var result = settings.Validate();

        result.Successful.Should().BeFalse();
        result.Message.Should().Contain("--progression-file");
    }

    [Fact]
    public void Validate_fails_when_progression_file_without_add_file()
    {
        var settings = new ProgressionCommandSettings
        {
            ProgressionFile = "Running_Progression.md",
        };

        var result = settings.Validate();

        result.Successful.Should().BeFalse();
        result.Message.Should().Contain("--add");
    }

    [Fact]
    public void Validate_fails_when_output_directory_is_whitespace()
    {
        var settings = new ProgressionCommandSettings
        {
            Path = "activities",
            OutputDirectory = "  ",
        };

        var result = settings.Validate();

        result.Successful.Should().BeFalse();
    }

    [Fact]
    public void Validate_returns_success_for_minimal_settings()
    {
        var settings = new ProgressionCommandSettings();

        var result = settings.Validate();

        result.Successful.Should().BeTrue();
    }
}
