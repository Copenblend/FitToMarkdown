using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized CourseMesg from a FIT file.
/// </summary>
public sealed record FitCourse
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>Primary sport type.</summary>
    public FitSport? Sport { get; init; }

    /// <summary>Name of the course.</summary>
    public string? CourseName { get; init; }

    /// <summary>Capabilities bitmask for this course.</summary>
    public uint? CapabilitiesMask { get; init; }

    /// <summary>Ordered list of course points.</summary>
    public IReadOnlyList<FitCoursePoint> Points { get; init; } = [];

    /// <summary>Developer field values attached to this message.</summary>
    public IReadOnlyList<FitDeveloperFieldValue> DeveloperFields { get; init; } = [];
}
