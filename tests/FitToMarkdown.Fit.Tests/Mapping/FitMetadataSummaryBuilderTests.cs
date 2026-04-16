using FluentAssertions;
using FitToMarkdown.Fit.Decoding;
using FitToMarkdown.Fit.Mapping;
using FitToMarkdown.Fit.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Fit.Tests.Mapping;

public sealed class FitMetadataSummaryBuilderTests
{
    private readonly FitMetadataSummaryBuilder _sut = new();

    [Fact]
    public void Build_EmptySnapshot_ReturnsMinimalResult()
    {
        var snapshot = SnapshotFactory.CreateEmpty();

        var result = _sut.Build(snapshot);

        result.Should().NotBeNull();
        result.Summary.Should().NotBeNull();
        result.Summary!.ManufacturerName.Should().BeNull();
        result.Summary.SerialNumber.Should().BeNull();
    }

    [Fact]
    public void Build_WithFileId_PopulatesSummary()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(
            manufacturer: 1,
            product: 1234,
            serialNumber: 123456789,
            sourceName: "my_activity.fit");

        var result = _sut.Build(snapshot);

        result.Should().NotBeNull();
        result.Summary.Should().NotBeNull();
        result.Summary!.ProductId.Should().Be(1234);
        result.Summary.SerialNumber.Should().Be(123456789u);
        result.Summary.FileName.Should().Be("my_activity.fit");
    }
}
