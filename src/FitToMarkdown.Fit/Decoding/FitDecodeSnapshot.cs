using Dynastream.Fit;

namespace FitToMarkdown.Fit.Decoding;

/// <summary>
/// Immutable snapshot of all SDK messages accumulated during a single FIT decode pass.
/// </summary>
internal sealed class FitDecodeSnapshot
{
    /// <summary>Optional human-readable name identifying the source (file path, stream label).</summary>
    public string? SourceName { get; init; }

    /// <summary>Whether a <see cref="FitException"/> was caught during decode.</summary>
    public bool HadDecodeFault { get; init; }

    /// <summary>The message from the decode fault exception, if any.</summary>
    public string? DecodeFaultMessage { get; init; }

    /// <summary>The fully qualified exception type name of the decode fault, if any.</summary>
    public string? DecodeFaultExceptionType { get; init; }

    /// <summary>The total number of messages successfully decoded before the snapshot was taken.</summary>
    public int TotalMessageCount { get; init; }

    /// <summary>The decode mode used for this snapshot.</summary>
    public FitDecodeMode Mode { get; init; }

    /// <summary>File ID messages.</summary>
    public List<(FileIdMesg Message, int Sequence)> FileIdMesgs { get; init; } = [];

    /// <summary>File creator messages.</summary>
    public List<(FileCreatorMesg Message, int Sequence)> FileCreatorMesgs { get; init; } = [];

    /// <summary>Device info messages.</summary>
    public List<(DeviceInfoMesg Message, int Sequence)> DeviceInfoMesgs { get; init; } = [];

    /// <summary>User profile messages.</summary>
    public List<(UserProfileMesg Message, int Sequence)> UserProfileMesgs { get; init; } = [];

    /// <summary>Activity messages.</summary>
    public List<(ActivityMesg Message, int Sequence)> ActivityMesgs { get; init; } = [];

    /// <summary>Session messages.</summary>
    public List<(SessionMesg Message, int Sequence)> SessionMesgs { get; init; } = [];

    /// <summary>Lap messages.</summary>
    public List<(LapMesg Message, int Sequence)> LapMesgs { get; init; } = [];

    /// <summary>Record messages.</summary>
    public List<(RecordMesg Message, int Sequence)> RecordMesgs { get; init; } = [];

    /// <summary>Event messages.</summary>
    public List<(EventMesg Message, int Sequence)> EventMesgs { get; init; } = [];

    /// <summary>Length messages (pool swim).</summary>
    public List<(LengthMesg Message, int Sequence)> LengthMesgs { get; init; } = [];

    /// <summary>HRV messages.</summary>
    public List<(HrvMesg Message, int Sequence)> HrvMesgs { get; init; } = [];

    /// <summary>HR messages (compressed heart rate).</summary>
    public List<(HrMesg Message, int Sequence)> HrMesgs { get; init; } = [];

    /// <summary>Zones target messages.</summary>
    public List<(ZonesTargetMesg Message, int Sequence)> ZonesTargetMesgs { get; init; } = [];

    /// <summary>Sport messages.</summary>
    public List<(SportMesg Message, int Sequence)> SportMesgs { get; init; } = [];

    /// <summary>Workout messages.</summary>
    public List<(WorkoutMesg Message, int Sequence)> WorkoutMesgs { get; init; } = [];

    /// <summary>Workout step messages.</summary>
    public List<(WorkoutStepMesg Message, int Sequence)> WorkoutStepMesgs { get; init; } = [];

    /// <summary>Course messages.</summary>
    public List<(CourseMesg Message, int Sequence)> CourseMesgs { get; init; } = [];

    /// <summary>Course point messages.</summary>
    public List<(CoursePointMesg Message, int Sequence)> CoursePointMesgs { get; init; } = [];

    /// <summary>Segment lap messages.</summary>
    public List<(SegmentLapMesg Message, int Sequence)> SegmentLapMesgs { get; init; } = [];

    /// <summary>Developer data ID messages.</summary>
    public List<(DeveloperDataIdMesg Message, int Sequence)> DeveloperDataIdMesgs { get; init; } = [];

    /// <summary>Field description messages.</summary>
    public List<(FieldDescriptionMesg Message, int Sequence)> FieldDescriptionMesgs { get; init; } = [];

    /// <summary>Raw messages for SDK types not available as typed events (e.g., ClimbPro, Monitoring).</summary>
    public List<(Mesg Message, int Sequence)> UnhandledMesgs { get; init; } = [];
}
