using FluentAssertions;
using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.Parsing;
using FitToMarkdown.Core.Summaries;
using Xunit;

namespace FitToMarkdown.Cli.Tests;

public sealed class CoreContractSmokeTests
{
    [Fact]
    public void FitFileInfoSummary_should_be_constructible()
    {
        var summary = new FitFileInfoSummary();

        summary.InputFilePath.Should().BeEmpty();
        summary.FileName.Should().BeEmpty();
        summary.Devices.Should().BeEmpty();
        summary.StartTimeUtc.Should().BeNull();
    }

    [Fact]
    public void FitMetadataInspectionResult_should_be_accessible_from_cli()
    {
        var result = new FitMetadataInspectionResult();

        result.Summary.Should().BeNull();
        result.Metadata.Should().NotBeNull();
        result.Issues.Should().BeEmpty();
    }

    [Fact]
    public void MarkdownDocumentResult_should_be_accessible_from_cli()
    {
        var result = new MarkdownDocumentResult();

        result.Content.Should().BeEmpty();
        result.SuggestedFileName.Should().BeEmpty();
        result.RenderedSections.Should().BeEmpty();
    }

    [Fact]
    public void Full_pipeline_interfaces_should_be_resolvable_from_cli()
    {
        typeof(IFitFileParser).IsInterface.Should().BeTrue();
        typeof(IFitMetadataInspector).IsInterface.Should().BeTrue();
        typeof(IFitMarkdownProjector).IsInterface.Should().BeTrue();
        typeof(IMarkdownDocumentGenerator).IsInterface.Should().BeTrue();
    }

    [Fact]
    public void FitMarkdownDocument_should_be_accessible_from_cli()
    {
        var doc = new FitMarkdownDocument();

        doc.Source.Should().NotBeNull();
        doc.Frontmatter.Should().NotBeNull();
        doc.Overview.Should().NotBeNull();
        doc.SessionSections.Should().BeEmpty();
    }

    [Fact]
    public void SessionSection_and_summary_types_should_be_accessible()
    {
        var section = new SessionSection();

        section.Session.Should().NotBeNull();
        section.LapRows.Should().BeEmpty();
        section.LengthRows.Should().BeEmpty();
        section.RecordSamples.Should().BeEmpty();
        section.SectionDecisions.Should().BeEmpty();
    }
}
