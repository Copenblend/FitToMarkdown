using Dynastream.Fit;
using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Parsing;
using FitToMarkdown.Fit.Decoding;
using FitToMarkdown.Fit.Mapping;

namespace FitToMarkdown.Fit.Parsing;

/// <summary>
/// Implements <see cref="IFitMetadataInspector"/> by coordinating a metadata-only decode pass.
/// </summary>
public sealed class FitMetadataInspector : IFitMetadataInspector
{
    private readonly FitDecodeCoordinator _coordinator;
    private readonly FitMetadataSummaryBuilder _builder;

    /// <summary>
    /// Initializes a new instance of the <see cref="FitMetadataInspector"/> class.
    /// </summary>
    /// <param name="coordinator">The decode coordinator for FIT stream processing.</param>
    /// <param name="builder">The metadata summary builder for assembling inspection results.</param>
    internal FitMetadataInspector(FitDecodeCoordinator coordinator, FitMetadataSummaryBuilder builder)
    {
        _coordinator = coordinator;
        _builder = builder;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FitMetadataInspector"/> class with default dependencies.
    /// </summary>
    public FitMetadataInspector()
        : this(new FitDecodeCoordinator(), new FitMetadataSummaryBuilder())
    {
    }

    /// <inheritdoc />
    public async Task<FitMetadataInspectionResult> InspectFileAsync(string inputFilePath, CancellationToken cancellationToken)
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
            return await InspectAsync(stream, inputFilePath, cancellationToken).ConfigureAwait(false);
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
    public Task<FitMetadataInspectionResult> InspectAsync(Stream input, string? sourceName, CancellationToken cancellationToken)
    {
        try
        {
            var metadataOptions = new FitParseOptions { MetadataOnly = true };
            var snapshot = _coordinator.Decode(input, sourceName, metadataOptions, FitDecodeMode.MetadataOnly);
            var result = _builder.Build(snapshot);
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

    private static FitMetadataInspectionResult CreateFatalResult(string code, string message, string? exceptionType = null, string? phase = null)
    {
        return new FitMetadataInspectionResult
        {
            Summary = null,
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
