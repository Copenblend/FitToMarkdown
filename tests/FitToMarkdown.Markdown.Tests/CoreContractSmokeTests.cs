using FluentAssertions;
using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Summaries;
using Xunit;

namespace FitToMarkdown.Markdown.Tests;

public sealed class CoreContractSmokeTests
{
    [Fact]
    public void FitMarkdownDocument_should_be_constructible_with_defaults()
    {
        var doc = new FitMarkdownDocument();

        doc.Source.Should().NotBeNull();
        doc.Title.Should().BeEmpty();
        doc.Frontmatter.Should().NotBeNull();
        doc.Overview.Should().NotBeNull();
        doc.SessionSections.Should().BeEmpty();
        doc.Devices.Should().BeEmpty();
        doc.Events.Should().BeEmpty();
        doc.SectionDecisions.Should().BeEmpty();
    }

    [Fact]
    public void MarkdownDocumentOptions_should_be_constructible()
    {
        var options = new MarkdownDocumentOptions();

        options.IncludeFrontmatter.Should().BeFalse();
        options.MaximumRecordSampleRows.Should().Be(0);
        options.ApproximateTokenBudget.Should().Be(0);
    }

    [Fact]
    public void MarkdownDocumentResult_should_be_constructible()
    {
        var result = new MarkdownDocumentResult();

        result.Content.Should().BeEmpty();
        result.SuggestedFileName.Should().BeEmpty();
        result.RenderedSections.Should().BeEmpty();
        result.ApproximateTokenCount.Should().BeNull();
    }

    [Fact]
    public void SectionRenderDecision_should_express_render_intent()
    {
        var decision = new SectionRenderDecision
        {
            Section = FitToMarkdown.Core.Enums.FitMarkdownSectionKey.Overview,
            Order = 1,
            ShouldRender = true
        };

        decision.ShouldRender.Should().BeTrue();
        decision.OmissionReason.Should().BeNull();
    }

    [Fact]
    public void IFitMarkdownProjector_interface_should_be_resolvable()
    {
        typeof(IFitMarkdownProjector).Should().NotBeNull();
        typeof(IFitMarkdownProjector).IsInterface.Should().BeTrue();
    }

    [Fact]
    public void IMarkdownDocumentGenerator_interface_should_be_resolvable()
    {
        typeof(IMarkdownDocumentGenerator).Should().NotBeNull();
        typeof(IMarkdownDocumentGenerator).IsInterface.Should().BeTrue();
    }
}
