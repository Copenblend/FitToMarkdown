namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Identifies the owning message or section for grouped developer values.
/// </summary>
public enum FitDeveloperFieldOwnerType : byte
{
    /// <summary>Document-level / file-scoped developer field.</summary>
    Document = 0,

    /// <summary>Attached to a session message.</summary>
    Session = 1,

    /// <summary>Attached to a lap message.</summary>
    Lap = 2,

    /// <summary>Attached to a length message (pool swim).</summary>
    Length = 3,

    /// <summary>Attached to a record (time-series) message.</summary>
    Record = 4,

    /// <summary>Attached to an event message.</summary>
    Event = 5,

    /// <summary>Attached to a device info message.</summary>
    Device = 6,

    /// <summary>Attached to a workout message.</summary>
    Workout = 7,

    /// <summary>Attached to a workout step message.</summary>
    WorkoutStep = 8,

    /// <summary>Attached to a course message.</summary>
    Course = 9,

    /// <summary>Attached to a course point message.</summary>
    CoursePoint = 10,

    /// <summary>Attached to a segment lap message.</summary>
    SegmentLap = 11,

    /// <summary>Attached to a monitoring message.</summary>
    Monitoring = 12,
}
