using Dynastream.Fit;
using FitToMarkdown.Core.Parsing;

namespace FitToMarkdown.Fit.Decoding;

/// <summary>
/// Orchestrates the FIT decode pipeline: validation, broadcaster wiring, accumulation, and fault handling.
/// </summary>
internal sealed class FitDecodeCoordinator
{
    /// <summary>
    /// Decodes a FIT stream into a snapshot of accumulated SDK messages.
    /// </summary>
    /// <param name="input">The FIT data stream to decode.</param>
    /// <param name="sourceName">Optional human-readable source identifier.</param>
    /// <param name="options">Parse options influencing decode behavior.</param>
    /// <param name="mode">Whether to perform a full parse or metadata-only inspection.</param>
    /// <returns>A snapshot of all accumulated messages.</returns>
    public FitDecodeSnapshot Decode(Stream input, string? sourceName, FitParseOptions options, FitDecodeMode mode)
    {
        var decoder = new Decode();

        // Step 1: IsFIT check
        if (!decoder.IsFIT(input))
        {
            return new FitDecodeSnapshot
            {
                SourceName = sourceName,
                HadDecodeFault = true,
                DecodeFaultMessage = "Stream does not contain a valid FIT file header.",
                DecodeFaultExceptionType = null,
                TotalMessageCount = 0,
                Mode = mode,
            };
        }

        input.Position = 0;

        // Step 2: Create broadcaster
        var (broadcaster, _) = FitBroadcasterFactory.Create(decoder, options);

        // Step 3: Create accumulator and register listeners
        var accumulator = new FitMessageAccumulator();
        RegisterListeners(broadcaster, accumulator, mode);

        // Step 4: Decode with fault handling
        bool hadFault = false;
        string? faultMessage = null;
        string? faultExceptionType = null;

        try
        {
            decoder.Read(input);
        }
        catch (FitException ex)
        {
            // Truncated or corrupt — partial data in accumulator is still usable
            hadFault = true;
            faultMessage = accumulator.FileIdCount > 0
                ? $"Chained-file fault after {accumulator.FileIdCount} valid segment(s): {ex.Message}"
                : ex.Message;
            faultExceptionType = ex.GetType().FullName;
        }

        return accumulator.ToSnapshot(sourceName, hadFault, faultMessage, faultExceptionType, mode);
    }

    /// <summary>
    /// Registers accumulator event handlers on the broadcaster based on decode mode.
    /// </summary>
    private static void RegisterListeners(MesgBroadcaster broadcaster, FitMessageAccumulator accumulator, FitDecodeMode mode)
    {
        // Metadata-relevant listeners (always registered)
        broadcaster.FileIdMesgEvent += accumulator.OnFileIdMesg;
        broadcaster.FileCreatorMesgEvent += accumulator.OnFileCreatorMesg;
        broadcaster.DeviceInfoMesgEvent += accumulator.OnDeviceInfoMesg;
        broadcaster.ActivityMesgEvent += accumulator.OnActivityMesg;
        broadcaster.SessionMesgEvent += accumulator.OnSessionMesg;
        broadcaster.SportMesgEvent += accumulator.OnSportMesg;

        if (mode == FitDecodeMode.MetadataOnly)
        {
            return;
        }

        // Full-parse listeners
        broadcaster.UserProfileMesgEvent += accumulator.OnUserProfileMesg;
        broadcaster.LapMesgEvent += accumulator.OnLapMesg;
        broadcaster.RecordMesgEvent += accumulator.OnRecordMesg;
        broadcaster.EventMesgEvent += accumulator.OnEventMesg;
        broadcaster.LengthMesgEvent += accumulator.OnLengthMesg;
        broadcaster.HrvMesgEvent += accumulator.OnHrvMesg;
        broadcaster.HrMesgEvent += accumulator.OnHrMesg;
        broadcaster.ZonesTargetMesgEvent += accumulator.OnZonesTargetMesg;
        broadcaster.WorkoutMesgEvent += accumulator.OnWorkoutMesg;
        broadcaster.WorkoutStepMesgEvent += accumulator.OnWorkoutStepMesg;
        broadcaster.CourseMesgEvent += accumulator.OnCourseMesg;
        broadcaster.CoursePointMesgEvent += accumulator.OnCoursePointMesg;
        broadcaster.SegmentLapMesgEvent += accumulator.OnSegmentLapMesg;
        broadcaster.DeveloperDataIdMesgEvent += accumulator.OnDeveloperDataIdMesg;
        broadcaster.FieldDescriptionMesgEvent += accumulator.OnFieldDescriptionMesg;
    }
}
