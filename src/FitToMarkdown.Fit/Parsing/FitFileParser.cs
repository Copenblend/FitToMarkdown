using Dynastream.Fit;
using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Parsing;
using FitToMarkdown.Fit.Decoding;
using FitToMarkdown.Fit.Mapping;

namespace FitToMarkdown.Fit.Parsing;

/// <summary>
/// Implements <see cref="IFitFileParser"/> by coordinating decode, validation, and document building.
/// </summary>
public sealed class FitFileParser : IFitFileParser
{
    private readonly FitDecodeCoordinator _coordinator;
    private readonly FitDocumentBuilder _builder;

    /// <summary>
    /// Initializes a new instance of the <see cref="FitFileParser"/> class.
    /// </summary>
    /// <param name="coordinator">The decode coordinator for FIT stream processing.</param>
    /// <param name="builder">The document builder for assembling parse results.</param>
    internal FitFileParser(FitDecodeCoordinator coordinator, FitDocumentBuilder builder)
    {
        _coordinator = coordinator;
        _builder = builder;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FitFileParser"/> class with default dependencies.
    /// </summary>
    public FitFileParser()
        : this(new FitDecodeCoordinator(), new FitDocumentBuilder())
    {
    }

    /// <inheritdoc />
    public async Task<FitParseResult> ParseFileAsync(string inputFilePath, FitParseOptions options, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(inputFilePath))
            {
                return CreateFatalResult("FIT_INVALID_PATH", "Input file path is null or empty.", phase: "Validation");
            }

            if (!System.IO.File.Exists(inputFilePath))
            {
                return CreateFatalResult("FIT_FILE_NOT_FOUND", $"File not found: {inputFilePath}", phase: "Validation");
            }

            await using var stream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
            return await ParseAsync(stream, inputFilePath, options, cancellationToken).ConfigureAwait(false);
        }
        catch (FitException ex)
        {
            return CreateFatalResult("FIT_DECODE_FATAL", ex.Message, exceptionType: ex.GetType().FullName, phase: "Decode");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return CreateFatalResult("FIT_UNEXPECTED_ERROR", ex.Message, exceptionType: ex.GetType().FullName, phase: "Unknown");
        }
    }

    /// <inheritdoc />
    public Task<FitParseResult> ParseAsync(Stream input, string? sourceName, FitParseOptions options, CancellationToken cancellationToken)
    {
        try
        {
            var snapshot = _coordinator.Decode(input, sourceName, options, FitDecodeMode.FullParse);
            var result = _builder.Build(snapshot, options);
            return Task.FromResult(result);
        }
        catch (FitException ex)
        {
            return Task.FromResult(CreateFatalResult("FIT_DECODE_FATAL", ex.Message, exceptionType: ex.GetType().FullName, phase: "Decode"));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Task.FromResult(CreateFatalResult("FIT_UNEXPECTED_ERROR", ex.Message, exceptionType: ex.GetType().FullName, phase: "Unknown"));
        }
    }

    private static FitParseResult CreateFatalResult(string code, string message, string? exceptionType = null, string? phase = null)
    {
        return new FitParseResult
        {
            Document = null,
            Metadata = new FitParseMetadata
            {
                Status = FitParseStatus.Failed,
                HadDecodeFault = true,
            },
            FatalError = new FitParseError
            {
                Code = code,
                Message = message,
                ExceptionType = exceptionType,
                Phase = phase,
                Recoverable = false,
            },
        };
    }
}
