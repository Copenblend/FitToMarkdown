using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Summaries;

/// <summary>
/// Represents one markdown-ready course-point row.
/// </summary>
public sealed record CoursePointRow
{
    /// <summary>Display order of this course point.</summary>
    public int Order { get; init; }

    /// <summary>Name of the course point.</summary>
    public string? PointName { get; init; }

    /// <summary>Type of the course point.</summary>
    public FitCoursePointType? PointType { get; init; }

    /// <summary>Distance from the start in meters.</summary>
    public double? DistanceMeters { get; init; }

    /// <summary>GPS position of the course point.</summary>
    public GeoCoordinate? Position { get; init; }
}
