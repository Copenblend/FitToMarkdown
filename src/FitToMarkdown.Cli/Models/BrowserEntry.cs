namespace FitToMarkdown.Cli.Models;

internal sealed record BrowserEntry(string FullPath, string DisplayText, BrowserEntryKind Kind, string GroupName);
