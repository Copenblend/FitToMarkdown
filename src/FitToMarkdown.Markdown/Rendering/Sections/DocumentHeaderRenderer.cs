using System.Text;
using FitToMarkdown.Core.Documents;
using FitToMarkdown.Markdown.Formatting;

namespace FitToMarkdown.Markdown.Rendering.Sections;

internal static class DocumentHeaderRenderer
{
    internal static void Render(FitMarkdownDocument document, StringBuilder sb)
    {
        var writer = new MarkdownTextWriter();
        writer.AppendHeading(1, MarkdownEscaper.EscapeHeading(document.Title));

        if (document.HeadingTimestampUtc is not null)
        {
            writer.AppendParagraph(MarkdownValueFormatter.FormatTimestampBody(document.HeadingTimestampUtc.Value));
        }

        sb.Append(writer.ToString());
    }
}
