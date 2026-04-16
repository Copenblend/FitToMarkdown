using System.Text;

namespace FitToMarkdown.Markdown.Formatting;

internal sealed class MarkdownTextWriter
{
    private readonly StringBuilder _sb = new();
    private bool _needsBlankLine;

    private void EnsureBlankLine()
    {
        if (_needsBlankLine)
        {
            _sb.Append('\n');
            _needsBlankLine = false;
        }
    }

    internal MarkdownTextWriter AppendHeading(int level, string text)
    {
        EnsureBlankLine();
        _sb.Append('#', level);
        _sb.Append(' ');
        _sb.Append(MarkdownEscaper.EscapeHeading(text));
        _sb.Append('\n');
        _needsBlankLine = true;
        return this;
    }

    internal MarkdownTextWriter AppendParagraph(string text)
    {
        EnsureBlankLine();
        _sb.Append(text);
        _sb.Append('\n');
        _needsBlankLine = true;
        return this;
    }

    internal MarkdownTextWriter AppendBulletItem(string text)
    {
        _sb.Append("- ");
        _sb.Append(text);
        _sb.Append('\n');
        return this;
    }

    internal MarkdownTextWriter AppendNumberedItem(int number, string text)
    {
        _sb.Append(number);
        _sb.Append(". ");
        _sb.Append(text);
        _sb.Append('\n');
        return this;
    }

    internal MarkdownTextWriter AppendTable(string[] headers, List<string[]> rows)
    {
        var (filteredHeaders, filteredRows) = MarkdownTableLayoutService.FilterEmptyColumns(headers, rows);

        if (filteredHeaders.Length == 0)
            return this;

        EnsureBlankLine();

        // Header row
        _sb.Append('|');
        foreach (var header in filteredHeaders)
        {
            _sb.Append(' ');
            _sb.Append(MarkdownEscaper.EscapeTableCell(header));
            _sb.Append(" |");
        }
        _sb.Append('\n');

        // Separator row
        _sb.Append('|');
        for (int i = 0; i < filteredHeaders.Length; i++)
        {
            _sb.Append(" --- |");
        }
        _sb.Append('\n');

        // Data rows
        foreach (var row in filteredRows)
        {
            _sb.Append('|');
            for (int i = 0; i < filteredHeaders.Length; i++)
            {
                _sb.Append(' ');
                _sb.Append(i < row.Length ? MarkdownEscaper.EscapeTableCell(row[i]) : string.Empty);
                _sb.Append(" |");
            }
            _sb.Append('\n');
        }

        _needsBlankLine = true;
        return this;
    }

    internal MarkdownTextWriter AppendFencedCodeBlock(string content, string language)
    {
        EnsureBlankLine();
        _sb.Append("```");
        _sb.Append(language);
        _sb.Append('\n');
        _sb.Append(content);
        if (content.Length > 0 && content[^1] != '\n')
        {
            _sb.Append('\n');
        }
        _sb.Append("```\n");
        _needsBlankLine = true;
        return this;
    }

    internal MarkdownTextWriter AppendBlankLine()
    {
        _sb.Append('\n');
        _needsBlankLine = false;
        return this;
    }

    internal MarkdownTextWriter AppendRaw(string text)
    {
        _sb.Append(text);
        _needsBlankLine = false;
        return this;
    }

    public override string ToString()
    {
        var result = _sb.ToString().Replace("\r\n", "\n");

        // Ensure trailing newline
        if (result.Length > 0 && result[^1] != '\n')
        {
            result += '\n';
        }

        return result;
    }
}
