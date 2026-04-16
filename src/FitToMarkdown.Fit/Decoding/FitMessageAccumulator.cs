using Dynastream.Fit;

namespace FitToMarkdown.Fit.Decoding;

/// <summary>
/// Accumulates typed SDK messages during a decode pass, maintaining parse-order sequencing.
/// </summary>
internal sealed class FitMessageAccumulator
{
    private int _parseSequence;

    private readonly List<(FileIdMesg Message, int Sequence)> _fileIdMesgs = [];
    private readonly List<(FileCreatorMesg Message, int Sequence)> _fileCreatorMesgs = [];
    private readonly List<(DeviceInfoMesg Message, int Sequence)> _deviceInfoMesgs = [];
    private readonly List<(UserProfileMesg Message, int Sequence)> _userProfileMesgs = [];
    private readonly List<(ActivityMesg Message, int Sequence)> _activityMesgs = [];
    private readonly List<(SessionMesg Message, int Sequence)> _sessionMesgs = [];
    private readonly List<(LapMesg Message, int Sequence)> _lapMesgs = [];
    private readonly List<(RecordMesg Message, int Sequence)> _recordMesgs = [];
    private readonly List<(EventMesg Message, int Sequence)> _eventMesgs = [];
    private readonly List<(LengthMesg Message, int Sequence)> _lengthMesgs = [];
    private readonly List<(HrvMesg Message, int Sequence)> _hrvMesgs = [];
    private readonly List<(HrMesg Message, int Sequence)> _hrMesgs = [];
    private readonly List<(ZonesTargetMesg Message, int Sequence)> _zonesTargetMesgs = [];
    private readonly List<(SportMesg Message, int Sequence)> _sportMesgs = [];
    private readonly List<(WorkoutMesg Message, int Sequence)> _workoutMesgs = [];
    private readonly List<(WorkoutStepMesg Message, int Sequence)> _workoutStepMesgs = [];
    private readonly List<(CourseMesg Message, int Sequence)> _courseMesgs = [];
    private readonly List<(CoursePointMesg Message, int Sequence)> _coursePointMesgs = [];
    private readonly List<(SegmentLapMesg Message, int Sequence)> _segmentLapMesgs = [];
    private readonly List<(DeveloperDataIdMesg Message, int Sequence)> _developerDataIdMesgs = [];
    private readonly List<(FieldDescriptionMesg Message, int Sequence)> _fieldDescriptionMesgs = [];
    private readonly List<(Mesg Message, int Sequence)> _unhandledMesgs = [];

    /// <summary>Handles FileIdMesg broadcast events.</summary>
    public void OnFileIdMesg(object sender, MesgEventArgs e)
    {
        if (e.mesg is FileIdMesg typed)
            _fileIdMesgs.Add((typed, _parseSequence++));
    }

    /// <summary>Handles FileCreatorMesg broadcast events.</summary>
    public void OnFileCreatorMesg(object sender, MesgEventArgs e)
    {
        if (e.mesg is FileCreatorMesg typed)
            _fileCreatorMesgs.Add((typed, _parseSequence++));
    }

    /// <summary>Handles DeviceInfoMesg broadcast events.</summary>
    public void OnDeviceInfoMesg(object sender, MesgEventArgs e)
    {
        if (e.mesg is DeviceInfoMesg typed)
            _deviceInfoMesgs.Add((typed, _parseSequence++));
    }

    /// <summary>Handles UserProfileMesg broadcast events.</summary>
    public void OnUserProfileMesg(object sender, MesgEventArgs e)
    {
        if (e.mesg is UserProfileMesg typed)
            _userProfileMesgs.Add((typed, _parseSequence++));
    }

    /// <summary>Handles ActivityMesg broadcast events.</summary>
    public void OnActivityMesg(object sender, MesgEventArgs e)
    {
        if (e.mesg is ActivityMesg typed)
            _activityMesgs.Add((typed, _parseSequence++));
    }

    /// <summary>Handles SessionMesg broadcast events.</summary>
    public void OnSessionMesg(object sender, MesgEventArgs e)
    {
        if (e.mesg is SessionMesg typed)
            _sessionMesgs.Add((typed, _parseSequence++));
    }

    /// <summary>Handles LapMesg broadcast events.</summary>
    public void OnLapMesg(object sender, MesgEventArgs e)
    {
        if (e.mesg is LapMesg typed)
            _lapMesgs.Add((typed, _parseSequence++));
    }

    /// <summary>Handles RecordMesg broadcast events.</summary>
    public void OnRecordMesg(object sender, MesgEventArgs e)
    {
        if (e.mesg is RecordMesg typed)
            _recordMesgs.Add((typed, _parseSequence++));
    }

    /// <summary>Handles EventMesg broadcast events.</summary>
    public void OnEventMesg(object sender, MesgEventArgs e)
    {
        if (e.mesg is EventMesg typed)
            _eventMesgs.Add((typed, _parseSequence++));
    }

    /// <summary>Handles LengthMesg broadcast events.</summary>
    public void OnLengthMesg(object sender, MesgEventArgs e)
    {
        if (e.mesg is LengthMesg typed)
            _lengthMesgs.Add((typed, _parseSequence++));
    }

    /// <summary>Handles HrvMesg broadcast events.</summary>
    public void OnHrvMesg(object sender, MesgEventArgs e)
    {
        if (e.mesg is HrvMesg typed)
            _hrvMesgs.Add((typed, _parseSequence++));
    }

