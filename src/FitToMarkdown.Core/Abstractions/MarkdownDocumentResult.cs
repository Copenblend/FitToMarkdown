using FitToMarkdown.Core.Enums;

namespace FitToMarkdown.Core.Abstractions;

/// <summary>
/// Represents the final markdown artifact emitted by the Markdown project.
/// </summary>
public sealed record MarkdownDocumentResult
{
    /// <summary>The generated markdown content.</summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>The suggested output file name.</summary>
    public string SuggestedFileName { get; init; } = string.Empty;

    /// <summary>The sections that were rendered in the output.</summary>
    public IReadOnlyList<FitMarkdownSectionKey> RenderedSections { get; init; } = [];

    /// <summary>Approximate token count of the generated content.</summary>
    public int? ApproximateTokenCount { get; init; }
}
