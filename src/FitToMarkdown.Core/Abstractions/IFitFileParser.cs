using FitToMarkdown.Core.Parsing;

namespace FitToMarkdown.Core.Abstractions;

/// <summary>
/// Defines the full-file parsing boundary implemented by <c>FitToMarkdown.Fit</c>.
/// </summary>
public interface IFitFileParser
{
    /// <summary>
    /// Parses a FIT file from a file path.
    /// </summary>
    /// <param name="inputFilePath">The absolute path to the FIT file.</param>
    /// <param name="options">Parser behavior flags.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parse result envelope.</returns>
    Task<FitParseResult> ParseFileAsync(string inputFilePath, FitParseOptions options, CancellationToken cancellationToken);

    /// <summary>
    /// Parses a FIT file from a stream.
    /// </summary>
    /// <param name="input">The input stream containing FIT data.</param>
    /// <param name="sourceName">Optional source name for diagnostics.</param>
    /// <param name="options">Parser behavior flags.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parse result envelope.</returns>
    Task<FitParseResult> ParseAsync(Stream input, string? sourceName, FitParseOptions options, CancellationToken cancellationToken);
}
