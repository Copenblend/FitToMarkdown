using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized ClimbProMesg from a FIT file.
/// </summary>
public sealed record FitClimbPro
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>Timestamp of the ClimbPro message in UTC.</summary>
    public DateTimeOffset? TimestampUtc { get; init; }

    /// <summary>GPS position coordinate.</summary>
    public GeoCoordinate? Position { get; init; }

    /// <summary>ClimbPro event type.</summary>
    public FitClimbProEvent? ClimbProEvent { get; init; }

    /// <summary>Climb number identifier.</summary>
    public ushort? ClimbNumber { get; init; }

    /// <summary>Climb category classification.</summary>
    public FitClimbCategory? ClimbCategory { get; init; }

    /// <summary>Current distance into the climb in meters.</summary>
    public double? CurrentDistanceMeters { get; init; }

    /// <summary>Developer field values attached to this message.</summary>
    public IReadOnlyList<FitDeveloperFieldValue> DeveloperFields { get; init; } = [];
}
