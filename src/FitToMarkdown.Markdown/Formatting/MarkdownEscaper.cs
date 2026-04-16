using System.Text;

namespace FitToMarkdown.Markdown.Formatting;

internal static class MarkdownEscaper
{
    internal static string EscapeTableCell(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return text
            .Replace("|", "\\|")
            .Replace("\n", " ")
            .Replace("\r", "");
    }

    internal static string EscapeMarkdownText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        var sb = new StringBuilder(text.Length + 10);
        foreach (char c in text)
        {
            if (c is '\\' or '`' or '*' or '_' or '{' or '}'
                or '[' or ']' or '(' or ')' or '#' or '+'
                or '-' or '.' or '!' or '|')
            {
                sb.Append('\\');
            }
            sb.Append(c);
        }
        return sb.ToString();
    }

    internal static string SanitizeFitString(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        var sb = new StringBuilder(text.Length);
        foreach (char c in text)
        {
            if (c == '\0')
                continue;

            // Strip control characters except tab, LF, CR
            if (char.IsControl(c) && c is not '\t' and not '\n' and not '\r')
                continue;

            sb.Append(c);
        }
        return sb.ToString();
    }

    internal static string EscapeHeading(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        // Headings: escape characters that break heading parsing
        return text
            .Replace("\n", " ")
            .Replace("\r", "")
            .Replace("#", "\\#");
    }
}
