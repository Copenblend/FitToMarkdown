using FluentAssertions;
using FitToMarkdown.Markdown.Projection;
using FitToMarkdown.Markdown.Rendering;
using FitToMarkdown.Markdown.Tests.Fixtures;
using FitToMarkdown.Markdown.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Markdown.Tests.Rendering;

public sealed class DeterministicMarkdownOutputTests
{
    private static Core.Abstractions.MarkdownDocumentResult Generate(Core.Documents.FitFileDocument doc)
    {
        var opts = MarkdownTestFixtures.CreateDefaultOptions();
        var projector = new FitMarkdownProjector();
        var generator = new MarkdownDocumentGenerator();
        var mdDoc = projector.ProjectAsync(doc, opts).GetAwaiter().GetResult();
        return generator.GenerateAsync(mdDoc).GetAwaiter().GetResult();
    }

    [Fact]
    public void Two_renders_of_same_input_should_be_byte_for_byte_identical()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();

        var first = Generate(doc);
        var second = Generate(doc);

        MarkdownArtifactAssertions.AssertDeterministic(first, second);
    }

    [Fact]
    public void Different_fixture_should_produce_different_output()
    {
        var running = Generate(MarkdownTestFixtures.CreateStandardRunningActivity());
        var multiSport = Generate(MarkdownTestFixtures.CreateMultiSportActivity());

        running.Content.Should().NotBe(multiSport.Content);
    }

    [Fact]
    public void Suggested_file_name_should_be_deterministic()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();

        var first = Generate(doc);
        var second = Generate(doc);

        first.SuggestedFileName.Should().Be(second.SuggestedFileName);
        first.SuggestedFileName.Should().EndWith(".md");
    }
}
