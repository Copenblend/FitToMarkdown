using Markdig;

namespace FitToMarkdown.Markdown.Validation;

internal sealed class MarkdownValidationService
{
    internal bool Validate(string markdown, out IReadOnlyList<string> issues)
    {
        var allIssues = new List<string>();

        // Parse to check for structural errors
        try
        {
            var pipeline = MarkdownPipelineFactory.CreateValidationPipeline();
            var document = Markdig.Markdown.Parse(markdown, pipeline);
            if (document.Count == 0)
                allIssues.Add("[Content] Document is empty after parsing.");
        }
        catch (Exception ex)
        {
            allIssues.Add($"[Parse] Markdown parse error: {ex.Message}");
            issues = allIssues;
            return false;
        }

        // Structural inspection
        var structureIssues = MarkdownStructureInspector.Inspect(markdown);
        allIssues.AddRange(structureIssues);

        issues = allIssues;
        return allIssues.Count == 0;
    }
}
