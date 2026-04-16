using System.Text;
using System.Text.RegularExpressions;
using FitToMarkdown.Core.Documents;

namespace FitToMarkdown.Markdown.Formatting;

internal sealed partial class SuggestedFileNameFactory
{
    private const int MaxBaseLength = 96;

    internal string Generate(FitMarkdownDocument document)
    {
        var parts = new List<string>();

        // Timestamp part
        if (document.HeadingTimestampUtc is not null)
        {
            parts.Add(document.HeadingTimestampUtc.Value.ToUniversalTime().ToString("yyyyMMdd-HHmmss"));
        }
        else
        {
            parts.Add("unknown-time");
        }

        // Sport or file type part
        string? sport = document.Frontmatter.Sport?.ToString();
        string? fileType = document.Frontmatter.FileType?.ToString();
        parts.Add(!string.IsNullOrEmpty(sport) ? sport : (!string.IsNullOrEmpty(fileType) ? fileType : "activity"));

        // Sub-sport, if present
        string? subSport = document.Frontmatter.SubSport?.ToString();
        if (!string.IsNullOrEmpty(subSport) && !string.Equals(subSport, "Generic", StringComparison.OrdinalIgnoreCase))
        {
            parts.Add(subSport);
        }

        string joined = string.Join("-", parts);
        string kebab = ToKebabCase(joined);

        if (kebab.Length > MaxBaseLength)
            kebab = kebab[..MaxBaseLength];

        // Trim trailing dashes
        kebab = kebab.TrimEnd('-');

        if (string.IsNullOrEmpty(kebab))
            kebab = "fit-document";

        return kebab + ".md";
    }

    private static string ToKebabCase(string input)
    {
        // Lowercase
        string lower = input.ToLowerInvariant();

        // Replace whitespace and punctuation runs with single dash
        string cleaned = WhitespaceOrPunctuationRegex().Replace(lower, "-");

        // Collapse repeated dashes
        cleaned = RepeatedDashRegex().Replace(cleaned, "-");

        // Trim leading/trailing dashes
        cleaned = cleaned.Trim('-');

        return cleaned;
    }

    [GeneratedRegex(@"[\s_.,;:!?/\\(){}[\]]+")]
    private static partial Regex WhitespaceOrPunctuationRegex();

    [GeneratedRegex(@"-{2,}")]
    private static partial Regex RepeatedDashRegex();
}
