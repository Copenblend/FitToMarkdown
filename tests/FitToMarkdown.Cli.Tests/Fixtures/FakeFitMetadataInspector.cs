using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Parsing;

namespace FitToMarkdown.Cli.Tests.Fixtures;

internal sealed class FakeFitMetadataInspector : IFitMetadataInspector
{
    public Dictionary<string, FitMetadataInspectionResult> Results { get; } = new(StringComparer.OrdinalIgnoreCase);

    public Task<FitMetadataInspectionResult> InspectFileAsync(string inputFilePath, CancellationToken cancellationToken)
    {
        var result = Results.TryGetValue(inputFilePath, out var r)
            ? r
            : new FitMetadataInspectionResult();
        return Task.FromResult(result);
    }

    public Task<FitMetadataInspectionResult> InspectAsync(Stream input, string? sourceName, CancellationToken cancellationToken)
    {
        var key = sourceName ?? string.Empty;
        var result = Results.TryGetValue(key, out var r)
            ? r
            : new FitMetadataInspectionResult();
        return Task.FromResult(result);
    }
}
