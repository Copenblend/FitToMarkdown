using FitToMarkdown.Core.Enums;

namespace FitToMarkdown.Core.ValueObjects;

/// <summary>
/// Represents one swimming stroke-count bucket.
/// </summary>
public sealed record FitStrokeCountByType
{
    /// <summary>The type of swimming stroke.</summary>
    public FitStrokeType StrokeType { get; init; }

    /// <summary>The number of strokes of this type.</summary>
    public uint Count { get; init; }
}
