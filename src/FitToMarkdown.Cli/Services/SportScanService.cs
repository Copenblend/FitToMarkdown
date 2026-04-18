using FitToMarkdown.Cli.Models;
using FitToMarkdown.Core.Abstractions;

namespace FitToMarkdown.Cli.Services;

/// <summary>
/// Scans discovered FIT files for sport metadata and groups them by sport and sub-sport.
/// </summary>
internal sealed class SportScanService
{
    private readonly IFitMetadataInspector _inspector;

    internal SportScanService(IFitMetadataInspector inspector)
    {
        _inspector = inspector;
    }

    /// <summary>
    /// Inspects each discovered file to extract sport metadata and groups them by sport/sub-sport.
    /// </summary>
    internal async Task<IReadOnlyList<SportGroup>> ScanAndGroupBySportAsync(
        IReadOnlyList<DiscoveredFitFile> files,
        CancellationToken cancellationToken,
        Action<int>? onFileScanned = null)
    {
        var sportMap = new Dictionary<string, (string Sport, string? SubSport, List<DiscoveredFitFile> Files)>(StringComparer.OrdinalIgnoreCase);
        var scannedCount = 0;

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var result = await _inspector.InspectFileAsync(file.FullPath, cancellationToken).ConfigureAwait(false);
                var sport = result.Summary?.Sport;

                if (!string.IsNullOrWhiteSpace(sport))
                {
                    var subSport = NormalizeSubSport(result.Summary?.SubSport);
                    var key = subSport is not null ? $"{sport}|{subSport}" : sport;

                    if (!sportMap.TryGetValue(key, out var entry))
                    {
                        entry = (sport, subSport, []);
                        sportMap[key] = entry;
                    }

                    entry.Files.Add(file);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // Skip files that can't be inspected
            }

            scannedCount++;
            onFileScanned?.Invoke(scannedCount);
        }

        return sportMap
            .Select(kvp => new SportGroup
            {
                SportName = kvp.Value.Sport,
                SubSportName = kvp.Value.SubSport,
                Files = kvp.Value.Files
                    .OrderBy(f => f.LastWriteTimeUtc)
                    .ToList(),
            })
            .OrderBy(g => g.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string? NormalizeSubSport(string? subSport)
    {
        if (string.IsNullOrWhiteSpace(subSport))
            return null;

        // "Generic" sub-sport is meaningless — treat as no sub-sport
        if (string.Equals(subSport, "Generic", StringComparison.OrdinalIgnoreCase))
            return null;

        return subSport;
    }
}
