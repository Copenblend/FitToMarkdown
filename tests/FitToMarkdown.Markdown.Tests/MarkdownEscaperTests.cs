using FluentAssertions;
using FitToMarkdown.Markdown.Formatting;
using Xunit;

namespace FitToMarkdown.Markdown.Tests;

public sealed class MarkdownEscaperTests
{
    [Fact]
    public void EscapeTableCell_should_escape_pipe_characters()
    {
        var result = MarkdownEscaper.EscapeTableCell("value|with|pipes");

        result.Should().Be(@"value\|with\|pipes");
    }

    [Fact]
    public void EscapeTableCell_should_replace_newlines_with_spaces()
    {
        var result = MarkdownEscaper.EscapeTableCell("line1\nline2\r\nline3");

        result.Should().Be("line1 line2 line3");
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    public void EscapeTableCell_should_handle_null_and_empty(string? input, string expected)
    {
        var result = MarkdownEscaper.EscapeTableCell(input!);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("*bold*", @"\*bold\*")]
    [InlineData("_italic_", @"\_italic\_")]
    [InlineData("# heading", @"\# heading")]
    [InlineData("pipe|char", @"pipe\|char")]
    [InlineData("[link](url)", @"\[link\]\(url\)")]
    [InlineData("back\\slash", @"back\\slash")]
    [InlineData("`code`", @"\`code\`")]
    public void EscapeMarkdownText_should_escape_special_characters(string input, string expected)
    {
        var result = MarkdownEscaper.EscapeMarkdownText(input);

        result.Should().Be(expected);
    }

    [Fact]
    public void EscapeMarkdownText_should_handle_null_and_empty()
    {
        MarkdownEscaper.EscapeMarkdownText(null!).Should().BeEmpty();
        MarkdownEscaper.EscapeMarkdownText("").Should().BeEmpty();
    }

    [Fact]
    public void SanitizeFitString_should_remove_null_terminators()
    {
        var result = MarkdownEscaper.SanitizeFitString("Forerunner\0265\0");

        result.Should().Be("Forerunner265");
    }

    [Fact]
    public void SanitizeFitString_should_strip_control_characters_but_keep_tabs()
    {
        var input = "Name\t" + "Value" + (char)0x01 + (char)0x02 + "\nDone";
        var result = MarkdownEscaper.SanitizeFitString(input);

        result.Should().Be("Name\tValue\nDone");
    }

    [Fact]
    public void SanitizeFitString_should_return_empty_for_null_input()
    {
        MarkdownEscaper.SanitizeFitString(null).Should().BeEmpty();
    }

    [Fact]
    public void EscapeHeading_should_escape_hash_and_newlines()
    {
        var result = MarkdownEscaper.EscapeHeading("My # Activity\nDetails");

        result.Should().Be(@"My \# Activity Details");
    }

    [Fact]
    public void EscapeHeading_should_handle_empty()
    {
        MarkdownEscaper.EscapeHeading("").Should().BeEmpty();
    }
}
