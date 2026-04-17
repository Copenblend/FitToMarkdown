using FitToMarkdown.Cli.Commands.Info;
using FluentAssertions;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Commands;

public sealed class InfoCommandTests
{
    [Fact]
    public void Validate_returns_error_for_empty_path()
    {
        var settings = new InfoCommandSettings { Path = string.Empty };

        var result = settings.Validate();

        result.Successful.Should().BeFalse();
    }

    [Fact]
    public void Validate_returns_error_for_whitespace_path()
    {
        var settings = new InfoCommandSettings { Path = "   " };

        var result = settings.Validate();

        result.Successful.Should().BeFalse();
    }

    [Fact]
    public void Validate_succeeds_for_non_empty_path()
    {
        var settings = new InfoCommandSettings { Path = "activity.fit" };

        var result = settings.Validate();

        result.Successful.Should().BeTrue();
    }
}
