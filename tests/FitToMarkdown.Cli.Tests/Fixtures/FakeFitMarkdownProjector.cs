using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Documents;

namespace FitToMarkdown.Cli.Tests.Fixtures;

internal sealed class FakeFitMarkdownProjector : IFitMarkdownProjector
{
    public ValueTask<FitMarkdownDocument> ProjectAsync(FitFileDocument document, MarkdownDocumentOptions options, CancellationToken cancellationToken = default)
    {
        return new ValueTask<FitMarkdownDocument>(new FitMarkdownDocument());
    }
}
