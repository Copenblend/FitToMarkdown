using FitToMarkdown.Core.Abstractions;

namespace FitToMarkdown.Cli.Models;

/// <summary>
/// Immutable handoff object produced by the bounded-parallel prepare stage.
/// </summary>
internal sealed record PreparedConversionArtifact
{
    /// <summary>The original discovery order index.</summary>
    public int SourceOrder { get; init; }

    /// <summary>The full path of the input .fit file.</summary>
    public string InputFilePath { get; init; } = string.Empty;

    /// <summary>The generated markdown result from the prepare stage.</summary>
    public MarkdownDocumentResult MarkdownResult { get; init; } = null!;

    /// <summary>Warning lines collected during preparation.</summary>
    public IReadOnlyList<string> WarningLines { get; init; } = [];

    /// <summary>Time elapsed during the prepare stage for this file.</summary>
    public TimeSpan PreparationElapsed { get; init; }
}
