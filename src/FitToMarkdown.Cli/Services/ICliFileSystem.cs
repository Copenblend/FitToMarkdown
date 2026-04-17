namespace FitToMarkdown.Cli.Services;

internal interface ICliFileSystem
{
    string GetCurrentDirectory();
    bool FileExists(string path);
    bool DirectoryExists(string path);
    IEnumerable<string> EnumerateDirectories(string path);
    IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption);
    DateTimeOffset GetLastWriteTimeUtc(string path);
    long GetFileLength(string path);
    void CreateDirectory(string path);
    Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken);
}
