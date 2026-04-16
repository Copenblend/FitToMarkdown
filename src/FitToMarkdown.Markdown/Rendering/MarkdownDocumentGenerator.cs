using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Markdown.Formatting;
using FitToMarkdown.Markdown.Validation;

namespace FitToMarkdown.Markdown.Rendering;

/// <summary>
/// Generates a markdown document from a markdown-ready projection.
/// </summary>
public sealed class MarkdownDocumentGenerator : IMarkdownDocumentGenerator
{
    private readonly MarkdownRenderCoordinator _coordinator;
    private readonly MarkdownValidationService _validator;
    private readonly SuggestedFileNameFactory _fileNameFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkdownDocumentGenerator"/> class.
    /// </summary>
    /// <param name="coordinator">The rendering coordinator.</param>
    /// <param name="validator">The markdown validation service.</param>
    /// <param name="fileNameFactory">The file name factory.</param>
    internal MarkdownDocumentGenerator(
        MarkdownRenderCoordinator coordinator,
        MarkdownValidationService validator,
        SuggestedFileNameFactory fileNameFactory)
    {
        _coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _fileNameFactory = fileNameFactory ?? throw new ArgumentNullException(nameof(fileNameFactory));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkdownDocumentGenerator"/> class with default dependencies.
    /// </summary>
    public MarkdownDocumentGenerator()
        : this(new MarkdownRenderCoordinator(), new MarkdownValidationService(), new SuggestedFileNameFactory())
    {
    }

    /// <inheritdoc />
    public ValueTask<MarkdownDocumentResult> GenerateAsync(FitMarkdownDocument document, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        cancellationToken.ThrowIfCancellationRequested();

        var markdown = _coordinator.Render(document, cancellationToken);

        _ = _validator.Validate(markdown, out _);

        var suggestedFileName = _fileNameFactory.Generate(document);

        var renderedSections = document.SectionDecisions
            .Where(d => d.ShouldRender)
            .Select(d => d.Section)
            .ToList();

        int approximateTokens = (markdown.Length + 3) / 4;

        var result = new MarkdownDocumentResult
        {
            Content = markdown,
            SuggestedFileName = suggestedFileName,
            RenderedSections = renderedSections,
            ApproximateTokenCount = approximateTokens,
        };

        return new ValueTask<MarkdownDocumentResult>(result);
    }
}
