using Dynastream.Fit;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Fit.Decoding;

using FitFile = Dynastream.Fit.File;

namespace FitToMarkdown.Fit.Internal;

/// <summary>
/// Classifies the FIT file type from the first FileIdMesg in a decode snapshot.
/// </summary>
internal static class FitFileTypeClassifier
{
    /// <summary>
    /// Reads the first FileIdMesg and returns the corresponding <see cref="FitFileType"/>.
    /// </summary>
    /// <param name="snapshot">The decoded snapshot to classify.</param>
    /// <returns>The classified file type, or <c>null</c> if no FileIdMesg is present or the type is unrecognized.</returns>
    public static FitFileType? Classify(FitDecodeSnapshot snapshot)
    {
        if (snapshot.FileIdMesgs.Count == 0)
        {
            return null;
        }

        var fileIdMesg = snapshot.FileIdMesgs[0].Message;
        var fileType = fileIdMesg.GetType();

        if (fileType is null)
        {
            return null;
        }

        // The SDK File enum values align with FitFileType numeric values
        return fileType switch
        {
            FitFile.Device => FitFileType.Device,
            FitFile.Settings => FitFileType.Settings,
            FitFile.Sport => FitFileType.Sport,
            FitFile.Activity => FitFileType.Activity,
            FitFile.Workout => FitFileType.Workout,
            FitFile.Course => FitFileType.Course,
            FitFile.Schedules => FitFileType.Schedules,
            FitFile.Weight => FitFileType.Weight,
            FitFile.Totals => FitFileType.Totals,
            FitFile.Goals => FitFileType.Goals,
            FitFile.BloodPressure => FitFileType.BloodPressure,
            FitFile.MonitoringA => FitFileType.MonitoringA,
            FitFile.ActivitySummary => FitFileType.ActivitySummary,
            FitFile.MonitoringDaily => FitFileType.MonitoringDaily,
            FitFile.MonitoringB => FitFileType.MonitoringB,
            FitFile.Segment => FitFileType.Segment,
            FitFile.SegmentList => FitFileType.SegmentList,
            FitFile.ExdConfiguration => FitFileType.ExdConfiguration,
            _ => null,
        };
    }
}
