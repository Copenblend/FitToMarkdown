using Dynastream.Fit;
using FluentAssertions;
using FitToMarkdown.Core.Parsing;
using FitToMarkdown.Fit.Decoding;
using FitToMarkdown.Fit.Mapping;
using FitToMarkdown.Fit.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Fit.Tests.Mapping;

public sealed class FitDocumentBuilderTests
{
    private readonly FitDocumentBuilder _sut = new();
    private readonly FitParseOptions _defaultOptions = new();

    [Fact]
    public void Build_MinimalSnapshot_ReturnsResult()
    {
        var snapshot = SnapshotFactory.CreateWithFileId();

        var result = _sut.Build(snapshot, _defaultOptions);

        result.Should().NotBeNull();
        result.Document.Should().NotBeNull();
        result.FatalError.Should().BeNull();
    }

    [Fact]
    public void Build_SnapshotWithFileId_PopulatesDocument()
    {
        var snapshot = SnapshotFactory.CreateWithFileId(
            manufacturer: 1,
            product: 1234,
            serialNumber: 123456789);

        var result = _sut.Build(snapshot, _defaultOptions);

        result.Document.Should().NotBeNull();
        result.Document!.FileId.Should().NotBeNull();
        result.Document.FileId!.ManufacturerId.Should().Be(1);
        result.Document.FileId.ProductId.Should().Be(1234);
        result.Document.FileId.SerialNumber.Should().Be(123456789u);
    }

    [Fact]
    public void Build_EmptySnapshot_WithAllowPartial_ReturnsPartial()
    {
        var snapshot = SnapshotFactory.CreateEmpty();
        var options = new FitParseOptions { AllowPartialExtraction = true };

        var result = _sut.Build(snapshot, options);

        // AllowPartialExtraction means we proceed even with validation errors
        result.Should().NotBeNull();
        result.Document.Should().NotBeNull();
        result.Issues.Should().Contain(i => i.Code == "fit.missing-file-id");
    }
}
