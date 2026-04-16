using Dynastream.Fit;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.ValueObjects;
using FitToMarkdown.Fit.Internal;
using static FitToMarkdown.Fit.Mapping.FitEnumMapper;
using static FitToMarkdown.Fit.Mapping.FitFieldValueConverter;

namespace FitToMarkdown.Fit.Mapping;

/// <summary>
/// Maps WorkoutMesg, WorkoutStepMesg, CourseMesg, and CoursePointMesg to Core models.
/// </summary>
internal static class WorkoutAndCourseMapper
{
    /// <summary>
    /// Maps the first WorkoutMesg and all WorkoutStepMesg items to a Core FitWorkout.
    /// </summary>
    public static FitWorkout? MapWorkout(
        List<(WorkoutMesg Message, int Sequence)> workouts,
        List<(WorkoutStepMesg Message, int Sequence)> steps,
        FitDeveloperFieldResolver? devResolver)
    {
        if (workouts.Count == 0)
            return null;

        var (mesg, seq) = workouts[0];

        return new FitWorkout
        {
            Message = FitMessageIdentityFactory.Create(seq),
            WorkoutName = ByteArrayToString(mesg.GetWktName()),
            Sport = MapSport(mesg.GetSport()),
            SubSport = MapSubSport(mesg.GetSubSport()),
            NumberOfValidSteps = ToNullableUshort(mesg.GetNumValidSteps()),
            Steps = MapWorkoutSteps(steps, devResolver),
            DeveloperFields = devResolver?.ResolveDeveloperFields(mesg) ?? [],
        };
    }

    /// <summary>
    /// Maps the first CourseMesg and all CoursePointMesg items to a Core FitCourse.
    /// </summary>
    public static FitCourse? MapCourse(
        List<(CourseMesg Message, int Sequence)> courses,
        List<(CoursePointMesg Message, int Sequence)> points,
        FitDeveloperFieldResolver? devResolver)
    {
        if (courses.Count == 0)
            return null;

        var (mesg, seq) = courses[0];

        return new FitCourse
        {
            Message = FitMessageIdentityFactory.Create(seq),
            Sport = MapSport(mesg.GetSport()),
            CourseName = ByteArrayToString(mesg.GetName()),
            CapabilitiesMask = ToNullableUint(mesg.GetCapabilities()),
            Points = MapCoursePoints(points, devResolver),
            DeveloperFields = devResolver?.ResolveDeveloperFields(mesg) ?? [],
        };
    }

    private static IReadOnlyList<FitWorkoutStep> MapWorkoutSteps(
        List<(WorkoutStepMesg Message, int Sequence)> items,
        FitDeveloperFieldResolver? devResolver)
    {
        if (items.Count == 0)
            return [];

        var results = new List<FitWorkoutStep>(items.Count);

        for (int i = 0; i < items.Count; i++)
        {
            var (mesg, seq) = items[i];
            var targetType = MapWorkoutTargetType(mesg.GetTargetType());
            var targetValue = ToNullableUint(mesg.GetTargetValue());
            var customLow = ToNullableUint(mesg.GetCustomTargetValueLow());
            var customHigh = ToNullableUint(mesg.GetCustomTargetValueHigh());

            WorkoutTargetRange? targetRange = targetType is not null
                ? new WorkoutTargetRange
                {
                    TargetValue = targetValue.HasValue ? (double?)targetValue.Value : null,
                    CustomLow = customLow.HasValue ? (double?)customLow.Value : null,
                    CustomHigh = customHigh.HasValue ? (double?)customHigh.Value : null,
                }
                : null;

            results.Add(new FitWorkoutStep
            {
                Message = FitMessageIdentityFactory.Create(seq, mesg.GetMessageIndex()),
                StepIndex = i,
                StepName = ByteArrayToString(mesg.GetWktStepName()),
                DurationType = MapWorkoutStepDurationType(mesg.GetDurationType()),
                DurationValue = ToNullableUint(mesg.GetDurationValue()) is uint dv ? (double?)dv : null,
                TargetType = targetType,
                TargetRange = targetRange,
                Intensity = MapIntensity(mesg.GetIntensity()),
                Notes = ByteArrayToString(mesg.GetNotes()),
                Equipment = null,
                DeveloperFields = devResolver?.ResolveDeveloperFields(mesg) ?? [],
            });
        }

        return results;
    }

    private static IReadOnlyList<FitCoursePoint> MapCoursePoints(
        List<(CoursePointMesg Message, int Sequence)> items,
        FitDeveloperFieldResolver? devResolver)
    {
        if (items.Count == 0)
            return [];

        var results = new List<FitCoursePoint>(items.Count);

        for (int i = 0; i < items.Count; i++)
        {
            var (mesg, seq) = items[i];

            results.Add(new FitCoursePoint
            {
                Message = FitMessageIdentityFactory.Create(seq, mesg.GetMessageIndex()),
                PointIndex = i,
                TimestampUtc = FitTimestampNormalizer.ToUtcDateTimeOffset(mesg.GetTimestamp()),
                Position = FitCoordinateNormalizer.ToGeoCoordinate(
                    mesg.GetPositionLat(), mesg.GetPositionLong()),
                DistanceMeters = ToNullableDouble(mesg.GetDistance()),
                PointType = MapCoursePointType(mesg.GetType()),
                PointName = ByteArrayToString(mesg.GetName()),
                DeveloperFields = devResolver?.ResolveDeveloperFields(mesg) ?? [],
            });
        }

        return results;
    }
}
