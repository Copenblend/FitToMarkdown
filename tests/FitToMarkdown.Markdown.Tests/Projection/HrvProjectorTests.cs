using FluentAssertions;
using FitToMarkdown.Markdown.Projection;
using FitToMarkdown.Markdown.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Markdown.Tests.Projection;

public sealed class HrvProjectorTests
{
    [Fact]
    public void Project_document_with_hrv_should_produce_summary()
    {
        var doc = MarkdownTestFixtures.CreateDocumentWithHrv();

        var result = HrvProjector.Project(doc);

        result.Should().NotBeNull();
        result!.SampleCount.Should().Be(6);
        result.AverageRrMilliseconds.Should().BeGreaterThan(0);
        result.MinimumRrMilliseconds.Should().BeGreaterThan(0);
        result.MaximumRrMilliseconds.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Project_document_without_hrv_should_return_null()
    {
        var doc = MarkdownTestFixtures.CreateMinimalActivityDocument();

        var result = HrvProjector.Project(doc);

        result.Should().BeNull();
    }

    [Fact]
    public void Project_hrv_with_multiple_messages_should_aggregate_stats()
    {
        var doc = MarkdownTestFixtures.CreateDocumentWithHrv();

        var result = HrvProjector.Project(doc);

        result.Should().NotBeNull();
        result!.SampleCount.Should().Be(6);
        result.SdnnMilliseconds.Should().BeGreaterThan(0);
        result.RmssdMilliseconds.Should().BeGreaterThan(0);
    }
}
