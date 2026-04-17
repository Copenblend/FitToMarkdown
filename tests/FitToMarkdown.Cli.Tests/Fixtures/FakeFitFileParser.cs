using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Parsing;

namespace FitToMarkdown.Cli.Tests.Fixtures;

internal sealed class FakeFitFileParser : IFitFileParser
{
    public Dictionary<string, FitParseResult> Results { get; } = new(StringComparer.OrdinalIgnoreCase);

    public Task<FitParseResult> ParseFileAsync(string inputFilePath, FitParseOptions options, CancellationToken cancellationToken)
    {
        var result = Results.TryGetValue(inputFilePath, out var r)
            ? r
            : new FitParseResult { FatalError = new FitParseError { Code = "NOT_FOUND", Message = "No test result configured." } };
        return Task.FromResult(result);
    }

    public Task<FitParseResult> ParseAsync(Stream input, string? sourceName, FitParseOptions options, CancellationToken cancellationToken)
    {
        var key = sourceName ?? string.Empty;
        var result = Results.TryGetValue(key, out var r)
            ? r
            : new FitParseResult { FatalError = new FitParseError { Code = "NOT_FOUND", Message = "No test result configured." } };
        return Task.FromResult(result);
    }
}
