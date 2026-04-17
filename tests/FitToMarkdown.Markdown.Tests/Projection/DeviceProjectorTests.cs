using FluentAssertions;
using FitToMarkdown.Markdown.Projection;
using FitToMarkdown.Markdown.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Markdown.Tests.Projection;

public sealed class DeviceProjectorTests
{
    [Fact]
    public void Project_document_with_devices_should_produce_rows()
    {
        var doc = MarkdownTestFixtures.CreateDocumentWithDevices();

        var result = DeviceProjector.Project(doc);

        result.Should().NotBeEmpty();
        result[0].ManufacturerName.Should().Be("Garmin");
        result[0].ProductName.Should().Be("Forerunner 265");
    }

    [Fact]
    public void Project_document_without_devices_should_return_empty()
    {
        var doc = MarkdownTestFixtures.CreateMinimalActivityDocument();

        var result = DeviceProjector.Project(doc);

        result.Should().BeEmpty();
    }

    [Fact]
    public void Project_multiple_devices_should_return_correct_count()
    {
        var doc = MarkdownTestFixtures.CreateDocumentWithDevices();

        var result = DeviceProjector.Project(doc);

        result.Should().HaveCount(1);
        result[0].SerialNumber.Should().Be(345678901u);
    }
}
