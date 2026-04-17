using FitToMarkdown.Cli.Commands.Convert;
using FluentAssertions;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Commands;

public sealed class ConvertCommandTests
{
    [Theory]
    [InlineData("skip")]
    [InlineData("overwrite")]
    [InlineData("ask-each")]
    [InlineData("Skip")]
    [InlineData("OVERWRITE")]
    public void Validate_accepts_valid_overwrite_modes(string mode)
    {
        var settings = new ConvertCommandSettings { Overwrite = mode };

        var result = settings.Validate();

        result.Successful.Should().BeTrue();
    }

    [Theory]
    [InlineData("replace")]
    [InlineData("force")]
    [InlineData("")]
    public void Validate_rejects_invalid_overwrite_modes(string mode)
    {
        var settings = new ConvertCommandSettings { Overwrite = mode };

        var result = settings.Validate();

        result.Successful.Should().BeFalse();
    }

    [Fact]
    public void Validate_succeeds_with_null_overwrite()
    {
        var settings = new ConvertCommandSettings();

        var result = settings.Validate();

        result.Successful.Should().BeTrue();
    }

    [Fact]
    public void Validate_succeeds_with_empty_settings_and_path_is_optional()
    {
        var settings = new ConvertCommandSettings();

        var result = settings.Validate();

        result.Successful.Should().BeTrue();
        settings.Path.Should().BeNull();
    }
}
