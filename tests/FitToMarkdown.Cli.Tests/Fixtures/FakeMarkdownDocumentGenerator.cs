using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Documents;

namespace FitToMarkdown.Cli.Tests.Fixtures;

internal sealed class FakeMarkdownDocumentGenerator : IMarkdownDocumentGenerator
{
    public string ContentToReturn { get; set; } = "# Test Activity";
    public string SuggestedFileNameToReturn { get; set; } = "test.md";

    public ValueTask<MarkdownDocumentResult> GenerateAsync(FitMarkdownDocument document, CancellationToken cancellationToken = default)
    {
        return new ValueTask<MarkdownDocumentResult>(new MarkdownDocumentResult
        {
            Content = ContentToReturn,
            SuggestedFileName = SuggestedFileNameToReturn,
        });
    }
}
