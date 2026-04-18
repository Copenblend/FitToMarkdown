using System.Text;
using System.Text.RegularExpressions;
using FitToMarkdown.Core.Documents;

namespace FitToMarkdown.Markdown.Formatting;

internal sealed partial class SuggestedFileNameFactory
{
    private const int MaxBaseLength = 96;

    internal string Generate(FitMarkdownDocument document)
    {
        // Timestamp part: YYYYMMDD_hhmmss
        string timestampPart;
        if (document.HeadingTimestampUtc is not null)
        {
            timestampPart = document.HeadingTimestampUtc.Value.ToUniversalTime().ToString("yyyyMMdd_HHmmss");
        }
        else
        {
            timestampPart = "unknown_time";
        }

        // Sport or file type part
        string? sport = document.Frontmatter.Sport?.ToString();
        string? fileType = document.Frontmatter.FileType?.ToString();
        string sportPart = !string.IsNullOrEmpty(sport) ? sport : (!string.IsNullOrEmpty(fileType) ? fileType : "activity");

        // Format: YYYYMMDD_hhmmss_{sport}
        string baseName = $"{timestampPart}_{Sanitize(sportPart)}";

        if (baseName.Length > MaxBaseLength)
            baseName = baseName[..MaxBaseLength];

        // Trim trailing underscores
        baseName = baseName.TrimEnd('_');

        if (string.IsNullOrEmpty(baseName))
            baseName = "fit_document";

        return baseName + ".md";
    }

    private static string Sanitize(string input)
    {
        // Insert underscores before uppercase letters in PascalCase (e.g. "CrossCountrySkiing" → "Cross_Country_Skiing")
        var sb = new StringBuilder(input.Length + 4);
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (i > 0 && char.IsUpper(c) && !char.IsUpper(input[i - 1]))
                sb.Append('_');
            sb.Append(c);
        }

        // Replace any non-alphanumeric-underscore chars
        string result = InvalidFileCharsRegex().Replace(sb.ToString(), "_");

        // Collapse repeated underscores
        result = RepeatedUnderscoreRegex().Replace(result, "_");

        return result.Trim('_');
    }

    [GeneratedRegex(@"[^\w]+")]
    private static partial Regex InvalidFileCharsRegex();

    [GeneratedRegex(@"_{2,}")]
    private static partial Regex RepeatedUnderscoreRegex();
}
