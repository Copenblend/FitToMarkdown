using FitToMarkdown.Core.Abstractions;

namespace FitToMarkdown.Cli.Tests.Fixtures;

internal static class MarkdownResultFactory
{
    internal static MarkdownDocumentResult Create(string content = "# Test", string fileName = "test.md")
    {
        return new MarkdownDocumentResult
        {
            Content = content,
            SuggestedFileName = fileName,
        };
    }
}
