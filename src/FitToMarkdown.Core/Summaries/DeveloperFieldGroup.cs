using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;

namespace FitToMarkdown.Core.Summaries;

/// <summary>
/// Represents one grouped developer-field section item.
/// </summary>
public sealed record DeveloperFieldGroup
{
    /// <summary>Owner type of the developer field group.</summary>
    public FitDeveloperFieldOwnerType OwnerType { get; init; }

    /// <summary>Display order of this group.</summary>
    public int Order { get; init; }

    /// <summary>Zero-based session index this group belongs to.</summary>
    public int? SessionIndex { get; init; }

    /// <summary>Zero-based lap index this group belongs to.</summary>
    public int? LapIndex { get; init; }

    /// <summary>Zero-based length index this group belongs to.</summary>
    public int? LengthIndex { get; init; }

    /// <summary>Zero-based record index this group belongs to.</summary>
    public int? RecordIndex { get; init; }

    /// <summary>Zero-based event index this group belongs to.</summary>
    public int? EventIndex { get; init; }

    /// <summary>UTC timestamp associated with this group.</summary>
    public DateTimeOffset? TimestampUtc { get; init; }

    /// <summary>Developer field values in this group.</summary>
    public IReadOnlyList<FitDeveloperFieldValue> Values { get; init; } = [];
}
