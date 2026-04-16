using FluentAssertions;
using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Summaries;
using FitToMarkdown.Markdown.Formatting;
using Xunit;

namespace FitToMarkdown.Markdown.Tests;

public sealed class SuggestedFileNameFactoryTests
{
    [Fact]
    public void Generate_should_produce_kebab_case_with_sport()
    {
        var factory = new SuggestedFileNameFactory();
        var doc = new FitMarkdownDocument
        {
            HeadingTimestampUtc = new DateTimeOffset(2024, 6, 15, 8, 30, 0, TimeSpan.Zero),
            Frontmatter = new FitFrontmatter
            {
                Sport = FitSport.Running,
                SubSport = FitSubSport.Street,
            },
        };

        var result = factory.Generate(doc);

        result.Should().EndWith(".md");
        result.Should().Contain("20240615");
        result.Should().Contain("running");
        result.Should().Contain("street");
    }

    [Fact]
    public void Generate_should_handle_unknown_timestamp()
    {
        var factory = new SuggestedFileNameFactory();
        var doc = new FitMarkdownDocument
        {
            Frontmatter = new FitFrontmatter
            {
                Sport = FitSport.Cycling,
            },
        };

        var result = factory.Generate(doc);

        result.Should().Contain("unknown-time");
        result.Should().Contain("cycling");
        result.Should().EndWith(".md");
    }

    [Fact]
    public void Generate_should_truncate_long_names()
    {
        var factory = new SuggestedFileNameFactory();
        var doc = new FitMarkdownDocument
        {
            HeadingTimestampUtc = new DateTimeOffset(2024, 6, 15, 8, 30, 0, TimeSpan.Zero),
            Frontmatter = new FitFrontmatter
            {
                Sport = FitSport.Running,
            },
        };

        var result = factory.Generate(doc);

        // Base should be at most 96 chars + ".md" = 99 chars
        result.Length.Should().BeLessOrEqualTo(100);
        result.Should().EndWith(".md");
    }

    [Fact]
    public void Generate_should_skip_generic_subsport()
    {
        var factory = new SuggestedFileNameFactory();
        var doc = new FitMarkdownDocument
        {
            HeadingTimestampUtc = new DateTimeOffset(2024, 6, 15, 8, 30, 0, TimeSpan.Zero),
            Frontmatter = new FitFrontmatter
            {
                Sport = FitSport.Running,
                SubSport = FitSubSport.Generic,
            },
        };

        var result = factory.Generate(doc);

        result.Should().NotContain("generic");
    }
}
