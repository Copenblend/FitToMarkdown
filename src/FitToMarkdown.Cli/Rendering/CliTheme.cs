using Spectre.Console;

namespace FitToMarkdown.Cli.Rendering;

internal static class CliTheme
{
    internal static Style Primary => new(Color.Cyan1);
    internal static Style PrimaryBold => new(Color.Cyan1, decoration: Decoration.Bold);
    internal static Style Success => new(Color.Green);
    internal static Style Warning => new(Color.Yellow);
    internal static Style Error => new(Color.Red);
    internal static Style Dim => new(Color.Grey);
    internal static Style HighlightStyle => new(Color.Cyan1, decoration: Decoration.Bold);

    internal static Color PrimaryColor => Color.Cyan1;
    internal static Color SuccessColor => Color.Green;
    internal static Color WarningColor => Color.Yellow;
    internal static Color ErrorColor => Color.Red;
    internal static Color DimColor => Color.Grey;

    internal static TableBorder RoundedBorder => TableBorder.Rounded;
}
