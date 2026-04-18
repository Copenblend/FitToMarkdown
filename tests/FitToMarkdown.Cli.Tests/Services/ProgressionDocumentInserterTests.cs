using FitToMarkdown.Cli.Services;
using FluentAssertions;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Services;

public sealed class ProgressionDocumentInserterTests
{
    private readonly ProgressionDocumentInserter _inserter = new();

    [Fact]
    public void InsertChronologically_into_empty_document_appends_activity()
    {
        var existing = """
            # Running Progression

            """;
        var newActivity = """
            ## 2024-06-15 08:30:00 UTC
            Some activity content
            """;

        var result = _inserter.InsertChronologically(existing, newActivity, new DateTimeOffset(2024, 6, 15, 8, 30, 0, TimeSpan.Zero));

        result.Should().Contain("Running Progression");
        result.Should().Contain("2024-06-15 08:30:00 UTC");
        result.Should().Contain("Some activity content");
    }

    [Fact]
    public void InsertChronologically_inserts_before_later_activity()
    {
        var existing = """
            # Running Progression

            ---

            ## 2024-06-20 10:00:00 UTC
            Later activity
            """;
        var newActivity = """
            ## 2024-06-15 08:30:00 UTC
            Earlier activity
            """;

        var result = _inserter.InsertChronologically(existing, newActivity, new DateTimeOffset(2024, 6, 15, 8, 30, 0, TimeSpan.Zero));

        var earlierIndex = result.IndexOf("Earlier activity", StringComparison.Ordinal);
        var laterIndex = result.IndexOf("Later activity", StringComparison.Ordinal);

        earlierIndex.Should().BeGreaterThan(-1);
        laterIndex.Should().BeGreaterThan(-1);
        earlierIndex.Should().BeLessThan(laterIndex);
    }

    [Fact]
    public void InsertChronologically_appends_after_earlier_activity()
    {
        var existing = """
            # Running Progression

            ---

            ## 2024-06-10 08:00:00 UTC
            Earlier activity
            """;
        var newActivity = """
            ## 2024-06-20 10:00:00 UTC
            Later activity
            """;

        var result = _inserter.InsertChronologically(existing, newActivity, new DateTimeOffset(2024, 6, 20, 10, 0, 0, TimeSpan.Zero));

        var earlierIndex = result.IndexOf("Earlier activity", StringComparison.Ordinal);
        var laterIndex = result.IndexOf("Later activity", StringComparison.Ordinal);

        earlierIndex.Should().BeGreaterThan(-1);
        laterIndex.Should().BeGreaterThan(-1);
        earlierIndex.Should().BeLessThan(laterIndex);
    }

    [Fact]
    public void InsertChronologically_inserts_between_two_activities()
    {
        var existing = """
            # Running Progression

            ---

            ## 2024-06-10 08:00:00 UTC
            First activity

            ---

            ## 2024-06-20 10:00:00 UTC
            Third activity
            """;
        var newActivity = """
            ## 2024-06-15 09:00:00 UTC
            Second activity
            """;

        var result = _inserter.InsertChronologically(existing, newActivity, new DateTimeOffset(2024, 6, 15, 9, 0, 0, TimeSpan.Zero));

        var firstIndex = result.IndexOf("First activity", StringComparison.Ordinal);
        var secondIndex = result.IndexOf("Second activity", StringComparison.Ordinal);
        var thirdIndex = result.IndexOf("Third activity", StringComparison.Ordinal);

        firstIndex.Should().BeGreaterThan(-1);
        secondIndex.Should().BeGreaterThan(-1);
        thirdIndex.Should().BeGreaterThan(-1);
        firstIndex.Should().BeLessThan(secondIndex);
        secondIndex.Should().BeLessThan(thirdIndex);
    }

    [Fact]
    public void InsertChronologically_uses_yaml_frontmatter_timestamp()
    {
        var existing = """
            # Running Progression

            ---

            ---
            time_created_utc: 2024-06-20T10:00:00+00:00
            ---
            Later activity
            """;
        var newActivity = """
            ---
            time_created_utc: 2024-06-15T08:00:00+00:00
            ---
            Earlier activity
            """;

        var result = _inserter.InsertChronologically(existing, newActivity, new DateTimeOffset(2024, 6, 15, 8, 0, 0, TimeSpan.Zero));

        var earlierIndex = result.IndexOf("Earlier activity", StringComparison.Ordinal);
        var laterIndex = result.IndexOf("Later activity", StringComparison.Ordinal);

        earlierIndex.Should().BeGreaterThan(-1);
        laterIndex.Should().BeGreaterThan(-1);
        earlierIndex.Should().BeLessThan(laterIndex);
    }

    [Fact]
    public void InsertChronologically_preserves_horizontal_rule_separators()
    {
        var existing = """
            # Running Progression

            ---

            First activity content
            """;
        var newActivity = "New activity content";

        var result = _inserter.InsertChronologically(existing, newActivity, DateTimeOffset.UtcNow);

        // Should have at least two separators (one for existing, one for new)
        var separatorCount = result.Split("---").Length - 1;
        separatorCount.Should().BeGreaterOrEqualTo(2);
    }
}
