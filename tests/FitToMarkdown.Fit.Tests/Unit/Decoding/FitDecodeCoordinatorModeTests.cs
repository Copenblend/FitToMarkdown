using Dynastream.Fit;
using FluentAssertions;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Parsing;
using FitToMarkdown.Fit.Decoding;
using FitToMarkdown.Fit.Mapping;
using FitToMarkdown.Fit.Tests.Fixtures;
using FitToMarkdown.Fit.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Fit.Tests.Unit.Decoding;

public sealed class FitDecodeCoordinatorModeTests
{
    private readonly FitDocumentBuilder _builder = new();

    [Fact]
    public void MetadataOnly_Snapshot_Build_ProducesResult()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(
            fileType: Dynastream.Fit.File.Activity,
            mode: FitDecodeMode.MetadataOnly);

        var result = _builder.Build(snapshot, FitParseOptionsPresets.MetadataOnly());

        result.Should().NotBeNull();
        result.Document.Should().NotBeNull();
        result.Metadata.Should().NotBeNull();
    }

    [Fact]
    public void FullParse_Snapshot_WithActivityMessages_ProducesDocument()
    {
        var start = new DateTimeOffset(2024, 6, 15, 8, 0, 0, TimeSpan.Zero);
        var end = start.AddHours(1);

        var snapshot = SnapshotFactory.CreateWithFileId(
            fileType: Dynastream.Fit.File.Activity,
            mode: FitDecodeMode.FullParse);
        snapshot.ActivityMesgs.Add((FitSdkMessageBuilder.CreateActivity(end), 1));
        snapshot.SessionMesgs.Add((FitSdkMessageBuilder.CreateSession(start, end), 2));
        snapshot.LapMesgs.Add((FitSdkMessageBuilder.CreateLap(start, end), 3));
        snapshot.RecordMesgs.Add((FitSdkMessageBuilder.CreateRecord(start.AddMinutes(10), heartRate: 140), 4));

        var result = _builder.Build(snapshot, FitParseOptionsPresets.DefaultParse());

        result.Document.Should().NotBeNull();
        result.Document!.ActivityContent.Should().NotBeNull();
        result.Document.ActivityContent!.Sessions.Should().NotBeEmpty();
    }

    [Fact]
    public void FullParse_EmptySnapshot_FailsGracefully()
    {
        var snapshot = SnapshotFactory.CreateEmpty(FitDecodeMode.FullParse);
        var options = new FitParseOptions { AllowPartialExtraction = false };

        var result = _builder.Build(snapshot, options);

        result.Should().NotBeNull();
        result.FatalError.Should().NotBeNull();
        result.Document.Should().BeNull();
    }

    [Fact]
    public void MetadataOnly_Snapshot_PreservesDecodeMode_InMetadata()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(
            fileType: Dynastream.Fit.File.Activity,
            mode: FitDecodeMode.MetadataOnly);

        var result = _builder.Build(snapshot, FitParseOptionsPresets.MetadataOnly());

        result.Metadata.Should().NotBeNull();
        result.Metadata.Status.Should().Be(FitParseStatus.Succeeded);
    }
}
