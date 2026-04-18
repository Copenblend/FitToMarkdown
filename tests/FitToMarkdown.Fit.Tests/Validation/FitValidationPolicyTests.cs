using FluentAssertions;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Parsing;
using FitToMarkdown.Fit.Decoding;
using FitToMarkdown.Fit.Tests.TestHelpers;
using FitToMarkdown.Fit.Validation;
using Xunit;

namespace FitToMarkdown.Fit.Tests.Validation;

public sealed class FitValidationPolicyTests
{
    private readonly FitValidationPolicy _sut = new();
    private readonly FitParseOptions _defaultOptions = new();

    [Fact]
    public void Validate_EmptySnapshot_ReturnsMissingFileIdError()
    {
        var snapshot = SnapshotFactory.CreateEmpty();

        var result = _sut.Validate(snapshot, _defaultOptions);

        result.IsValid.Should().BeFalse();
        result.Issues.Should().Contain(i =>
            i.Code == "fit.missing-file-id" &&
            i.Severity == FitParseIssueSeverity.Error);
    }

    [Fact]
    public void Validate_WithFileId_ReturnsValid()
    {
        var snapshot = SnapshotFactory.CreateWithFileId();

        var result = _sut.Validate(snapshot, _defaultOptions);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithDecodeFault_ReturnsWarning()
    {
        var snapshot = SnapshotFactory.CreateWithFileId();
        var faultSnapshot = new FitDecodeSnapshot
        {
            Mode = snapshot.Mode,
            SourceName = snapshot.SourceName,
            HadDecodeFault = true,
            DecodeFaultMessage = "Truncated at byte 1024",
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

        var result = _sut.Validate(faultSnapshot, _defaultOptions);

        result.IsValid.Should().BeTrue(); // Warnings don't block
        result.Issues.Should().Contain(i =>
            i.Code == "fit.decode-fault" &&
            i.Severity == FitParseIssueSeverity.Warning);
    }
}
