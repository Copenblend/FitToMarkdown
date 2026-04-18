using Dynastream.Fit;
using FluentAssertions;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Parsing;
using FitToMarkdown.Fit.Decoding;
using FitToMarkdown.Fit.Mapping;
using FitToMarkdown.Fit.Tests.Fixtures;
using FitToMarkdown.Fit.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Fit.Tests.Integration.Parser;

public sealed class ParserEnvelopeInvariantTests
{
    private readonly FitDocumentBuilder _builder = new();

    private static readonly DateTimeOffset Start = new(2024, 6, 15, 8, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset End = Start.AddHours(1);

    [Fact]
    public void Activity_Result_HasNonNullCollections()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Activity);
        snapshot.ActivityMesgs.Add((FitSdkMessageBuilder.CreateActivity(End), 1));
        snapshot.SessionMesgs.Add((FitSdkMessageBuilder.CreateSession(Start, End), 2));

        var result = _builder.Build(snapshot, FitParseOptionsPresets.DefaultParse());

        FitParseAssertions.AssertCollectionsNotNull(result);
    }

    [Fact]
    public void Activity_Result_WithSessions_HasGroupedLapsAndRecords()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Activity);
        snapshot.ActivityMesgs.Add((FitSdkMessageBuilder.CreateActivity(End), 1));
        snapshot.SessionMesgs.Add((FitSdkMessageBuilder.CreateSession(Start, End), 2));
        snapshot.LapMesgs.Add((FitSdkMessageBuilder.CreateLap(Start, End), 3));
        snapshot.RecordMesgs.Add((FitSdkMessageBuilder.CreateRecord(Start.AddMinutes(15), heartRate: 140), 4));
        snapshot.RecordMesgs.Add((FitSdkMessageBuilder.CreateRecord(Start.AddMinutes(45), heartRate: 160), 5));

        var result = _builder.Build(snapshot, FitParseOptionsPresets.DefaultParse());

        result.Document.Should().NotBeNull();
        result.Document!.ActivityContent.Should().NotBeNull();
        var session = result.Document.ActivityContent!.Sessions[0];
        session.Laps.Should().NotBeEmpty();
        session.Records.Should().HaveCount(2);
    }

    [Fact]
    public void Failed_Result_HasNullDocument()
    {
        var snapshot = SnapshotFactory.CreateEmpty();
        var options = new FitParseOptions { AllowPartialExtraction = false };

        var result = _builder.Build(snapshot, options);

        FitParseAssertions.AssertFailed(result, "fit.missing-file-id");
    }

    [Fact]
    public void Partial_Result_HasBothDocumentAndIssues()
    {
        // Create a snapshot with a decode fault but enough data to produce a document
        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Activity);
        snapshot.ActivityMesgs.Add((FitSdkMessageBuilder.CreateActivity(End), 1));
        snapshot.SessionMesgs.Add((FitSdkMessageBuilder.CreateSession(Start, End), 2));

        var faultSnapshot = new FitDecodeSnapshot
        {
            Mode = snapshot.Mode,
            SourceName = snapshot.SourceName,
            HadDecodeFault = true,
            DecodeFaultMessage = "Truncated at byte 4096",
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

        var result = _builder.Build(faultSnapshot, FitParseOptionsPresets.DefaultParse());

        result.Document.Should().NotBeNull("partial success should still produce a document");
        result.Issues.Should().NotBeEmpty("partial success should carry issues");
        result.Metadata.Status.Should().Be(FitParseStatus.PartiallySucceeded);
    }

    [Fact]
    public void Issues_List_IsNeverNull()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Activity);

        var result = _builder.Build(snapshot, FitParseOptionsPresets.DefaultParse());

        result.Issues.Should().NotBeNull("issues list must never be null");
    }

    [Fact]
    public void Metadata_IsAlwaysPopulated_EvenOnFailure()
    {
        var snapshot = SnapshotFactory.CreateEmpty();
        var options = new FitParseOptions { AllowPartialExtraction = false };

        var result = _builder.Build(snapshot, options);

        FitParseAssertions.AssertMetadataWindow(result.Metadata);
    }
}
