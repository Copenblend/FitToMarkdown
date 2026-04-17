using FitToMarkdown.Cli.Commands.Convert;
using FluentAssertions;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Regression;

public sealed class ConvertCommandSettingsValidationTests
{
    [Theory]
    [InlineData("skip")]
    [InlineData("SKIP")]
    [InlineData("Skip")]
    public void Validate_accepts_skip_overwrite_mode_case_insensitively(string mode)
    {
        var settings = new ConvertCommandSettings { Overwrite = mode };

        var result = settings.Validate();

        result.Successful.Should().BeTrue();
    }

    [Theory]
    [InlineData("ask-each")]
    [InlineData("ASK-EACH")]
    [InlineData("Ask-Each")]
    public void Validate_accepts_ask_each_overwrite_mode_case_insensitively(string mode)
    {
        var settings = new ConvertCommandSettings { Overwrite = mode };

        var result = settings.Validate();

        result.Successful.Should().BeTrue();
    }

    [Theory]
    [InlineData("overwrite")]
    [InlineData("OVERWRITE")]
    [InlineData("Overwrite")]
    public void Validate_accepts_overwrite_mode_case_insensitively(string mode)
    {
        var settings = new ConvertCommandSettings { Overwrite = mode };

        var result = settings.Validate();

        result.Successful.Should().BeTrue();
    }

    [Theory]
    [InlineData("replace")]
    [InlineData("force")]
    [InlineData("always")]
    [InlineData("never")]
    public void Validate_rejects_invalid_overwrite_mode(string mode)
    {
        var settings = new ConvertCommandSettings { Overwrite = mode };

        var result = settings.Validate();

        result.Successful.Should().BeFalse();
    }

    [Fact]
    public void Validate_succeeds_with_default_output_directory_when_none_specified()
    {
        var settings = new ConvertCommandSettings { Path = @"C:\data\activity.fit" };

        var result = settings.Validate();

        result.Successful.Should().BeTrue();
        settings.OutputDirectory.Should().BeNull();
    }

    [Fact]
    public void Validate_accepts_path_with_spaces()
    {
        var settings = new ConvertCommandSettings { Path = @"C:\my data folder\my activity.fit" };

        var result = settings.Validate();

        result.Successful.Should().BeTrue();
        settings.Path.Should().Contain(" ");
    }
}
