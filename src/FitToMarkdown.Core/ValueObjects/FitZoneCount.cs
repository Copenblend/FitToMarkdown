using FitToMarkdown.Core.Enums;

namespace FitToMarkdown.Core.ValueObjects;

/// <summary>
/// Represents one zone-count bucket.
/// </summary>
public sealed record FitZoneCount
{
    /// <summary>The zone kind this count belongs to.</summary>
    public FitZoneKind Zone { get; init; }

    /// <summary>The number of occurrences in this zone.</summary>
    public uint Count { get; init; }
}
