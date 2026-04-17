using Spectre.Console;

namespace FitToMarkdown.Cli.Rendering;

internal static class CliMarkup
{
    internal static string Escape(string? value)
    {
        return Markup.Escape(value ?? string.Empty);
    }

    internal static string LinkOrText(string path)
    {
        if (path.Contains('[') || path.Contains(']'))
        {
            return Markup.Escape(path);
        }

        return $"[link={Markup.Escape(path)}]{Markup.Escape(path)}[/]";
    }

    internal static string DimOrFallback(string? value, string fallback = "n/a")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return $"[grey]{Markup.Escape(fallback)}[/]";
        }

        return $"[dim]{Markup.Escape(value)}[/]";
    }
}
