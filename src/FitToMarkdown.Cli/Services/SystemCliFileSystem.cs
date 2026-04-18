namespace FitToMarkdown.Cli.Services;

internal sealed class SystemCliFileSystem : ICliFileSystem
{
    public string GetCurrentDirectory()
    {
        return Directory.GetCurrentDirectory();
    }

    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    public IEnumerable<string> EnumerateDirectories(string path)
    {
        return Directory.EnumerateDirectories(path);
    }

    public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
    {
        return Directory.EnumerateFiles(path, searchPattern, searchOption);
    }

    public DateTimeOffset GetLastWriteTimeUtc(string path)
    {
        return new DateTimeOffset(new FileInfo(path).LastWriteTimeUtc, TimeSpan.Zero);
    }

    public long GetFileLength(string path)
    {
        return new FileInfo(path).Length;
    }

    public void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }

    public Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken)
    {
        return File.WriteAllTextAsync(path, content, cancellationToken);
    }

    public Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken)
    {
        return File.ReadAllTextAsync(path, cancellationToken);
    }
}
