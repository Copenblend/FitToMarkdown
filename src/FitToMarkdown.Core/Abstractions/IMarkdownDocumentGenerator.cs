using FitToMarkdown.Core.Documents;

namespace FitToMarkdown.Core.Abstractions;

/// <summary>
/// Defines the markdown rendering boundary implemented by <c>FitToMarkdown.Markdown</c>.
/// </summary>
public interface IMarkdownDocumentGenerator
{
    /// <summary>
    /// Generates a markdown document from a markdown-ready projection.
    /// </summary>
    /// <param name="document">The markdown-ready document projection.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated markdown result.</returns>
    ValueTask<MarkdownDocumentResult> GenerateAsync(FitMarkdownDocument document, CancellationToken cancellationToken = default);
}
