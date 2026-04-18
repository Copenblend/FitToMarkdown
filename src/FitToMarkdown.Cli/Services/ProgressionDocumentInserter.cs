using System.Text;
using System.Text.RegularExpressions;

namespace FitToMarkdown.Cli.Services;

/// <summary>
/// Inserts a new activity into an existing Progression markdown document
/// in chronological order based on timestamps found in the content.
/// </summary>
internal sealed partial class ProgressionDocumentInserter
{
    /// <summary>
    /// Inserts new activity content into an existing progression document at the correct chronological position.
    /// </summary>
    /// <param name="existingContent">The current content of the progression file.</param>
    /// <param name="newActivityContent">The rendered markdown for the new activity.</param>
    /// <param name="newActivityTimestamp">The timestamp of the new activity.</param>
    /// <returns>The merged document content.</returns>
    internal string InsertChronologically(string existingContent, string newActivityContent, DateTimeOffset newActivityTimestamp)
    {
        var sections = SplitIntoSections(existingContent);

        if (sections.Count == 0)
        {
            // Empty or header-only document; just append
            var sb = new StringBuilder(existingContent.TrimEnd());
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine(newActivityContent);
            return sb.ToString();
        }

        // Find insertion point
        int insertIndex = sections.Count; // default: append at end
        for (int i = 0; i < sections.Count; i++)
        {
            var sectionTimestamp = ExtractTimestamp(sections[i].Content);
            if (sectionTimestamp.HasValue && sectionTimestamp.Value > newActivityTimestamp)
            {
                insertIndex = i;
                break;
            }
        }

        // Rebuild document
        var result = new StringBuilder();

        // Write the progression header if present
        var headerMatch = ProgressionHeaderRegex().Match(existingContent);
        if (headerMatch.Success)
        {
            result.AppendLine(headerMatch.Value.TrimEnd());
            result.AppendLine();
        }

        for (int i = 0; i < sections.Count; i++)
        {
            if (i == insertIndex)
            {
                if (result.Length > 0 && !EndsWithBlankLine(result))
                {
                    result.AppendLine();
                }

                result.AppendLine("---");
                result.AppendLine();
                result.AppendLine(newActivityContent.TrimEnd());
                result.AppendLine();
            }

            if (i > 0 || headerMatch.Success)
            {
                result.AppendLine("---");
                result.AppendLine();
            }

            result.AppendLine(sections[i].Content.TrimEnd());
            result.AppendLine();
        }

        if (insertIndex >= sections.Count)
        {
            result.AppendLine("---");
            result.AppendLine();
            result.AppendLine(newActivityContent.TrimEnd());
            result.AppendLine();
        }

        return result.ToString();
    }

    private List<ActivitySection> SplitIntoSections(string content)
    {
        var sections = new List<ActivitySection>();

        // Remove the progression header line
        var withoutHeader = ProgressionHeaderRegex().Replace(content, "").TrimStart('\r', '\n');

        // Split on horizontal rule separators
        var parts = HorizontalRuleRegex().Split(withoutHeader);

        foreach (var part in parts)
        {
            var trimmed = part.Trim();
            if (trimmed.Length > 0)
            {
                var timestamp = ExtractTimestamp(trimmed);
                sections.Add(new ActivitySection(trimmed, timestamp));
            }
        }

        return sections;
    }

    private static DateTimeOffset? ExtractTimestamp(string content)
    {
        // Look for a YAML frontmatter time_created_utc field
        var yamlMatch = YamlTimestampRegex().Match(content);
        if (yamlMatch.Success && TryParseTimestamp(yamlMatch.Groups[1].Value.Trim(), out var yamlTs))
        {
            return yamlTs;
        }

        // Look for a heading with a date pattern like "## Running — 2024-06-15 08:30:00 UTC"
        var headingMatch = HeadingTimestampRegex().Match(content);
        if (headingMatch.Success && TryParseTimestamp(headingMatch.Groups[1].Value.Trim(), out var headingTs))
        {
            return headingTs;
        }

        return null;
    }

    private static bool TryParseTimestamp(string value, out DateTimeOffset result)
    {
        // Handle "UTC" suffix by converting to "+00:00"
        if (value.EndsWith("UTC", StringComparison.OrdinalIgnoreCase))
        {
            var withoutUtc = value[..^3].TrimEnd() + "+00:00";
            if (DateTimeOffset.TryParse(withoutUtc, out result))
                return true;
        }

        return DateTimeOffset.TryParse(value, out result);
    }

    private static bool EndsWithBlankLine(StringBuilder sb)
    {
        if (sb.Length < 2) return false;
        return sb[sb.Length - 1] == '\n' && (sb[sb.Length - 2] == '\n' || (sb.Length >= 3 && sb[sb.Length - 2] == '\r' && sb[sb.Length - 3] == '\n'));
    }

    [GeneratedRegex(@"^#\s+.+Progression\s*$", RegexOptions.Multiline)]
    private static partial Regex ProgressionHeaderRegex();

    [GeneratedRegex(@"^\s*---\s*$", RegexOptions.Multiline)]
    private static partial Regex HorizontalRuleRegex();

    [GeneratedRegex(@"time_created_utc:\s*(.+)$", RegexOptions.Multiline)]
    private static partial Regex YamlTimestampRegex();

    [GeneratedRegex(@"^##?\s+.*?(\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2}\s+\w+)", RegexOptions.Multiline)]
    private static partial Regex HeadingTimestampRegex();

    private sealed record ActivitySection(string Content, DateTimeOffset? Timestamp);
}
