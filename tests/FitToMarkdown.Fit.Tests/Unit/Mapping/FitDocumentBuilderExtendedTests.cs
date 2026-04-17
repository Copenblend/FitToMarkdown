using Dynastream.Fit;
using FluentAssertions;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Parsing;
using FitToMarkdown.Fit.Decoding;
using FitToMarkdown.Fit.Mapping;
using FitToMarkdown.Fit.Tests.Fixtures;
using FitToMarkdown.Fit.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Fit.Tests.Unit.Mapping;

public sealed class FitDocumentBuilderExtendedTests
{
    private readonly FitDocumentBuilder _sut = new();

    private static readonly DateTimeOffset Start = new(2024, 6, 15, 8, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset End = Start.AddHours(1);

    [Fact]
    public void Build_ActivitySnapshot_ProducesDocumentWithSessionsAndLaps()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Activity);
        snapshot.ActivityMesgs.Add((FitSdkMessageBuilder.CreateActivity(End), 1));
        snapshot.SessionMesgs.Add((FitSdkMessageBuilder.CreateSession(Start, End), 2));
        snapshot.LapMesgs.Add((FitSdkMessageBuilder.CreateLap(Start, End), 3));
        snapshot.RecordMesgs.Add((FitSdkMessageBuilder.CreateRecord(Start.AddMinutes(10), heartRate: 142), 4));
        snapshot.RecordMesgs.Add((FitSdkMessageBuilder.CreateRecord(Start.AddMinutes(30), heartRate: 155), 5));

        var result = _sut.Build(snapshot, FitParseOptionsPresets.DefaultParse());

        result.Document.Should().NotBeNull();
        result.Document!.ActivityContent.Should().NotBeNull();
        result.Document.ActivityContent!.Sessions.Should().HaveCount(1);
        result.Document.ActivityContent.Sessions[0].Laps.Should().NotBeEmpty();
        result.Document.ActivityContent.Sessions[0].Records.Should().HaveCount(2);
    }

    [Fact]
    public void Build_WorkoutSnapshot_ProducesWorkoutDocument()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Workout);
        var workout = new WorkoutMesg();
        workout.SetWktName("Interval Run");
        snapshot.WorkoutMesgs.Add((workout, 1));

        // Don't add empty WorkoutStepMesg — SDK throws NullReference on uninitialized byte[] fields
        var result = _sut.Build(snapshot, FitParseOptionsPresets.DefaultParse());

        result.Document.Should().NotBeNull();
        result.Document!.Workout.Should().NotBeNull();
    }

    [Fact]
    public void Build_CourseSnapshot_ProducesCourseDocument()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Course);
        var course = new CourseMesg();
        course.SetName("Morning Route");
        snapshot.CourseMesgs.Add((course, 1));

        // Don't add empty CoursePointMesg — SDK throws NullReference on uninitialized byte[] fields
        var result = _sut.Build(snapshot, FitParseOptionsPresets.DefaultParse());

        result.Document.Should().NotBeNull();
        result.Document!.Course.Should().NotBeNull();
    }

    [Fact]
    public void Build_SnapshotWithDeveloperFields_ResolvesDeveloperData()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Activity);
        snapshot.ActivityMesgs.Add((FitSdkMessageBuilder.CreateActivity(End), 1));
        snapshot.SessionMesgs.Add((FitSdkMessageBuilder.CreateSession(Start, End), 2));

        // Add developer data ID
        var devDataId = new DeveloperDataIdMesg();
        snapshot.DeveloperDataIdMesgs.Add((devDataId, 3));

        // Add field description
        var fieldDesc = new FieldDescriptionMesg();
        snapshot.FieldDescriptionMesgs.Add((fieldDesc, 4));

        var options = FitParseOptionsPresets.DefaultParse();
        var result = _sut.Build(snapshot, options);

        result.Document.Should().NotBeNull();
        result.Document!.DeveloperDataIds.Should().NotBeNull();
        result.Document.FieldDescriptions.Should().NotBeNull();
        result.Metadata.HasDeveloperFields.Should().BeTrue();
    }

    [Fact]
    public void Build_SnapshotWithHrvMessages_PreservesHrvInDocument()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Activity);
        snapshot.ActivityMesgs.Add((FitSdkMessageBuilder.CreateActivity(End), 1));
        snapshot.SessionMesgs.Add((FitSdkMessageBuilder.CreateSession(Start, End), 2));

        var hrvMesg = new HrvMesg();
        snapshot.HrvMesgs.Add((hrvMesg, 3));

        var result = _sut.Build(snapshot, FitParseOptionsPresets.DefaultParse());

        result.Document.Should().NotBeNull();
        result.Document!.ActivityContent.Should().NotBeNull();
        result.Document.ActivityContent!.HrvMessages.Should().NotBeNull();
    }

    [Fact]
    public void Build_ActivitySnapshot_WithNoMonitoring_OmitsMonitoringContent()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(fileType: Dynastream.Fit.File.Activity);
        snapshot.ActivityMesgs.Add((FitSdkMessageBuilder.CreateActivity(End), 1));
        snapshot.SessionMesgs.Add((FitSdkMessageBuilder.CreateSession(Start, End), 2));

        var result = _sut.Build(snapshot, FitParseOptionsPresets.DefaultParse());

        result.Document.Should().NotBeNull();
        // Activity files typically don't have monitoring content
        result.Document!.MonitoringContent.Should().BeNull();
    }
}