    /// <summary>Handles HrMesg broadcast events.</summary>
    public void OnHrMesg(object sender, MesgEventArgs e)
    {
        if (e.mesg is HrMesg typed)
            _hrMesgs.Add((typed, _parseSequence++));
    }

    /// <summary>Handles ZonesTargetMesg broadcast events.</summary>
    public void OnZonesTargetMesg(object sender, MesgEventArgs e)
    {
        if (e.mesg is ZonesTargetMesg typed)
            _zonesTargetMesgs.Add((typed, _parseSequence++));
    }

    /// <summary>Handles SportMesg broadcast events.</summary>
    public void OnSportMesg(object sender, MesgEventArgs e)
    {
        if (e.mesg is SportMesg typed)
            _sportMesgs.Add((typed, _parseSequence++));
    }

    /// <summary>Handles WorkoutMesg broadcast events.</summary>
    public void OnWorkoutMesg(object sender, MesgEventArgs e)
    {
        if (e.mesg is WorkoutMesg typed)
            _workoutMesgs.Add((typed, _parseSequence++));
    }

    /// <summary>Handles WorkoutStepMesg broadcast events.</summary>
    public void OnWorkoutStepMesg(object sender, MesgEventArgs e)
    {
        if (e.mesg is WorkoutStepMesg typed)
            _workoutStepMesgs.Add((typed, _parseSequence++));
    }

    /// <summary>Handles CourseMesg broadcast events.</summary>
    public void OnCourseMesg(object sender, MesgEventArgs e)
    {
        if (e.mesg is CourseMesg typed)
            _courseMesgs.Add((typed, _parseSequence++));
    }

    /// <summary>Handles CoursePointMesg broadcast events.</summary>
    public void OnCoursePointMesg(object sender, MesgEventArgs e)
    {
        if (e.mesg is CoursePointMesg typed)
            _coursePointMesgs.Add((typed, _parseSequence++));
    }

    /// <summary>Handles SegmentLapMesg broadcast events.</summary>
    public void OnSegmentLapMesg(object sender, MesgEventArgs e)
    {
        if (e.mesg is SegmentLapMesg typed)
            _segmentLapMesgs.Add((typed, _parseSequence++));
    }

    /// <summary>Handles DeveloperDataIdMesg broadcast events.</summary>
    public void OnDeveloperDataIdMesg(object sender, MesgEventArgs e)
    {
        if (e.mesg is DeveloperDataIdMesg typed)
            _developerDataIdMesgs.Add((typed, _parseSequence++));
    }

    /// <summary>Handles FieldDescriptionMesg broadcast events.</summary>
    public void OnFieldDescriptionMesg(object sender, MesgEventArgs e)
    {
        if (e.mesg is FieldDescriptionMesg typed)
            _fieldDescriptionMesgs.Add((typed, _parseSequence++));
    }

    /// <summary>Handles raw Mesg events for message types without typed broadcaster events.</summary>
    public void OnUnhandledMesg(object sender, MesgEventArgs e)
    {
        _unhandledMesgs.Add((e.mesg, _parseSequence++));
    }

    /// <summary>
    /// Creates an immutable snapshot from the accumulated messages.
    /// </summary>
    /// <param name="sourceName">Optional human-readable source identifier.</param>
    /// <param name="hadDecodeFault">Whether a decode fault was caught.</param>
    /// <param name="faultMessage">The fault exception message, if any.</param>
    /// <param name="faultExceptionType">The fault exception type name, if any.</param>
    /// <param name="mode">The decode mode used.</param>
    /// <returns>An immutable snapshot of accumulated messages.</returns>
    public FitDecodeSnapshot ToSnapshot(
        string? sourceName,
        bool hadDecodeFault,
        string? faultMessage,
        string? faultExceptionType,
        FitDecodeMode mode)
    {
        return new FitDecodeSnapshot
        {
            SourceName = sourceName,
            HadDecodeFault = hadDecodeFault,
            DecodeFaultMessage = faultMessage,
            DecodeFaultExceptionType = faultExceptionType,
            TotalMessageCount = _parseSequence,
            Mode = mode,
            FileIdMesgs = [.. _fileIdMesgs],
            FileCreatorMesgs = [.. _fileCreatorMesgs],
            DeviceInfoMesgs = [.. _deviceInfoMesgs],
            UserProfileMesgs = [.. _userProfileMesgs],
            ActivityMesgs = [.. _activityMesgs],
            SessionMesgs = [.. _sessionMesgs],
            LapMesgs = [.. _lapMesgs],
            RecordMesgs = [.. _recordMesgs],
            EventMesgs = [.. _eventMesgs],
            LengthMesgs = [.. _lengthMesgs],
            HrvMesgs = [.. _hrvMesgs],
            HrMesgs = [.. _hrMesgs],
            ZonesTargetMesgs = [.. _zonesTargetMesgs],
            SportMesgs = [.. _sportMesgs],
            WorkoutMesgs = [.. _workoutMesgs],
            WorkoutStepMesgs = [.. _workoutStepMesgs],
            CourseMesgs = [.. _courseMesgs],
            CoursePointMesgs = [.. _coursePointMesgs],
            SegmentLapMesgs = [.. _segmentLapMesgs],
            DeveloperDataIdMesgs = [.. _developerDataIdMesgs],
            FieldDescriptionMesgs = [.. _fieldDescriptionMesgs],
            UnhandledMesgs = [.. _unhandledMesgs],
        };
    }
}
