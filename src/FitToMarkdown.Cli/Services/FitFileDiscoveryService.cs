using FitToMarkdown.Cli.Models;

namespace FitToMarkdown.Cli.Services;

internal sealed class FitFileDiscoveryService
{
    private readonly ICliFileSystem _fileSystem;

    internal FitFileDiscoveryService(ICliFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    internal Task<IReadOnlyList<DiscoveredFitFile>> DiscoverAsync(string rootDirectory, CancellationToken cancellationToken)
    {
        var files = SafeEnumerateFiles(rootDirectory)
            .Where(f => string.Equals(Path.GetExtension(f), ".fit", StringComparison.OrdinalIgnoreCase))
            .Select(f =>
            {
                var directory = Path.GetDirectoryName(f)!;
                var relativePath = Path.GetRelativePath(rootDirectory, directory);
                var groupName = string.Equals(relativePath, ".", StringComparison.Ordinal)
                    ? "(root)"
                    : relativePath;

                return new DiscoveredFitFile(
                    FullPath: f,
                    RelativeGroupName: groupName,
                    FileName: Path.GetFileName(f),
                    LengthBytes: _fileSystem.GetFileLength(f),
                    LastWriteTimeUtc: _fileSystem.GetLastWriteTimeUtc(f));
            })
            .OrderBy(d => d.RelativeGroupName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(d => d.FileName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return Task.FromResult<IReadOnlyList<DiscoveredFitFile>>(files);
    }

    private IReadOnlyList<string> SafeEnumerateFiles(string rootDirectory)
    {
        var results = new List<string>();
        try
        {
            foreach (var file in _fileSystem.EnumerateFiles(rootDirectory, "*.fit", SearchOption.AllDirectories))
            {
                results.Add(file);
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Partial results from before the inaccessible entry
        }
        catch (IOException)
        {
            // Partial results from before the IO error
        }

        return results;
    }
}
