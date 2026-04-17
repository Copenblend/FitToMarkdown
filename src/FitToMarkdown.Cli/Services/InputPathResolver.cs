using FitToMarkdown.Cli.Models;

namespace FitToMarkdown.Cli.Services;

internal sealed class InputPathResolver
{
    private readonly ICliFileSystem _fileSystem;
    private readonly InteractivePathBrowser _browser;

    internal InputPathResolver(ICliFileSystem fileSystem, InteractivePathBrowser browser)
    {
        _fileSystem = fileSystem;
        _browser = browser;
    }

    internal async Task<InputTarget?> ResolveAsync(string? requestedPath, bool noInteraction, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(requestedPath))
        {
            var fullPath = Path.GetFullPath(requestedPath);

            if (_fileSystem.FileExists(fullPath))
            {
                return string.Equals(Path.GetExtension(fullPath), ".fit", StringComparison.OrdinalIgnoreCase)
                    ? new InputTarget(fullPath, InputTargetKind.File)
                    : null;
            }

            if (_fileSystem.DirectoryExists(fullPath))
            {
                return new InputTarget(fullPath, InputTargetKind.Directory);
            }

            return null;
        }

        if (noInteraction)
        {
            return null;
        }

        return await _browser.BrowseAsync(_fileSystem.GetCurrentDirectory(), cancellationToken).ConfigureAwait(false);
    }
}
