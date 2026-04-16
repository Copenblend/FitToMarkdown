using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Documents;

namespace FitToMarkdown.Markdown.Projection;

/// <summary>
/// Projects a parsed FIT file document into a markdown-ready document.
/// </summary>
public sealed class FitMarkdownProjector : IFitMarkdownProjector
{
    private readonly DocumentProjectionCoordinator _coordinator;

    /// <summary>
    /// Initializes a new instance of the <see cref="FitMarkdownProjector"/> class.
    /// </summary>
    /// <param name="coordinator">The projection coordinator.</param>
    internal FitMarkdownProjector(DocumentProjectionCoordinator coordinator)
    {
        _coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FitMarkdownProjector"/> class with default dependencies.
    /// </summary>
    public FitMarkdownProjector()
        : this(new DocumentProjectionCoordinator())
    {
    }

    /// <inheritdoc />
    public ValueTask<FitMarkdownDocument> ProjectAsync(FitFileDocument document, MarkdownDocumentOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(options);
        cancellationToken.ThrowIfCancellationRequested();

        var result = _coordinator.Project(document, options, cancellationToken);
        return new ValueTask<FitMarkdownDocument>(result);
    }
}
