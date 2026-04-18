using FitToMarkdown.Cli.Models;
using FitToMarkdown.Core.Abstractions;

namespace FitToMarkdown.Cli.Services;

internal sealed class OutputPathResolver
{
    private static readonly HashSet<string> ReservedNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "CON", "PRN", "AUX", "NUL",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9",
    };

    internal string GetDefaultOutputDirectory(InputTarget target)
    {
        return target.Kind == InputTargetKind.File
            ? Path.GetDirectoryName(target.FullPath)!
            : target.FullPath;
    }

    internal string ResolveOutputFilePath(string outputDirectory, string inputFilePath, MarkdownDocumentResult markdownResult)
    {
        var suggestedFileName = markdownResult.SuggestedFileName;
        var validFileName = IsSafeFileName(suggestedFileName)
            ? suggestedFileName
            : Path.GetFileNameWithoutExtension(inputFilePath) + ".md";

        return Path.Combine(outputDirectory, validFileName);
    }

    private static bool IsSafeFileName(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return false;
        }

        if (Path.IsPathRooted(fileName))
        {
            return false;
        }

        if (fileName.Contains("..", StringComparison.Ordinal))
        {
            return false;
        }

        if (Path.GetFileName(fileName) != fileName)
        {
            return false;
        }

        var invalidChars = Path.GetInvalidFileNameChars();
        if (fileName.IndexOfAny(invalidChars) >= 0)
        {
            return false;
        }

        var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        if (ReservedNames.Contains(nameWithoutExtension))
        {
            return false;
        }

        if (fileName.EndsWith('.') || fileName.EndsWith(' '))
        {
            return false;
        }

        return true;
    }
}
