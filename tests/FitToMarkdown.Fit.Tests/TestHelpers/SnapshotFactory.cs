using Dynastream.Fit;
using FitToMarkdown.Fit.Decoding;

namespace FitToMarkdown.Fit.Tests.TestHelpers;

internal static class SnapshotFactory
{
    public static FitDecodeSnapshot CreateEmpty(FitDecodeMode mode = FitDecodeMode.FullParse)
    {
        return new FitDecodeSnapshot
        {
            Mode = mode,
            FileIdMesgs = [],
            FileCreatorMesgs = [],
            DeviceInfoMesgs = [],
            UserProfileMesgs = [],
            ActivityMesgs = [],
            SessionMesgs = [],
            LapMesgs = [],
            RecordMesgs = [],
            EventMesgs = [],
            LengthMesgs = [],
            HrvMesgs = [],
            HrMesgs = [],
            ZonesTargetMesgs = [],
            SportMesgs = [],
            WorkoutMesgs = [],
            WorkoutStepMesgs = [],
            CourseMesgs = [],
            CoursePointMesgs = [],
            SegmentLapMesgs = [],
            DeveloperDataIdMesgs = [],
            FieldDescriptionMesgs = [],
            UnhandledMesgs = [],
        };
    }

    public static FitDecodeSnapshot CreateWithFileId(
        Dynastream.Fit.File fileType = Dynastream.Fit.File.Activity,
        ushort manufacturer = 1,
        ushort product = 1234,
        uint serialNumber = 123456789,
        string? sourceName = "test.fit",
        FitDecodeMode mode = FitDecodeMode.FullParse)
    {
        var fileIdMesg = new FileIdMesg();
        fileIdMesg.SetType(fileType);
        fileIdMesg.SetManufacturer(manufacturer);
        fileIdMesg.SetProduct(product);
        fileIdMesg.SetSerialNumber(serialNumber);

        var snapshot = CreateEmpty(mode);
        snapshot.FileIdMesgs.Add((fileIdMesg, 0));

        return new FitDecodeSnapshot
        {
            Mode = mode,
            SourceName = sourceName,
            FileIdMesgs = snapshot.FileIdMesgs,
            FileCreatorMesgs = snapshot.FileCreatorMesgs,
            DeviceInfoMesgs = snapshot.DeviceInfoMesgs,
            UserProfileMesgs = snapshot.UserProfileMesgs,
            ActivityMesgs = snapshot.ActivityMesgs,
            SessionMesgs = snapshot.SessionMesgs,
            LapMesgs = snapshot.LapMesgs,
            RecordMesgs = snapshot.RecordMesgs,
            EventMesgs = snapshot.EventMesgs,
            LengthMesgs = snapshot.LengthMesgs,
            HrvMesgs = snapshot.HrvMesgs,
            HrMesgs = snapshot.HrMesgs,
            ZonesTargetMesgs = snapshot.ZonesTargetMesgs,
            SportMesgs = snapshot.SportMesgs,
            WorkoutMesgs = snapshot.WorkoutMesgs,
            WorkoutStepMesgs = snapshot.WorkoutStepMesgs,
            CourseMesgs = snapshot.CourseMesgs,
            CoursePointMesgs = snapshot.CoursePointMesgs,
            SegmentLapMesgs = snapshot.SegmentLapMesgs,
            DeveloperDataIdMesgs = snapshot.DeveloperDataIdMesgs,
            FieldDescriptionMesgs = snapshot.FieldDescriptionMesgs,
            UnhandledMesgs = snapshot.UnhandledMesgs,
        };
    }
}
