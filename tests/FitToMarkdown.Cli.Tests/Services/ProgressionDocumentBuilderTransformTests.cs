using FitToMarkdown.Cli.Services;
using FluentAssertions;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Services;

public sealed class ProgressionDocumentBuilderTransformTests
{
    [Fact]
    public void TransformForProgression_strips_yaml_frontmatter()
    {
        var markdown = """
            ---
            file_type: Activity
            sport: Running
            ---
            # Running Activity

            2024-06-15 08:30:00 UTC

            ## Overview
            Some content
            """;
        var timestamp = new DateTimeOffset(2024, 6, 15, 8, 30, 0, TimeSpan.Zero);

        var result = ProgressionDocumentBuilder.TransformForProgression(markdown, timestamp);

        result.Should().NotContain("file_type: Activity");
        result.Should().NotContain("sport: Running");
    }

    [Fact]
    public void TransformForProgression_replaces_h1_with_h2_timestamp()
    {
        var markdown = """
            # Running Activity

            2024-06-15 08:30:00 UTC

            ## Overview
            Some content
            """;
        var timestamp = new DateTimeOffset(2024, 6, 15, 8, 30, 0, TimeSpan.Zero);

        var result = ProgressionDocumentBuilder.TransformForProgression(markdown, timestamp);

        result.Should().NotContain("# Running Activity");
        result.Should().StartWith("## 2024-06-15 08:30:00 UTC");
    }

    [Fact]
    public void TransformForProgression_downshifts_headings_by_one_level()
    {
        var markdown = """
            # Running Activity

            2024-06-15 08:30:00 UTC

            ## Overview
            Some content

            ## Session 1: Running
            Session details

            ### Laps
            Lap data
            """;
        var timestamp = new DateTimeOffset(2024, 6, 15, 8, 30, 0, TimeSpan.Zero);

        var result = ProgressionDocumentBuilder.TransformForProgression(markdown, timestamp);

        result.Should().Contain("### Overview");
        result.Should().Contain("### Session 1: Running");
        result.Should().Contain("#### Laps");
        result.Should().NotContain("\n## Overview");
        result.Should().NotContain("\n## Session 1");
    }

    [Fact]
    public void TransformForProgression_handles_full_activity_with_frontmatter()
    {
        var markdown = """
            ---
            file_type: Activity
            sport: Running
            sub_sport: Generic
            time_created: 2024-06-15 08:30:00 UTC
            serial_number: 12345
            ---
            # Running Activity

            2024-06-15 08:30:00 UTC

            ## Overview
            - **Total Distance:** 5.01 km

            ## Session 1: Running
            | Metric | Value |
            | --- | --- |
            | Duration | 00:32:35 |

            ### Laps
            | # | Duration |
            | --- | --- |
            | 1 | 00:06:31 |
            """;
        var timestamp = new DateTimeOffset(2024, 6, 15, 8, 30, 0, TimeSpan.Zero);

        var result = ProgressionDocumentBuilder.TransformForProgression(markdown, timestamp);

        result.Should().StartWith("## 2024-06-15 08:30:00 UTC");
        result.Should().NotContain("file_type:");
        result.Should().NotContain("# Running Activity");
        result.Should().Contain("### Overview");
        result.Should().Contain("### Session 1: Running");
        result.Should().Contain("#### Laps");
    }

    [Fact]
    public void TransformForProgression_handles_null_timestamp()
    {
        var markdown = """
            # Running Activity

            ## Overview
            Some content
            """;

        var result = ProgressionDocumentBuilder.TransformForProgression(markdown, null);

        result.Should().StartWith("## Unknown Time");
    }

    [Fact]
    public void TransformForProgression_preserves_non_heading_content()
    {
        var markdown = """
            # Running Activity

            2024-06-15 08:30:00 UTC

            ## Overview
            - **Total Distance:** 5.01 km
            - **Duration:** 00:32:35

            ```csv
            timestamp,elapsed_s,distance_m
            2024-06-15T08:30:00Z,0.0,0.00
            ```
            """;
        var timestamp = new DateTimeOffset(2024, 6, 15, 8, 30, 0, TimeSpan.Zero);

        var result = ProgressionDocumentBuilder.TransformForProgression(markdown, timestamp);

        result.Should().Contain("**Total Distance:** 5.01 km");
        result.Should().Contain("```csv");
        result.Should().Contain("2024-06-15T08:30:00Z,0.0,0.00");
    }
}
