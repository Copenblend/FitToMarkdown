using System.Diagnostics;
using FitToMarkdown.Cli.Models;
using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Enums;

namespace FitToMarkdown.Cli.Services;

internal sealed class ConversionBatchRunner
{
    private readonly IFitFileParser _parser;
    private readonly IFitMarkdownProjector _projector;
    private readonly IMarkdownDocumentGenerator _generator;
    private readonly ICliFileSystem _fileSystem;
    private readonly FitParseOptionsFactory _parseOptionsFactory;
    private readonly MarkdownOptionsFactory _optionsFactory;
    private readonly OutputPathResolver _outputPathResolver;
    private readonly ConvertPromptService _promptService;

    internal ConversionBatchRunner(
        IFitFileParser parser,
        IFitMarkdownProjector projector,
        IMarkdownDocumentGenerator generator,
        ICliFileSystem fileSystem,
        FitParseOptionsFactory parseOptionsFactory,
        MarkdownOptionsFactory optionsFactory,
        OutputPathResolver outputPathResolver,
        ConvertPromptService promptService)
    {
        _parser = parser;
        _projector = projector;
        _generator = generator;
        _fileSystem = fileSystem;
        _parseOptionsFactory = parseOptionsFactory;
        _optionsFactory = optionsFactory;
        _outputPathResolver = outputPathResolver;
        _promptService = promptService;
    }

    internal async Task<ConvertBatchSummary> RunAsync(
        ConvertExecutionPlan plan,
        int degreeOfParallelism,
        CancellationToken cancellationToken,
        Action<int, string>? onFileStarted = null)
    {
        var stopwatch = Stopwatch.StartNew();

        // Stage 1 — Prepare (bounded-parallel)
        var preparedOrFailed = new (PreparedConversionArtifact? Artifact, ConvertFileResult? Failure)[plan.Files.Count];

        await Parallel.ForEachAsync(
            Enumerable.Range(0, plan.Files.Count),
            new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism, CancellationToken = cancellationToken },
            async (index, ct) =>
            {
                var file = plan.Files[index];
                var fileStopwatch = Stopwatch.StartNew();
                try
                {
                    var parseOptions = _parseOptionsFactory.CreateForConvert();
                    var parseResult = await _parser.ParseFileAsync(file.FullPath, parseOptions, ct).ConfigureAwait(false);

                    if (parseResult.Document is null)
                    {
                        fileStopwatch.Stop();
                        preparedOrFailed[index] = (null, new ConvertFileResult
                        {
                            SourcePath = file.FullPath,
                            Status = ConvertFileResultStatus.Failed,
                            FailureReason = parseResult.FatalError?.Message ?? "No document returned.",
                            ElapsedTime = fileStopwatch.Elapsed,
                        });
                        return;
                    }

                    var options = _optionsFactory.CreateFor(parseResult);
                    var markdownDoc = await _projector.ProjectAsync(parseResult.Document, options, ct).ConfigureAwait(false);
                    var mdResult = await _generator.GenerateAsync(markdownDoc, ct).ConfigureAwait(false);

                    var warnings = new List<string>();
                    foreach (var issue in parseResult.Issues)
                    {
                        if (issue.Severity == FitParseIssueSeverity.Warning)
                            warnings.Add(issue.Message);
                    }

                    fileStopwatch.Stop();
                    preparedOrFailed[index] = (new PreparedConversionArtifact
                    {
                        SourceOrder = index,
                        InputFilePath = file.FullPath,
                        MarkdownResult = mdResult,
                        WarningLines = warnings,
                        PreparationElapsed = fileStopwatch.Elapsed,
                    }, null);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    fileStopwatch.Stop();
                    preparedOrFailed[index] = (null, new ConvertFileResult
                    {
                        SourcePath = file.FullPath,
                        Status = ConvertFileResultStatus.Failed,
                        FailureReason = ex.Message,
                        ElapsedTime = fileStopwatch.Elapsed,
                    });
                }
            }).ConfigureAwait(false);

        // Stage 2 — Commit (serialized, in original discovery order)
        var results = new List<ConvertFileResult>(plan.Files.Count);
        var claimedDestinations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var fileIndex = 0;

