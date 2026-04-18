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
    public void Generate_should_produce_underscore_format_with_sport()
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

        result.Should().Be("20240615_083000_Running.md");
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

        result.Should().Be("unknown_time_Cycling.md");
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
    public void Generate_should_not_include_subsport_in_filename()
    {
        var factory = new SuggestedFileNameFactory();
        var doc = new FitMarkdownDocument
        {
            HeadingTimestampUtc = new DateTimeOffset(2024, 6, 15, 8, 30, 0, TimeSpan.Zero),
            Frontmatter = new FitFrontmatter
            {
                Sport = FitSport.Running,
                SubSport = FitSubSport.Trail,
            },
        };

        var result = factory.Generate(doc);

        result.Should().Be("20240615_083000_Running.md");
    }

    [Fact]
    public void Generate_should_split_pascal_case_sport_with_underscores()
    {
        var factory = new SuggestedFileNameFactory();
        var doc = new FitMarkdownDocument
        {
            HeadingTimestampUtc = new DateTimeOffset(2024, 6, 15, 8, 30, 0, TimeSpan.Zero),
            Frontmatter = new FitFrontmatter
            {
                Sport = FitSport.CrossCountrySkiing,
            },
        };

        var result = factory.Generate(doc);

        result.Should().Be("20240615_083000_Cross_Country_Skiing.md");
    }

    [Fact]
    public void Generate_should_use_activity_when_no_sport()
    {
        var factory = new SuggestedFileNameFactory();
        var doc = new FitMarkdownDocument
        {
            HeadingTimestampUtc = new DateTimeOffset(2024, 6, 15, 8, 30, 0, TimeSpan.Zero),
            Frontmatter = new FitFrontmatter(),
        };

        var result = factory.Generate(doc);

        result.Should().Be("20240615_083000_activity.md");
    }

    [Fact]
    public void Generate_should_handle_meditation_sport()
    {
        var factory = new SuggestedFileNameFactory();
        var doc = new FitMarkdownDocument
        {
            HeadingTimestampUtc = new DateTimeOffset(2026, 4, 15, 12, 22, 26, TimeSpan.Zero),
            Frontmatter = new FitFrontmatter
            {
                Sport = FitSport.Meditation,
            },
        };

        var result = factory.Generate(doc);

        result.Should().Be("20260415_122226_Meditation.md");
    }
}
