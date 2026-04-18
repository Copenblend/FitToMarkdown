using FitToMarkdown.Cli.Commands.Info;
using FluentAssertions;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Commands;

public sealed class InfoCommandTests
{
    [Fact]
    public void Path_defaults_to_null()
    {
        var settings = new InfoCommandSettings();

        settings.Path.Should().BeNull();
    }

    [Fact]
    public void Validate_succeeds_for_null_path()
    {
        var settings = new InfoCommandSettings();

        var result = settings.Validate();

        result.Successful.Should().BeTrue();
    }

    [Fact]
    public void Validate_succeeds_for_non_empty_path()
    {
        var settings = new InfoCommandSettings { Path = "activity.fit" };

        var result = settings.Validate();

        result.Successful.Should().BeTrue();
    }
}
