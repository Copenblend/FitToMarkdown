using System.Diagnostics;
using FitToMarkdown.Cli.Models;
using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Parsing;

namespace FitToMarkdown.Cli.Services;

internal sealed class ConversionBatchRunner
{
    private readonly IFitFileParser _parser;
    private readonly IFitMarkdownProjector _projector;
    private readonly IMarkdownDocumentGenerator _generator;
    private readonly ICliFileSystem _fileSystem;
    private readonly MarkdownOptionsFactory _optionsFactory;
    private readonly OutputPathResolver _outputPathResolver;
    private readonly ConvertPromptService _promptService;

    internal ConversionBatchRunner(
        IFitFileParser parser,
        IFitMarkdownProjector projector,
        IMarkdownDocumentGenerator generator,
        ICliFileSystem fileSystem,
        MarkdownOptionsFactory optionsFactory,
        OutputPathResolver outputPathResolver,
        ConvertPromptService promptService)
    {
        _parser = parser;
        _projector = projector;
        _generator = generator;
        _fileSystem = fileSystem;
        _optionsFactory = optionsFactory;
        _outputPathResolver = outputPathResolver;
        _promptService = promptService;
    }

    internal async Task<ConvertBatchSummary> RunAsync(ConvertExecutionPlan plan, CancellationToken cancellationToken, Action<int, string>? onFileStarted = null)
    {
        var results = new List<ConvertFileResult>();
        var stopwatch = Stopwatch.StartNew();
        var fileIndex = 0;

        foreach (var file in plan.Files)
        {
            fileIndex++;
            onFileStarted?.Invoke(fileIndex, file.FileName);
            var fileStopwatch = Stopwatch.StartNew();
            ConvertFileResult result;

            try
            {
                var parseOptions = new FitParseOptions
                {
                    AllowPartialExtraction = true,
                    RecoverTruncatedActivityFiles = true,
                    ResolveDeveloperFields = true,
                };

                var parseResult = await _parser.ParseFileAsync(file.FullPath, parseOptions, cancellationToken).ConfigureAwait(false);

                if (parseResult.Document is null)
                {
                    fileStopwatch.Stop();
                    results.Add(new ConvertFileResult
                    {
                        SourcePath = file.FullPath,
                        Status = ConvertFileResultStatus.Failed,
                        FailureReason = parseResult.FatalError?.Message ?? "No document returned.",
                        ElapsedTime = fileStopwatch.Elapsed,
                    });
                    continue;
                }

                var options = _optionsFactory.CreateFor(parseResult);
                var markdownDoc = await _projector.ProjectAsync(parseResult.Document, options, cancellationToken).ConfigureAwait(false);
                var mdResult = await _generator.GenerateAsync(markdownDoc, cancellationToken).ConfigureAwait(false);
                var outputPath = _outputPathResolver.ResolveOutputFilePath(plan.OutputDirectory, file.FullPath, mdResult);

                if (_fileSystem.FileExists(outputPath))
                {
                    switch (plan.OverwriteMode)
                    {
                        case ConvertOverwriteMode.Skip:
                            fileStopwatch.Stop();
                            results.Add(new ConvertFileResult
                            {
                                SourcePath = file.FullPath,
                                OutputPath = outputPath,
                                Status = ConvertFileResultStatus.Skipped,
                                ElapsedTime = fileStopwatch.Elapsed,
                            });
                            continue;

                        case ConvertOverwriteMode.AskEach when !plan.NoInteraction:
                            var confirmed = await _promptService.ConfirmOverwriteAsync(outputPath, cancellationToken).ConfigureAwait(false);
                            if (!confirmed)
                            {
                                fileStopwatch.Stop();
                                results.Add(new ConvertFileResult
                                {
                                    SourcePath = file.FullPath,
                                    OutputPath = outputPath,
                                    Status = ConvertFileResultStatus.Skipped,
                                    ElapsedTime = fileStopwatch.Elapsed,
                                });
                                continue;
                            }
                            break;

                        case ConvertOverwriteMode.Overwrite:
                        case ConvertOverwriteMode.AskEach:
                            break;
                    }
                }

                await _fileSystem.WriteAllTextAsync(outputPath, mdResult.Content, cancellationToken).ConfigureAwait(false);

                fileStopwatch.Stop();
                result = new ConvertFileResult
                {
                    SourcePath = file.FullPath,
                    OutputPath = outputPath,
                    Status = ConvertFileResultStatus.Converted,
                    ElapsedTime = fileStopwatch.Elapsed,
                };
            }
            catch (Exception ex)
            {
                fileStopwatch.Stop();
                result = new ConvertFileResult
                {
                    SourcePath = file.FullPath,
                    Status = ConvertFileResultStatus.Failed,
                    FailureReason = ex.Message,
                    ElapsedTime = fileStopwatch.Elapsed,
                };
            }

            results.Add(result);
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
