namespace FitToMarkdown.Cli.Tests.Fixtures;

/// <summary>
/// Creates and manages a temporary workspace directory for integration and end-to-end tests.
/// Ensures tests never write into repository-controlled sample folders.
/// </summary>
internal sealed class TemporaryWorkspace : IDisposable
{
    private readonly string _root;
    private bool _disposed;

    internal TemporaryWorkspace()
    {
        _root = Path.Combine(Path.GetTempPath(), "FitToMarkdown_Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);
    }

    internal string Root => _root;

    internal string CreateSubdirectory(string name)
    {
        var path = Path.Combine(_root, name);
        Directory.CreateDirectory(path);
        return path;
    }

    internal string WriteFile(string relativePath, byte[] content)
    {
        var path = Path.Combine(_root, relativePath);
        var dir = Path.GetDirectoryName(path)!;
        Directory.CreateDirectory(dir);
        File.WriteAllBytes(path, content);
        return path;
    }

    internal string WriteFile(string relativePath, string content)
    {
        var path = Path.Combine(_root, relativePath);
        var dir = Path.GetDirectoryName(path)!;
        Directory.CreateDirectory(dir);
        File.WriteAllText(path, content);
        return path;
    }

    internal string CopyFile(string sourcePath, string relativeDestination)
    {
        var destPath = Path.Combine(_root, relativeDestination);
        var dir = Path.GetDirectoryName(destPath)!;
        Directory.CreateDirectory(dir);
        File.Copy(sourcePath, destPath, overwrite: true);
        return destPath;
    }

    internal IReadOnlyList<string> GetAllFiles(string? subdirectory = null)
    {
        var searchRoot = subdirectory is null ? _root : Path.Combine(_root, subdirectory);
        if (!Directory.Exists(searchRoot)) return [];
        return Directory.GetFiles(searchRoot, "*", SearchOption.AllDirectories);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        try
        {
            if (Directory.Exists(_root))
            {
                Directory.Delete(_root, recursive: true);
            }
        }
        catch (IOException)
        {
            // Best-effort cleanup; don't fail tests on temp directory deletion
        }
    }
}
