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

public sealed class ParseOutcomePolishTests
{
    private readonly FitDocumentBuilder _sut = new();

    [Fact]
    public void CleanActivity_ReturnsSucceeded_WithNoIssues()
    {
        var start = new DateTimeOffset(2024, 6, 15, 8, 0, 0, TimeSpan.Zero);
        var end = start.AddHours(1);

        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Activity);
        snapshot.ActivityMesgs.Add((FitSdkMessageBuilder.CreateActivity(end), 1));
        snapshot.SessionMesgs.Add((FitSdkMessageBuilder.CreateSession(start, end), 2));
        snapshot.LapMesgs.Add((FitSdkMessageBuilder.CreateLap(start, end), 3));
        snapshot.RecordMesgs.Add((FitSdkMessageBuilder.CreateRecord(start.AddMinutes(30), heartRate: 140), 4));

        var options = FitParseOptionsPresets.DefaultParse();

        var result = _sut.Build(snapshot, options);

        result.Metadata.Status.Should().Be(FitParseStatus.Succeeded);
        result.Issues.Should().BeEmpty();
        result.Document.Should().NotBeNull();
        result.FatalError.Should().BeNull();
    }

    [Fact]
    public void TruncatedActivity_MissingActivityOnly_ReturnsPartiallySucceeded_WithSyntheticActivityIssue()
    {
        var start = new DateTimeOffset(2024, 6, 15, 8, 0, 0, TimeSpan.Zero);
        var end = start.AddHours(1);

        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Activity);
        // Has sessions, laps, records but no Activity message — and had decode fault
        snapshot.SessionMesgs.Add((FitSdkMessageBuilder.CreateSession(start, end), 1));
        snapshot.LapMesgs.Add((FitSdkMessageBuilder.CreateLap(start, end), 2));
        snapshot.RecordMesgs.Add((FitSdkMessageBuilder.CreateRecord(start.AddMinutes(30), heartRate: 140), 3));
        snapshot = CloneWithFault(snapshot, "Truncated before ActivityMesg");

        var options = FitParseOptionsPresets.DefaultParse();

        var result = _sut.Build(snapshot, options);

        result.Metadata.Status.Should().Be(FitParseStatus.PartiallySucceeded);
        result.Issues.Should().Contain(i => i.Code == "fit.synthetic-activity");
        result.Document.Should().NotBeNull();
        result.FatalError.Should().BeNull();
    }

    [Fact]
    public void TruncatedActivity_MissingSessionAndActivity_ReturnsPartiallySucceeded_WithBothSyntheticIssues()
    {
        var start = new DateTimeOffset(2024, 6, 15, 8, 0, 0, TimeSpan.Zero);

        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Activity);
        // No Activity, no Session — only records + decode fault
        snapshot.RecordMesgs.Add((FitSdkMessageBuilder.CreateRecord(start.AddMinutes(15), heartRate: 130), 1));
        snapshot.RecordMesgs.Add((FitSdkMessageBuilder.CreateRecord(start.AddMinutes(45), heartRate: 160), 2));
        snapshot = CloneWithFault(snapshot, "Truncated early");

        var options = FitParseOptionsPresets.DefaultParse();

        var result = _sut.Build(snapshot, options);

        result.Metadata.Status.Should().Be(FitParseStatus.PartiallySucceeded);
        result.Issues.Should().Contain(i => i.Code == "fit.synthetic-session");
        result.Issues.Should().Contain(i => i.Code == "fit.synthetic-activity");
        result.Document.Should().NotBeNull();
    }

    [Fact]
    public void MonitoringFile_WithDecodeFault_ReturnsFailed()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.MonitoringA);
        snapshot = CloneWithFault(snapshot, "CRC mismatch");

        var options = FitParseOptionsPresets.DefaultParse();

        var result = _sut.Build(snapshot, options);

        result.FatalError.Should().NotBeNull();
        result.FatalError!.Code.Should().Be("fit.monitoring-integrity-failed");
        result.Issues.Should().Contain(i => i.Code == "fit.monitoring-integrity-failed");
    }

    [Fact]
    public void NonActivityFile_WithDecodeFault_AndIntegrityValidation_ReturnsFailed()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Workout);
        snapshot = CloneWithFault(snapshot, "CRC mismatch");

        var options = new FitParseOptions
        {
            AllowPartialExtraction = false,
            ValidateIntegrityForNonActivityFiles = true,
        };

        var result = _sut.Build(snapshot, options);

        result.Metadata.Status.Should().Be(FitParseStatus.Failed);
        result.FatalError.Should().NotBeNull();
    }

    [Fact]
    public void ActivityFile_WithDecodeFault_AndAllowPartialExtraction_ReturnsPartiallySucceeded()
    {
        var start = new DateTimeOffset(2024, 6, 15, 8, 0, 0, TimeSpan.Zero);

        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Activity);
        snapshot.RecordMesgs.Add((FitSdkMessageBuilder.CreateRecord(start.AddMinutes(10), heartRate: 135), 1));
        snapshot = CloneWithFault(snapshot, "Truncated at byte 4096");

        var options = new FitParseOptions
        {
            AllowPartialExtraction = true,
            RecoverTruncatedActivityFiles = true,
        };

        var result = _sut.Build(snapshot, options);

        result.Metadata.Status.Should().Be(FitParseStatus.PartiallySucceeded);
        result.Document.Should().NotBeNull();
        result.FatalError.Should().BeNull();
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
