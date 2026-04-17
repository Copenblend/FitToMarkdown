using Dynastream.Fit;
using FluentAssertions;
using FitToMarkdown.Fit.Decoding;
using FitToMarkdown.Fit.Mapping;
using FitToMarkdown.Fit.Tests.Fixtures;
using FitToMarkdown.Fit.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Fit.Tests.Unit.Mapping;

public sealed class FitMetadataSummaryBuilderExtendedTests
{
    private readonly FitMetadataSummaryBuilder _sut = new();

    private static readonly DateTimeOffset Start = new(2024, 6, 15, 8, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset End = Start.AddHours(1);

    [Fact]
    public void Build_ActivitySnapshot_ContainsFileTypeAndProduct()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(
            fileType: Dynastream.Fit.File.Activity,
            manufacturer: 1,
            product: 3890,
            serialNumber: 999888777,
            sourceName: "run_activity.fit");

        snapshot.SessionMesgs.Add((FitSdkMessageBuilder.CreateSession(Start, End, Sport.Running), 1));

        var result = _sut.Build(snapshot);

        result.Should().NotBeNull();
        result.Summary.Should().NotBeNull();
        result.Summary!.FileType.Should().Be("Activity");
        result.Summary.ProductId.Should().Be(3890);
        result.Summary.Sport.Should().Be("Running");
        result.Summary.FileName.Should().Be("run_activity.fit");
    }

    [Fact]
    public void Build_WorkoutSnapshot_ContainsWorkoutFileType()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(
            fileType: Dynastream.Fit.File.Workout,
            sourceName: "interval_workout.fit");

        var result = _sut.Build(snapshot);

        result.Should().NotBeNull();
        result.Summary.Should().NotBeNull();
        result.Summary!.FileType.Should().Be("Workout");
        result.Summary.FileName.Should().Be("interval_workout.fit");
    }

    [Fact]
    public void Build_EmptySnapshot_ReflectsMinimalState()
    {
        var snapshot = SnapshotFactory.CreateEmpty();

        var result = _sut.Build(snapshot);

        result.Should().NotBeNull();
        result.Summary.Should().NotBeNull();
        result.Summary!.FileType.Should().BeNull();
        result.Summary.ManufacturerName.Should().BeNull();
        result.Summary.ProductId.Should().BeNull();
        result.Summary.SerialNumber.Should().BeNull();
        result.Summary.Sport.Should().BeNull();
    }

    [Fact]
    public void Build_SnapshotWithMultipleSessions_ReflectsFirstSessionSport()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(
            fileType: Dynastream.Fit.File.Activity);

        var midpoint = Start.AddMinutes(30);
        snapshot.SessionMesgs.Add((FitSdkMessageBuilder.CreateSession(Start, midpoint, Sport.Running), 1));
        snapshot.SessionMesgs.Add((FitSdkMessageBuilder.CreateSession(midpoint, End, Sport.Cycling), 2));

        var result = _sut.Build(snapshot);

        result.Should().NotBeNull();
        result.Summary.Should().NotBeNull();
        // Summary builder uses the first session
        result.Summary!.Sport.Should().Be("Running");
    }
}