        for (int i = 0; i < plan.Files.Count; i++)
        {
            fileIndex++;
            var file = plan.Files[i];
            onFileStarted?.Invoke(fileIndex, file.FileName);

            var (artifact, earlyFailure) = preparedOrFailed[i];

            if (earlyFailure is not null)
            {
                results.Add(earlyFailure);
                continue;
            }

            if (artifact is null)
            {
                results.Add(new ConvertFileResult
                {
                    SourcePath = file.FullPath,
                    Status = ConvertFileResultStatus.Failed,
                    FailureReason = "Internal error: no artifact produced.",
                    ElapsedTime = TimeSpan.Zero,
                });
                continue;
            }

            var commitStopwatch = Stopwatch.StartNew();
            try
            {
                var outputPath = _outputPathResolver.ResolveOutputFilePath(
                    plan.OutputDirectory, file.FullPath, artifact.MarkdownResult);

                // Duplicate destination detection
                var canonicalOutput = Path.GetFullPath(outputPath).ToUpperInvariant();
                if (!claimedDestinations.Add(canonicalOutput))
                {
                    commitStopwatch.Stop();
                    results.Add(new ConvertFileResult
                    {
                        SourcePath = file.FullPath,
                        OutputPath = outputPath,
                        Status = ConvertFileResultStatus.Failed,
                        FailureReason = $"Duplicate output destination: '{Path.GetFileName(outputPath)}' already claimed by an earlier file.",
                        ElapsedTime = artifact.PreparationElapsed + commitStopwatch.Elapsed,
                    });
                    continue;
                }

                // Overwrite handling
                if (_fileSystem.FileExists(outputPath))
                {
                    switch (plan.OverwriteMode)
                    {
                        case ConvertOverwriteMode.Skip:
                            commitStopwatch.Stop();
                            results.Add(new ConvertFileResult
                            {
                                SourcePath = file.FullPath,
                                OutputPath = outputPath,
                                Status = ConvertFileResultStatus.Skipped,
                                ElapsedTime = artifact.PreparationElapsed + commitStopwatch.Elapsed,
                            });
                            continue;

                        case ConvertOverwriteMode.AskEach when !plan.NoInteraction:
                            var confirmed = await _promptService.ConfirmOverwriteAsync(outputPath, cancellationToken).ConfigureAwait(false);
                            if (!confirmed)
                            {
                                commitStopwatch.Stop();
                                results.Add(new ConvertFileResult
                                {
                                    SourcePath = file.FullPath,
                                    OutputPath = outputPath,
                                    Status = ConvertFileResultStatus.Skipped,
                                    ElapsedTime = artifact.PreparationElapsed + commitStopwatch.Elapsed,
                                });
                                continue;
                            }
                            break;

                        case ConvertOverwriteMode.Overwrite:
                        case ConvertOverwriteMode.AskEach:
                            break;
                    }
                }

                await _fileSystem.WriteAllTextAsync(outputPath, artifact.MarkdownResult.Content, cancellationToken).ConfigureAwait(false);

                commitStopwatch.Stop();
                results.Add(new ConvertFileResult
                {
                    SourcePath = file.FullPath,
                    OutputPath = outputPath,
                    Status = ConvertFileResultStatus.Converted,
                    ElapsedTime = artifact.PreparationElapsed + commitStopwatch.Elapsed,
                });
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                commitStopwatch.Stop();
                results.Add(new ConvertFileResult
                {
                    SourcePath = file.FullPath,
                    Status = ConvertFileResultStatus.Failed,
                    FailureReason = ex.Message,
                    ElapsedTime = artifact.PreparationElapsed + commitStopwatch.Elapsed,
                });
            }
        }

        stopwatch.Stop();

        return new ConvertBatchSummary
        {
            ProcessedCount = results.Count,
            ConvertedCount = results.Count(r => r.Status == ConvertFileResultStatus.Converted),
            FailedCount = results.Count(r => r.Status == ConvertFileResultStatus.Failed),
            SkippedCount = results.Count(r => r.Status == ConvertFileResultStatus.Skipped),
            TotalElapsed = stopwatch.Elapsed,
            OutputDirectory = plan.OutputDirectory,
            Results = results,
        };
    }
}
