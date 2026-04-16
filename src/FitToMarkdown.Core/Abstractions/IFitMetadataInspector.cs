using FitToMarkdown.Core.Parsing;

namespace FitToMarkdown.Core.Abstractions;

/// <summary>
/// Defines the metadata-only inspection boundary implemented by <c>FitToMarkdown.Fit</c>.
/// </summary>
public interface IFitMetadataInspector
{
    /// <summary>
    /// Inspects a FIT file from a file path for metadata only.
    /// </summary>
    /// <param name="inputFilePath">The absolute path to the FIT file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The metadata inspection result envelope.</returns>
    Task<FitMetadataInspectionResult> InspectFileAsync(string inputFilePath, CancellationToken cancellationToken);

    /// <summary>
    /// Inspects a FIT file from a stream for metadata only.
    /// </summary>
    /// <param name="input">The input stream containing FIT data.</param>
    /// <param name="sourceName">Optional source name for diagnostics.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The metadata inspection result envelope.</returns>
    Task<FitMetadataInspectionResult> InspectAsync(Stream input, string? sourceName, CancellationToken cancellationToken);
}
