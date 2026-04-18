using Markdig;
using Markdig.Syntax;

namespace FitToMarkdown.Markdown.Validation;

internal static class MarkdownStructureInspector
{
    internal static IReadOnlyList<string> Inspect(string markdown)
    {
        var issues = new List<string>();
        var pipeline = MarkdownPipelineFactory.CreateValidationPipeline();
        var document = Markdig.Markdown.Parse(markdown, pipeline);

        var headings = document.Descendants<HeadingBlock>().ToList();

        // Check: exactly 1 H1 heading
        int h1Count = headings.Count(h => h.Level == 1);
        if (h1Count == 0)
            issues.Add("[Structure] Missing H1 document title.");
        else if (h1Count > 1)
            issues.Add($"[Structure] Multiple H1 headings found ({h1Count}). Expected exactly one.");

        // Check: heading hierarchy (no skipping levels)
        int prevLevel = 0;
        foreach (var heading in headings)
        {
            if (heading.Level > prevLevel + 1 && prevLevel > 0)
                issues.Add($"[Structure] Heading level skipped: H{prevLevel} followed by H{heading.Level}.");
            prevLevel = heading.Level;
        }

        // Check: no empty headings
        int emptyHeadings = headings.Count(h => h.Inline is null || !h.Inline.Any());
        if (emptyHeadings > 0)
            issues.Add($"[Structure] Found {emptyHeadings} empty heading(s).");

        // Check: no empty paragraphs
        int emptyParas = document.Descendants<ParagraphBlock>()
            .Count(p => p.Inline is null || !p.Inline.Any());
        if (emptyParas > 0)
            issues.Add($"[Structure] Found {emptyParas} empty paragraph(s).");

        return issues;
    }
}
