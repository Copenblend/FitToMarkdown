using FitToMarkdown.Cli.Services;

namespace FitToMarkdown.Cli.Tests.Fixtures;

internal sealed class FakeCliFileSystem : ICliFileSystem
{
    public Dictionary<string, byte[]> Files { get; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> Directories { get; } = new(StringComparer.OrdinalIgnoreCase);
    public List<(string Path, string Content)> WrittenFiles { get; } = [];
    public string CurrentDirectory { get; set; } = @"C:\test";

    public string GetCurrentDirectory() => CurrentDirectory;

    public bool FileExists(string path) => Files.ContainsKey(path);

    public bool DirectoryExists(string path) => Directories.Contains(path);

    public IEnumerable<string> EnumerateDirectories(string path)
    {
        var pathWithSep = path.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        return Directories
            .Where(d => d.StartsWith(pathWithSep, StringComparison.OrdinalIgnoreCase) &&
                        d.IndexOf(Path.DirectorySeparatorChar, pathWithSep.Length) < 0);
    }

    public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
    {
        var extension = searchPattern.StartsWith("*", StringComparison.Ordinal)
            ? searchPattern[1..]
            : string.Empty;

        var pathWithSep = path.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

        return Files.Keys.Where(f =>
        {
            var dir = Path.GetDirectoryName(f) ?? string.Empty;

            if (searchOption == SearchOption.TopDirectoryOnly)
            {
                if (!dir.Equals(path, StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            else
            {
                if (!dir.Equals(path, StringComparison.OrdinalIgnoreCase) &&
                    !dir.StartsWith(pathWithSep, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            if (!string.IsNullOrEmpty(extension))
                return f.EndsWith(extension, StringComparison.OrdinalIgnoreCase);

            return true;
        });
    }

    public DateTimeOffset GetLastWriteTimeUtc(string path) =>
        new(2025, 6, 15, 10, 0, 0, TimeSpan.Zero);

    public long GetFileLength(string path) =>
        Files.TryGetValue(path, out var bytes) ? bytes.Length : 1024;

    public void CreateDirectory(string path) => Directories.Add(path);

    public Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken)
    {
        WrittenFiles.Add((path, content));
        return Task.CompletedTask;
    }

    public void AddFitFile(string path)
    {
        Files[path] = [0x0E, 0x20, 0x00, 0x00];
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
            Directories.Add(directory);
    }

    public void AddDirectory(string path) => Directories.Add(path);
}
