using Dynastream.Fit;
using FluentAssertions;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Parsing;
using FitToMarkdown.Fit.Decoding;
using FitToMarkdown.Fit.Tests.Fixtures;
using FitToMarkdown.Fit.Tests.TestHelpers;
using FitToMarkdown.Fit.Validation;
using Xunit;

namespace FitToMarkdown.Fit.Tests.Unit.Validation;

public sealed class FitValidationPolicyExtendedTests
{
    private readonly FitValidationPolicy _sut = new();

    [Fact]
    public void Validate_NonActivityFile_WithIntegrityValidationEnabled_ReturnsFaultError()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Workout);
        var faultSnapshot = CloneWithFault(snapshot, "CRC mismatch");

        var options = FitParseOptionsPresets.DefaultParse();

        var result = _sut.Validate(faultSnapshot, options);

        result.IsValid.Should().BeFalse();
        result.Issues.Should().Contain(i =>
            i.Code == "fit.decode-fault" &&
            i.Severity == FitParseIssueSeverity.Error);
    }

    [Fact]
    public void Validate_NonActivityFile_WithIntegrityValidationDisabled_ReturnsWarningOnly()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Workout);
        var faultSnapshot = CloneWithFault(snapshot, "CRC mismatch");

        var options = FitParseOptionsPresets.WithoutNonActivityIntegrityValidation();

        var result = _sut.Validate(faultSnapshot, options);

        result.IsValid.Should().BeTrue("integrity validation is disabled for non-activity files");
        result.Issues.Should().Contain(i =>
            i.Code == "fit.decode-fault" &&
            i.Severity == FitParseIssueSeverity.Warning);
    }

    [Fact]
    public void Validate_ActivityWithTruncationFault_PartialExtractionEnabled_RemainsValid()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Activity);
        var faultSnapshot = CloneWithFault(snapshot, "Truncated at byte 2048");

        var options = FitParseOptionsPresets.DefaultParse();

        var result = _sut.Validate(faultSnapshot, options);

        // Activity faults are warnings, not errors — recovery can handle them
        result.IsValid.Should().BeTrue();
        result.Issues.Should().Contain(i =>
            i.Code == "fit.decode-fault" &&
            i.Severity == FitParseIssueSeverity.Warning);
    }

    [Fact]
    public void Validate_ActivityWithTruncationFault_PartialExtractionDisabled_StillPassesValidation()
    {
        // Validation itself doesn't check AllowPartialExtraction — that's the builder's concern.
        // Validation only reports issues; the builder decides whether to continue.
        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Activity);
        var faultSnapshot = CloneWithFault(snapshot, "Truncated at byte 2048");

        var options = FitParseOptionsPresets.WithoutTruncatedRecovery();

        var result = _sut.Validate(faultSnapshot, options);

        result.IsValid.Should().BeTrue("activity decode faults are warnings, not errors");
    }

    [Fact]
    public void Validate_MultipleFileIdMessages_ReturnsValid()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Activity);
        // Add a second FileIdMesg
        var secondFileId = FitSdkMessageBuilder.CreateFileId(fileType: Dynastream.Fit.File.Activity, manufacturer: 2);
        snapshot.FileIdMesgs.Add((secondFileId, 1));

        var options = FitParseOptionsPresets.DefaultParse();

        var result = _sut.Validate(snapshot, options);

        // Multiple FileId messages should not be a blocking error
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WorkoutFileWithMissingFileId_ReturnsError()
    {
        var snapshot = SnapshotFactory.CreateEmpty();
        // Add a workout message but no FileId
        var workout = new WorkoutMesg();
        snapshot.WorkoutMesgs.Add((workout, 0));

        var options = FitParseOptionsPresets.DefaultParse();

        var result = _sut.Validate(snapshot, options);

        result.IsValid.Should().BeFalse();
        result.Issues.Should().Contain(i =>
            i.Code == "fit.missing-file-id" &&
            i.Severity == FitParseIssueSeverity.Error);
    }

    private static FitDecodeSnapshot CloneWithFault(FitDecodeSnapshot source, string faultMessage)
    {
        return new FitDecodeSnapshot
        {
            Mode = source.Mode,
            SourceName = source.SourceName,
            HadDecodeFault = true,
            DecodeFaultMessage = faultMessage,
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
