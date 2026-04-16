using FluentAssertions;
using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.Parsing;
using FitToMarkdown.Core.ValueObjects;
using Xunit;

namespace FitToMarkdown.Fit.Tests;

public sealed class CoreContractSmokeTests
{
    [Fact]
    public void FitParseResult_should_be_constructible_with_defaults()
    {
        var result = new FitParseResult();

        result.Document.Should().BeNull();
        result.Metadata.Should().NotBeNull();
        result.Issues.Should().BeEmpty();
        result.FatalError.Should().BeNull();
    }

    [Fact]
    public void FitFileDocument_should_be_constructible_with_defaults()
    {
        var doc = new FitFileDocument();

        doc.FileId.Should().BeNull();
        doc.FileCreator.Should().BeNull();
        doc.ParseMetadata.Should().NotBeNull();
        doc.ActivityContent.Should().BeNull();
        doc.MonitoringContent.Should().BeNull();
        doc.DeviceInfos.Should().BeEmpty();
        doc.DeveloperDataIds.Should().BeEmpty();
        doc.FieldDescriptions.Should().BeEmpty();
    }

    [Fact]
    public void FitSession_should_carry_summary_metrics()
    {
        var session = new FitSession();

        session.Message.Should().NotBeNull();
        session.Metrics.Should().NotBeNull();
        session.Laps.Should().BeEmpty();
        session.Lengths.Should().BeEmpty();
        session.Records.Should().BeEmpty();
        session.DeveloperFields.Should().BeEmpty();
    }

    [Fact]
    public void FitRecord_should_expose_position_and_metrics()
    {
        var record = new FitRecord();

        record.Message.Should().NotBeNull();
        record.Position.Should().BeNull();
        record.HeartRateBpm.Should().BeNull();
        record.PowerWatts.Should().BeNull();
        record.DeveloperFields.Should().BeEmpty();
    }

    [Fact]
    public void FitMetadataInspectionResult_should_be_constructible()
    {
        var result = new FitMetadataInspectionResult();

        result.Summary.Should().BeNull();
        result.Metadata.Should().NotBeNull();
        result.Issues.Should().BeEmpty();
        result.FatalError.Should().BeNull();
    }

    [Fact]
    public void IFitFileParser_interface_should_be_resolvable()
    {
        typeof(IFitFileParser).Should().NotBeNull();
        typeof(IFitFileParser).IsInterface.Should().BeTrue();
    }

    [Fact]
    public void IFitMetadataInspector_interface_should_be_resolvable()
    {
        typeof(IFitMetadataInspector).Should().NotBeNull();
        typeof(IFitMetadataInspector).IsInterface.Should().BeTrue();
    }
}
