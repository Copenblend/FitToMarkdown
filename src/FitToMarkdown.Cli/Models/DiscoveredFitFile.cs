namespace FitToMarkdown.Cli.Models;

internal sealed record DiscoveredFitFile(
    string FullPath,
    string RelativeGroupName,
    string FileName,
    long LengthBytes,
    DateTimeOffset LastWriteTimeUtc);
