using FluentAssertions;
using FitToMarkdown.Markdown.Formatting;
using Xunit;

namespace FitToMarkdown.Markdown.Tests;

public sealed class YamlScalarFormatterTests
{
    [Fact]
    public void FormatScalar_should_return_null_for_null_value()
    {
        var result = YamlScalarFormatter.FormatScalar("key", null);

        result.Should().BeNull();
    }

    [Theory]
    [InlineData("true")]
    [InlineData("false")]
    [InlineData("yes")]
    [InlineData("no")]
    [InlineData("on")]
    [InlineData("off")]
    [InlineData("null")]
    [InlineData("~")]
    public void FormatScalar_should_quote_boolean_like_strings(string value)
    {
        var result = YamlScalarFormatter.FormatScalar("key", value);

        result.Should().NotBeNull();
        result.Should().Contain("\"");
    }

    [Fact]
    public void FormatScalar_should_not_quote_plain_strings()
    {
        var result = YamlScalarFormatter.FormatScalar("sport", "Running");

        result.Should().Be("sport: Running\n");
    }

    [Theory]
    [InlineData(":value")]
    [InlineData("#comment")]
    [InlineData("{curly}")]
    [InlineData("[bracket]")]
    [InlineData("value:colon")]
    [InlineData("with\nnewline")]
    public void NeedsQuoting_should_detect_yaml_special_characters(string value)
    {
        var result = YamlScalarFormatter.NeedsQuoting(value);

        result.Should().BeTrue();
    }

    [Fact]
    public void QuoteYamlString_should_escape_backslash_and_quotes()
    {
        var result = YamlScalarFormatter.QuoteYamlString("path\\to\\\"file\"");

        result.Should().Be("\"path\\\\to\\\\\\\"file\\\"\"");
    }

    [Fact]
    public void QuoteYamlString_should_strip_null_terminators()
    {
        var result = YamlScalarFormatter.QuoteYamlString("name\0value");

        result.Should().Be("\"namevalue\"");
    }

    [Fact]
    public void FormatNumeric_should_format_with_specified_decimals()
    {
        var result = YamlScalarFormatter.FormatNumeric("distance_km", 10.123456, 2);

        result.Should().Be("distance_km: 10.12\n");
    }

    [Fact]
    public void FormatNumeric_should_return_null_for_null_value()
    {
        YamlScalarFormatter.FormatNumeric("key", null, 2).Should().BeNull();
    }

    [Fact]
    public void FormatInteger_should_return_null_for_null_value()
    {
        YamlScalarFormatter.FormatInteger("key", null).Should().BeNull();
    }

    [Fact]
    public void FormatInteger_should_format_value()
    {
        var result = YamlScalarFormatter.FormatInteger("count", 42);

        result.Should().Be("count: 42\n");
    }
}
