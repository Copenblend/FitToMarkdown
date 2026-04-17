using FitToMarkdown.Cli.Models;
using FitToMarkdown.Core.Abstractions;

namespace FitToMarkdown.Cli.Services;

internal sealed class OutputPathResolver
{
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

        return true;
    }
}
