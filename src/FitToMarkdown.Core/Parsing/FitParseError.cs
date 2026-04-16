namespace FitToMarkdown.Core.Parsing;

/// <summary>
/// Represents a fatal parse failure when no trustworthy document can be returned.
/// </summary>
public sealed record FitParseError
{
    /// <summary>A machine-readable code identifying the fatal error category.</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>A human-readable description of the fatal error.</summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>The fully qualified type name of the exception that caused the failure, if applicable.</summary>
    public string? ExceptionType { get; init; }

    /// <summary>The parsing phase during which the fatal error occurred.</summary>
    public string? Phase { get; init; }

    /// <summary>The byte offset in the source file where the error was detected, if available.</summary>
    public long? ByteOffset { get; init; }

    /// <summary>Whether the error condition could potentially be recovered from with different options.</summary>
    public bool Recoverable { get; init; }
}
