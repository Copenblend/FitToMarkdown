using Dynastream.Fit;
using FitToMarkdown.Core.Enums;

namespace FitToMarkdown.Fit.Mapping;

/// <summary>
/// Maps SDK enum types to their Core enum counterparts via numeric value preservation.
/// </summary>
internal static class FitEnumMapper
{
    public static FitSport? MapSport(Sport? sdkSport) =>
        sdkSport.HasValue ? (FitSport)(byte)sdkSport.Value : null;

    public static FitSubSport? MapSubSport(SubSport? sdkSubSport) =>
        sdkSubSport.HasValue ? (FitSubSport)(byte)sdkSubSport.Value : null;

    public static FitEventKind? MapEvent(Event? sdkEvent) =>
        sdkEvent.HasValue ? (FitEventKind)(byte)sdkEvent.Value : null;

    public static FitEventAction? MapEventType(EventType? sdkEventType) =>
        sdkEventType.HasValue ? (FitEventAction)(byte)sdkEventType.Value : null;

    public static FitSessionTrigger? MapSessionTrigger(SessionTrigger? t) =>
        t.HasValue ? (FitSessionTrigger)(byte)t.Value : null;

    public static FitLapTrigger? MapLapTrigger(LapTrigger? t) =>
        t.HasValue ? (FitLapTrigger)(byte)t.Value : null;

    public static FitSwimStroke? MapSwimStroke(SwimStroke? s) =>
        s.HasValue ? (FitSwimStroke)(byte)s.Value : null;

    public static FitLengthType? MapLengthType(LengthType? t) =>
        t.HasValue ? (FitLengthType)(byte)t.Value : null;

    public static FitActivityType? MapActivityType(ActivityType? t) =>
        t.HasValue ? (FitActivityType)(byte)t.Value : null;

    public static FitGender? MapGender(Gender? g) =>
        g.HasValue ? (FitGender)(byte)g.Value : null;

    public static FitFileType? MapFileType(Dynastream.Fit.File? sdkType) =>
        sdkType.HasValue ? (FitFileType)(byte)sdkType.Value : null;

    public static FitBatteryStatus? MapBatteryStatus(byte? sdkStatus) =>
        sdkStatus.HasValue && sdkStatus.Value >= 1 && sdkStatus.Value <= 7
            ? (FitBatteryStatus)sdkStatus.Value
            : null;

    public static FitCoursePointType? MapCoursePointType(CoursePoint? sdkType) =>
        sdkType.HasValue ? (FitCoursePointType)(byte)sdkType.Value : null;

    public static FitIntensity? MapIntensity(Intensity? sdkIntensity) =>
        sdkIntensity.HasValue ? (FitIntensity)(byte)sdkIntensity.Value : null;

    public static FitWorkoutStepDurationType? MapWorkoutStepDurationType(WktStepDuration? d) =>
        d.HasValue ? (FitWorkoutStepDurationType)(byte)d.Value : null;

    public static FitWorkoutTargetType? MapWorkoutTargetType(WktStepTarget? t) =>
        t.HasValue ? (FitWorkoutTargetType)(byte)t.Value : null;

    public static FitDeviceRole? MapDeviceRole(ushort? deviceIndex) =>
        deviceIndex switch
        {
            null => null,
            0 => FitDeviceRole.Creator,
            _ => FitDeviceRole.Unknown,
        };

    public static FitLengthUnit? MapDisplayMeasure(DisplayMeasure? dm) =>
        dm.HasValue ? (FitLengthUnit)(byte)dm.Value : null;
}
