using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized CoursePointMesg from a FIT file.
/// </summary>
public sealed record FitCoursePoint
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>Zero-based point index within the course.</summary>
    public int PointIndex { get; init; }

    /// <summary>Timestamp of the course point in UTC.</summary>
    public DateTimeOffset? TimestampUtc { get; init; }

    /// <summary>GPS position coordinate.</summary>
    public GeoCoordinate? Position { get; init; }

    /// <summary>Accumulated distance in meters.</summary>
    public double? DistanceMeters { get; init; }

    /// <summary>Course point type.</summary>
    public FitCoursePointType? PointType { get; init; }

    /// <summary>Name of the course point.</summary>
    public string? PointName { get; init; }

    /// <summary>Developer field values attached to this message.</summary>
    public IReadOnlyList<FitDeveloperFieldValue> DeveloperFields { get; init; } = [];
}
