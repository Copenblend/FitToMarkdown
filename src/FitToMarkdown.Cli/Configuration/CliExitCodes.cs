namespace FitToMarkdown.Cli.Configuration;

/// <summary>
/// Defines well-known process exit codes for the CLI application.
/// </summary>
public static class CliExitCodes
{
    /// <summary>All files processed successfully.</summary>
    public const int Success = 0;

    /// <summary>Some files processed, but one or more failed or were skipped.</summary>
    public const int PartialSuccess = 1;

    /// <summary>No files could be processed.</summary>
    public const int TotalFailure = 2;

    /// <summary>The command-line arguments or options were invalid.</summary>
    public const int InvalidInput = 3;

    /// <summary>An unexpected runtime error occurred.</summary>
    public const int UnexpectedError = 4;
}
