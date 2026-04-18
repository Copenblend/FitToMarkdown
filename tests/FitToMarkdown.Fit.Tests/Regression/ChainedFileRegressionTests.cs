using Dynastream.Fit;
using FluentAssertions;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Parsing;
using FitToMarkdown.Fit.Decoding;
using FitToMarkdown.Fit.Mapping;
using FitToMarkdown.Fit.Tests.Fixtures;
using FitToMarkdown.Fit.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Fit.Tests.Regression;

public sealed class ChainedFileRegressionTests
{
    private readonly FitDocumentBuilder _sut = new();

    [Fact]
    public void SnapshotWithFileId_AndDecodeFault_ProducesUsableDocument()
    {
        var start = new DateTimeOffset(2024, 6, 15, 8, 0, 0, TimeSpan.Zero);

        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Activity);
        snapshot.RecordMesgs.Add((FitSdkMessageBuilder.CreateRecord(start.AddMinutes(10), heartRate: 140), 1));
        snapshot = CloneWithFault(snapshot, "Chained-file fault after 1 valid segment(s): CRC mismatch");

        var options = FitParseOptionsPresets.DefaultParse();

        var result = _sut.Build(snapshot, options);

        result.Document.Should().NotBeNull("at least one segment decoded successfully");
        result.Metadata.Status.Should().Be(FitParseStatus.PartiallySucceeded);
        result.Issues.Should().Contain(i => i.Code == "fit.decode-fault");
    }

    [Fact]
    public void SnapshotWithNoFileId_AndDecodeFault_IsFatal()
    {
        var snapshot = SnapshotFactory.CreateEmpty();
        snapshot = CloneWithFault(snapshot, "Fault at start of stream");

        var options = new FitParseOptions
        {
            AllowPartialExtraction = false,
        };

        var result = _sut.Build(snapshot, options);

        result.FatalError.Should().NotBeNull();
        result.Metadata.Status.Should().Be(FitParseStatus.Failed);
    }

    [Fact]
    public void SnapshotWithRecords_AndDecodeFault_ProducesPartialRecovery()
    {
        var start = new DateTimeOffset(2024, 6, 15, 8, 0, 0, TimeSpan.Zero);

        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Activity);
        snapshot.RecordMesgs.Add((FitSdkMessageBuilder.CreateRecord(start.AddMinutes(5), heartRate: 120), 1));
        snapshot.RecordMesgs.Add((FitSdkMessageBuilder.CreateRecord(start.AddMinutes(25), heartRate: 155), 2));
        snapshot = CloneWithFault(snapshot, "Truncated after records");

        var options = FitParseOptionsPresets.DefaultParse();

        var result = _sut.Build(snapshot, options);

        result.Document.Should().NotBeNull();
        result.Metadata.Status.Should().Be(FitParseStatus.PartiallySucceeded);
        result.Issues.Should().Contain(i => i.Code == "fit.synthetic-session");
        result.Issues.Should().Contain(i => i.Code == "fit.synthetic-activity");
    }

    [Fact]
    public void CleanDecode_NoFault_ReturnsSucceeded()
    {
        var start = new DateTimeOffset(2024, 6, 15, 8, 0, 0, TimeSpan.Zero);
        var end = start.AddHours(1);

        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Activity);
        snapshot.ActivityMesgs.Add((FitSdkMessageBuilder.CreateActivity(end), 1));
        snapshot.SessionMesgs.Add((FitSdkMessageBuilder.CreateSession(start, end), 2));
        snapshot.LapMesgs.Add((FitSdkMessageBuilder.CreateLap(start, end), 3));
        snapshot.RecordMesgs.Add((FitSdkMessageBuilder.CreateRecord(start.AddMinutes(30), heartRate: 145), 4));

        var options = FitParseOptionsPresets.DefaultParse();

        var result = _sut.Build(snapshot, options);

        result.Metadata.Status.Should().Be(FitParseStatus.Succeeded);
        result.FatalError.Should().BeNull();
        result.Issues.Should().BeEmpty();
    }

    private static FitDecodeSnapshot CloneWithFault(FitDecodeSnapshot source, string faultMessage)
    {
        return new FitDecodeSnapshot
        {
            Mode = source.Mode,
            SourceName = source.SourceName,
            HadDecodeFault = true,
            DecodeFaultMessage = faultMessage,
            DecodeFaultExceptionType = "Dynastream.Fit.FitException",
            TotalMessageCount = source.TotalMessageCount,
            FileIdMesgs = source.FileIdMesgs,
            FileCreatorMesgs = source.FileCreatorMesgs,
            DeviceInfoMesgs = source.DeviceInfoMesgs,
            UserProfileMesgs = source.UserProfileMesgs,
            ActivityMesgs = source.ActivityMesgs,
            SessionMesgs = source.SessionMesgs,
            LapMesgs = source.LapMesgs,
            RecordMesgs = source.RecordMesgs,
            EventMesgs = source.EventMesgs,
            LengthMesgs = source.LengthMesgs,
            HrvMesgs = source.HrvMesgs,
            HrMesgs = source.HrMesgs,
            ZonesTargetMesgs = source.ZonesTargetMesgs,
            SportMesgs = source.SportMesgs,
            WorkoutMesgs = source.WorkoutMesgs,
            WorkoutStepMesgs = source.WorkoutStepMesgs,
            CourseMesgs = source.CourseMesgs,
            CoursePointMesgs = source.CoursePointMesgs,
            SegmentLapMesgs = source.SegmentLapMesgs,
            DeveloperDataIdMesgs = source.DeveloperDataIdMesgs,
            FieldDescriptionMesgs = source.FieldDescriptionMesgs,
            UnhandledMesgs = source.UnhandledMesgs,
        };
    }
}
