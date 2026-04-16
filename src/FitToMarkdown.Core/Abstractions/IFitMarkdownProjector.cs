using FitToMarkdown.Core.Documents;

namespace FitToMarkdown.Core.Abstractions;

/// <summary>
/// Defines the projection boundary that converts raw parsed documents into markdown-ready summary contracts.
/// </summary>
public interface IFitMarkdownProjector
{
    /// <summary>
    /// Projects a parsed FIT file document into a markdown-ready document.
    /// </summary>
    /// <param name="document">The parsed FIT file document.</param>
    /// <param name="options">Projection rules.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The markdown-ready document projection.</returns>
    ValueTask<FitMarkdownDocument> ProjectAsync(FitFileDocument document, MarkdownDocumentOptions options, CancellationToken cancellationToken = default);
}
