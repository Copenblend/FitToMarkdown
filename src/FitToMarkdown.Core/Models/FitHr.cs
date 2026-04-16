using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized compressed HrMesg payload from a FIT file.
/// </summary>
public sealed record FitHr
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>Heart rate message timestamp in UTC.</summary>
    public DateTimeOffset? TimestampUtc { get; init; }

    /// <summary>Filtered heart rate in beats per minute.</summary>
    public byte? FilteredBpm { get; init; }

    /// <summary>Expanded beat timestamps in UTC.</summary>
    public IReadOnlyList<DateTimeOffset> BeatTimestampsUtc { get; init; } = [];

    /// <summary>Raw event timestamp values.</summary>
    public IReadOnlyList<uint> EventTimestampRaw { get; init; } = [];

    /// <summary>Raw 12-bit compressed event timestamp values.</summary>
    public IReadOnlyList<ushort> EventTimestamp12Raw { get; init; } = [];

    /// <summary>Indicates whether the heart rate data was merged into record messages.</summary>
    public bool MergedIntoRecords { get; init; }
}
