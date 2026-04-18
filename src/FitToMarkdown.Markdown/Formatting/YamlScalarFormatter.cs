using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FitToMarkdown.Markdown.Formatting;

internal static partial class YamlScalarFormatter
{
    [GeneratedRegex(@"^\d{4}-\d{2}", RegexOptions.None, matchTimeoutMilliseconds: 100)]
    private static partial Regex IsoDateLikePattern();

    internal static string? FormatScalar(string key, object? value)
    {
        if (value is null)
            return null;

        string stringValue = value switch
        {
            string s => s,
            bool b => b ? "true" : "false",
            DateTime dt => dt.ToString("O", CultureInfo.InvariantCulture),
            DateTimeOffset dto => dto.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
            double d => d.ToString("G", CultureInfo.InvariantCulture),
            float f => f.ToString("G", CultureInfo.InvariantCulture),
            _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty,
        };

        if (string.IsNullOrEmpty(stringValue))
            return null;

        if (NeedsQuoting(stringValue))
            return $"{key}: {QuoteYamlString(stringValue)}\n";

        return $"{key}: {stringValue}\n";
    }

    internal static bool NeedsQuoting(string value)
    {
        if (string.IsNullOrEmpty(value))
            return true;

        // Boolean-like values
        var lower = value.ToLowerInvariant();
        if (lower is "true" or "false" or "yes" or "no" or "on" or "off" or "null" or "~")
            return true;

        // Starts with special characters
        if (value[0] is '-' or '?' or ':' or ' ' or '\t'
            or '{' or '}' or '[' or ']' or ',' or '&'
            or '*' or '#' or '!' or '|' or '>' or '\''
            or '"' or '%' or '@' or '`')
            return true;

        // Contains characters that need quoting
        foreach (char c in value)
        {
            if (c is ':' or '#' or '\n' or '\r' or '\t' or '"' or '\'' or '\\')
                return true;
        }

        // Numeric-like values that should remain as strings
        if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
            return true;

        // ISO date-like values (e.g. "2024-06-15") that YAML parsers auto-convert
        if (IsoDateLikePattern().IsMatch(value))
            return true;

        return false;
    }

    internal static string QuoteYamlString(string value)
    {
        var sb = new StringBuilder(value.Length + 4);
        sb.Append('"');
        foreach (char c in value)
        {
            switch (c)
            {
                case '\\': sb.Append("\\\\"); break;
                case '"': sb.Append("\\\""); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                case '\0': break; // strip null terminators
                default: sb.Append(c); break;
            }
        }
        sb.Append('"');
        return sb.ToString();
    }

    internal static string? FormatNumeric(string key, double? value, int decimals)
    {
        if (value is null)
            return null;

        string format = $"F{decimals}";
        return $"{key}: {value.Value.ToString(format, CultureInfo.InvariantCulture)}\n";
    }

    internal static string? FormatInteger(string key, int? value)
    {
        if (value is null)
            return null;

        return $"{key}: {value.Value.ToString(CultureInfo.InvariantCulture)}\n";
    }
}
