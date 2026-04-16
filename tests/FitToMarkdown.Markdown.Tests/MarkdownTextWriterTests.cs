using FluentAssertions;
using FitToMarkdown.Markdown.Formatting;
using Xunit;

namespace FitToMarkdown.Markdown.Tests;

public sealed class MarkdownTextWriterTests
{
    [Theory]
    [InlineData(1, "# Title\n")]
    [InlineData(2, "## Section\n")]
    [InlineData(3, "### Sub\n")]
    public void AppendHeading_should_produce_correct_level_syntax(int level, string expectedStart)
    {
        var writer = new MarkdownTextWriter();

        writer.AppendHeading(level, level switch { 1 => "Title", 2 => "Section", _ => "Sub" });

        var result = writer.ToString();
        result.Should().StartWith(expectedStart);
    }

    [Fact]
    public void AppendParagraph_should_add_blank_line_between_blocks()
    {
        var writer = new MarkdownTextWriter();

        writer.AppendHeading(1, "Title");
        writer.AppendParagraph("First paragraph.");
        writer.AppendParagraph("Second paragraph.");

        var result = writer.ToString();
        result.Should().Contain("\n\nFirst paragraph.\n");
        result.Should().Contain("\n\nSecond paragraph.\n");
    }

    [Fact]
    public void AppendTable_should_suppress_empty_columns()
    {
        var writer = new MarkdownTextWriter();
        string[] headers = ["Name", "Value", "Notes"];
        var rows = new List<string[]>
        {
            new[] { "Distance", "10.00", "" },
            new[] { "Duration", "30:00", "" },
        };

        writer.AppendTable(headers, rows);

        var result = writer.ToString();

        // "Notes" column should be suppressed since all values are empty
        result.Should().NotContain("Notes");
        result.Should().Contain("Name");
        result.Should().Contain("Value");
    }

    [Fact]
    public void AppendTable_should_render_pipe_table_with_separator()
    {
        var writer = new MarkdownTextWriter();
        string[] headers = ["Metric", "Value"];
        var rows = new List<string[]>
        {
            new[] { "Distance", "10.00 km" },
        };

        writer.AppendTable(headers, rows);

        var result = writer.ToString();
        result.Should().Contain("| Metric | Value |");
        result.Should().Contain("| --- | --- |");
        result.Should().Contain("| Distance | 10.00 km |");
    }

    [Fact]
    public void AppendFencedCodeBlock_should_include_language()
    {
        var writer = new MarkdownTextWriter();

        writer.AppendFencedCodeBlock("timestamp,hr\n2024-06-15,150", "csv");

        var result = writer.ToString();
        result.Should().Contain("```csv\n");
        result.Should().Contain("```\n");
        result.Should().Contain("timestamp,hr");
    }

    [Fact]
    public void AppendBulletItem_should_produce_dash_syntax()
    {
        var writer = new MarkdownTextWriter();

        writer.AppendBulletItem("Item one");
        writer.AppendBulletItem("Item two");

        var result = writer.ToString();
        result.Should().Contain("- Item one\n");
        result.Should().Contain("- Item two\n");
    }

    [Fact]
    public void ToString_should_use_lf_line_endings()
    {
        var writer = new MarkdownTextWriter();
        writer.AppendHeading(1, "Title");
        writer.AppendParagraph("Content");

        var result = writer.ToString();

        result.Should().NotContain("\r\n");
    }

    [Fact]
    public void ToString_should_end_with_trailing_newline()
    {
        var writer = new MarkdownTextWriter();
        writer.AppendHeading(1, "Title");

        var result = writer.ToString();

        result.Should().EndWith("\n");
    }

    [Fact]
    public void AppendTable_should_return_without_output_when_headers_empty()
    {
        var writer = new MarkdownTextWriter();
        // All columns empty → filtered to zero columns
        string[] headers = ["A"];
        var rows = new List<string[]> { new[] { "" } };

        writer.AppendTable(headers, rows);

        var result = writer.ToString();
        result.Should().NotContain("|");
    }
}
