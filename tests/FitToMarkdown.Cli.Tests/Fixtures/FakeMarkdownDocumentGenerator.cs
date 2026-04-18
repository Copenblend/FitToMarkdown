using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Documents;

namespace FitToMarkdown.Cli.Tests.Fixtures;

internal sealed class FakeMarkdownDocumentGenerator : IMarkdownDocumentGenerator
{
    private int _callCount;

    public string ContentToReturn { get; set; } = "# Test Activity";
    public string SuggestedFileNameToReturn { get; set; } = "test.md";

    /// <summary>
    /// When true, all calls return the exact same filename (useful for duplicate detection tests).
    /// When false (default), appends a counter suffix to avoid duplicate filenames across calls.
    /// </summary>
    public bool ForceSameFileName { get; set; }

    public ValueTask<MarkdownDocumentResult> GenerateAsync(FitMarkdownDocument document, CancellationToken cancellationToken = default)
    {
        var count = Interlocked.Increment(ref _callCount);
        var fileName = !ForceSameFileName && count > 1
            ? Path.GetFileNameWithoutExtension(SuggestedFileNameToReturn) + $"_{count}" + Path.GetExtension(SuggestedFileNameToReturn)
            : SuggestedFileNameToReturn;

        return new ValueTask<MarkdownDocumentResult>(new MarkdownDocumentResult
        {
            Content = ContentToReturn,
            SuggestedFileName = fileName,
        });
    }
}
