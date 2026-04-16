using FitToMarkdown.Core.Enums;

namespace FitToMarkdown.Core.Summaries;

/// <summary>
/// Represents the explicit decision to render or omit one markdown section.
/// </summary>
public sealed record SectionRenderDecision
{
    /// <summary>The markdown section this decision applies to.</summary>
    public FitMarkdownSectionKey Section { get; init; }

    /// <summary>Display order of this section.</summary>
    public int Order { get; init; }

    /// <summary>Whether the section should be rendered.</summary>
    public bool ShouldRender { get; init; }

    /// <summary>Number of items the section would contain.</summary>
    public int? ItemCount { get; init; }

    /// <summary>Reason the section was omitted, if applicable.</summary>
    public string? OmissionReason { get; init; }
}
