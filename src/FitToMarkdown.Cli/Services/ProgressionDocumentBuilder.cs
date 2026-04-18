using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using FitToMarkdown.Cli.Models;
using FitToMarkdown.Core.Abstractions;

namespace FitToMarkdown.Cli.Services;

/// <summary>
/// Builds a Progression markdown document for a single sport by combining
/// individual activity markdown documents in chronological order.
/// </summary>
internal sealed partial class ProgressionDocumentBuilder
{
    private readonly IFitFileParser _parser;
    private readonly IFitMarkdownProjector _projector;
    private readonly IMarkdownDocumentGenerator _generator;
    private readonly FitParseOptionsFactory _parseOptionsFactory;
    private readonly MarkdownOptionsFactory _optionsFactory;

    internal ProgressionDocumentBuilder(
        IFitFileParser parser,
        IFitMarkdownProjector projector,
        IMarkdownDocumentGenerator generator,
        FitParseOptionsFactory parseOptionsFactory,
        MarkdownOptionsFactory optionsFactory)
    {
        _parser = parser;
        _projector = projector;
        _generator = generator;
        _parseOptionsFactory = parseOptionsFactory;
        _optionsFactory = optionsFactory;
    }

    /// <summary>
    /// Builds a progression document for the given sport groups.
    /// </summary>
    internal async Task<(string Content, ProgressionBuildResult Result)> BuildAsync(
        IReadOnlyList<SportGroup> sportGroups,
        string progressionLabel,
        string outputDirectory,
        CancellationToken cancellationToken,
        Action<int, string>? onFileProcessed = null)
    {
        var stopwatch = Stopwatch.StartNew();
        var sb = new StringBuilder();
        var warnings = new List<string>();
        var failedFiles = new List<string>();
        var activityCount = 0;
        var processedCount = 0;

        // Write progression header
        sb.AppendLine($"# {progressionLabel} Progression");
        sb.AppendLine();

        var orderedFiles = sportGroups
            .SelectMany(g => g.Files)
            .OrderBy(f => f.LastWriteTimeUtc)
            .ToList();

        foreach (var file in orderedFiles)
        {
            cancellationToken.ThrowIfCancellationRequested();
            processedCount++;
            onFileProcessed?.Invoke(processedCount, file.FileName);

            try
            {
                var parseOptions = _parseOptionsFactory.CreateForConvert();
                var parseResult = await _parser.ParseFileAsync(file.FullPath, parseOptions, cancellationToken).ConfigureAwait(false);

                if (parseResult.Document is null)
                {
                    failedFiles.Add(file.FullPath);
                    var reason = parseResult.FatalError?.Message ?? "No document returned.";
                    warnings.Add($"Skipped '{file.FileName}': {reason}");
                    continue;
                }

                var options = _optionsFactory.CreateFor(parseResult);
                var markdownDoc = await _projector.ProjectAsync(parseResult.Document, options, cancellationToken).ConfigureAwait(false);
                var mdResult = await _generator.GenerateAsync(markdownDoc, cancellationToken).ConfigureAwait(false);

                var transformed = TransformForProgression(mdResult.Content, markdownDoc.HeadingTimestampUtc);

                // Add separator between activities
                if (activityCount > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("---");
                    sb.AppendLine();
                }

                sb.AppendLine(transformed);
                activityCount++;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                failedFiles.Add(file.FullPath);
                warnings.Add($"Failed '{file.FileName}': {ex.Message}");
            }
        }

        stopwatch.Stop();

        var outputFileName = $"{SanitizeSportName(progressionLabel)}_Progression_{DateTime.UtcNow:yyyyMMdd}.md";
        var outputPath = Path.Combine(outputDirectory, outputFileName);

        var result = new ProgressionBuildResult
        {
            SportName = progressionLabel,
            OutputPath = outputPath,
            ActivityCount = activityCount,
            ElapsedTime = stopwatch.Elapsed,
            Warnings = warnings,
            FailedFiles = failedFiles,
        };

        return (sb.ToString(), result);
    }

    /// <summary>
    /// Builds the markdown content for a single FIT file to be added to an existing progression.
    /// </summary>
    internal async Task<(string? Content, DateTimeOffset? Timestamp, string? Error)> BuildSingleActivityAsync(
        string fitFilePath,
        CancellationToken cancellationToken)
    {
        try
        {
            var parseOptions = _parseOptionsFactory.CreateForConvert();
            var parseResult = await _parser.ParseFileAsync(fitFilePath, parseOptions, cancellationToken).ConfigureAwait(false);

            if (parseResult.Document is null)
            {
                var reason = parseResult.FatalError?.Message ?? "No document returned.";
                return (null, null, reason);
            }

            var options = _optionsFactory.CreateFor(parseResult);
            var markdownDoc = await _projector.ProjectAsync(parseResult.Document, options, cancellationToken).ConfigureAwait(false);
            var mdResult = await _generator.GenerateAsync(markdownDoc, cancellationToken).ConfigureAwait(false);

            var transformed = TransformForProgression(mdResult.Content, markdownDoc.HeadingTimestampUtc);

            return (transformed, markdownDoc.HeadingTimestampUtc, null);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return (null, null, ex.Message);
        }
    }

    private static string SanitizeSportName(string sportName)
    {
        var sanitized = new StringBuilder(sportName.Length);
        foreach (var c in sportName)
        {
            if (char.IsLetterOrDigit(c))
                sanitized.Append(c);
            else if (char.IsWhiteSpace(c) || c == '-')
                sanitized.Append('_');
        }

        return sanitized.Length > 0 ? sanitized.ToString() : "Unknown";
    }

    /// <summary>
    /// Transforms a single-activity markdown document for embedding in a progression:
    /// 1. Strips YAML frontmatter
    /// 2. Replaces H1 + loose timestamp line with an H2 containing the UTC timestamp
    /// 3. Downshifts all remaining headings by one level (## → ###, ### → ####, etc.)
    /// </summary>
    internal static string TransformForProgression(string markdown, DateTimeOffset? timestamp)
    {
        // 1. Strip YAML frontmatter
        var content = FrontmatterRegex().Replace(markdown, "").TrimStart('\r', '\n');

        // 2. Replace H1 heading + optional timestamp paragraph with H2 timestamp heading
        var timestampLabel = timestamp is not null
            ? timestamp.Value.ToString("yyyy-MM-dd HH:mm:ss") + " UTC"
            : "Unknown Time";
        content = H1WithTimestampRegex().Replace(content, "");
        content = content.TrimStart('\r', '\n');

        // 3. Downshift all remaining headings by one level
        content = HeadingRegex().Replace(content, m => "#" + m.Value);

        // 4. Prepend the H2 timestamp heading
        var sb = new StringBuilder();
        sb.AppendLine($"## {timestampLabel}");
        sb.AppendLine();
        sb.Append(content);

        return sb.ToString();
    }

    [GeneratedRegex(@"^---\s*\n[\s\S]*?\n---\s*\n?", RegexOptions.Multiline)]
    private static partial Regex FrontmatterRegex();

    [GeneratedRegex(@"^#\s+.+\n+(?:\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2}\s+\w+\s*\n)?", RegexOptions.Multiline)]
    private static partial Regex H1WithTimestampRegex();

    [GeneratedRegex(@"^(#{1,5}\s)", RegexOptions.Multiline)]
    private static partial Regex HeadingRegex();
}
