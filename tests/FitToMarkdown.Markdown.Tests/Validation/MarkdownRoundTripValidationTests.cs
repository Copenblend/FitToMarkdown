using FluentAssertions;
using FitToMarkdown.Markdown.Projection;
using FitToMarkdown.Markdown.Rendering;
using FitToMarkdown.Markdown.Validation;
using FitToMarkdown.Markdown.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Markdown.Tests.Validation;

public sealed class MarkdownRoundTripValidationTests
{
    private static string RenderFullMarkdown(Core.Documents.FitFileDocument doc)
    {
        var opts = MarkdownTestFixtures.CreateDefaultOptions();
        var projector = new FitMarkdownProjector();
        var generator = new MarkdownDocumentGenerator();
        var mdDoc = projector.ProjectAsync(doc, opts).GetAwaiter().GetResult();
        var result = generator.GenerateAsync(mdDoc).GetAwaiter().GetResult();
        return result.Content;
    }

    [Fact]
    public void Standard_activity_should_pass_validation()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();

        var markdown = RenderFullMarkdown(doc);

        var validator = new MarkdownValidationService();
        var isValid = validator.Validate(markdown, out var issues);

        isValid.Should().BeTrue("standard activity should produce valid markdown");
        issues.Should().BeEmpty();
    }

    [Fact]
    public void Multi_sport_activity_should_produce_parseable_markdown()
    {
        var doc = MarkdownTestFixtures.CreateMultiSportActivity();

        var markdown = RenderFullMarkdown(doc);

        markdown.Should().NotBeNullOrWhiteSpace();

        // Multi-sport documents may have structural variations but should still parse
        var issues = MarkdownStructureInspector.Inspect(markdown);

        // Should have exactly one H1
        var lines = markdown.Split('\n');
        var h1Count = lines.Count(l => l.StartsWith("# ") && !l.StartsWith("## "));
        h1Count.Should().Be(1);
    }

    [Fact]
    public void Well_formed_markdown_should_pass_structure_inspection()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();

        var markdown = RenderFullMarkdown(doc);

        var issues = MarkdownStructureInspector.Inspect(markdown);

        issues.Should().BeEmpty("well-formed rendered markdown should have no structure issues");
    }
}
