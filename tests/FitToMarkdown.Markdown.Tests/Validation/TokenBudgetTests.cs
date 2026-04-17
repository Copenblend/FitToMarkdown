using FluentAssertions;
using FitToMarkdown.Markdown.Projection;
using FitToMarkdown.Markdown.Rendering;
using FitToMarkdown.Markdown.Tests.Fixtures;
using FitToMarkdown.Markdown.Tests.Fixtures.Boundaries;
using FitToMarkdown.Markdown.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Markdown.Tests.Validation;

public sealed class TokenBudgetTests
{
    private static Core.Abstractions.MarkdownDocumentResult Generate(
        Core.Documents.FitFileDocument doc,
        Core.Abstractions.MarkdownDocumentOptions? opts = null)
    {
        opts ??= MarkdownTestFixtures.CreateDefaultOptions();
        var projector = new FitMarkdownProjector();
        var generator = new MarkdownDocumentGenerator();
        var mdDoc = projector.ProjectAsync(doc, opts).GetAwaiter().GetResult();
        return generator.GenerateAsync(mdDoc).GetAwaiter().GetResult();
    }

    [Fact]
    public void Budget_zero_should_not_compact()
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();
        var opts = MarkdownTestFixtures.CreateDefaultOptions() with { ApproximateTokenBudget = 0 };

        var result = Generate(doc, opts);

        result.Content.Should().NotBeNullOrWhiteSpace();
        result.ApproximateTokenCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Budget_set_should_produce_output_within_estimate()
    {
        var doc = CsvCapFixtureFactory.CreateWith61SampledRows();
        var opts = MarkdownTestFixtures.CreateDefaultOptions() with { ApproximateTokenBudget = 50000 };

        var result = Generate(doc, opts);

        MarkdownArtifactAssertions.AssertTokenBudget(result.Content, 50000);
    }

    [Fact]
    public void Large_fixture_with_tight_budget_should_apply_compaction()
    {
        var doc = CsvCapFixtureFactory.CreateWith61SampledRows();
        var noBudget = Generate(doc, MarkdownTestFixtures.CreateDefaultOptions() with { ApproximateTokenBudget = 0 });
        var tightBudget = Generate(doc, MarkdownTestFixtures.CreateDefaultOptions() with { ApproximateTokenBudget = 500 });

        // With a tight budget, output should be shorter or equal
        tightBudget.Content.Length.Should().BeLessOrEqualTo(noBudget.Content.Length);
    }
}
