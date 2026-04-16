using FluentAssertions;
using FitToMarkdown.Core.Summaries;
using FitToMarkdown.Core.ValueObjects;
using FitToMarkdown.Markdown.Formatting;
using Xunit;

namespace FitToMarkdown.Markdown.Tests;

public sealed class CsvSampleRendererTests
{
    [Fact]
    public void RenderCsvBlock_should_produce_fenced_csv()
    {
        var samples = new List<SampledTimeSeriesRow>
        {
            new()
            {
                TimestampUtc = new DateTimeOffset(2024, 6, 15, 8, 0, 0, TimeSpan.Zero),
                HeartRateBpm = 150,
                SpeedMetersPerSecond = 3.70,
            },
            new()
            {
                TimestampUtc = new DateTimeOffset(2024, 6, 15, 8, 1, 0, TimeSpan.Zero),
                HeartRateBpm = 155,
                SpeedMetersPerSecond = 3.75,
            },
        };

        var result = CsvSampleRenderer.RenderCsvBlock(samples);

        result.Should().StartWith("```csv\n");
        result.Should().EndWith("```\n");
        result.Should().Contain("timestamp_utc");
        result.Should().Contain("heart_rate_bpm");
        result.Should().Contain("speed_mps");
        result.Should().Contain("150");
    }

    [Fact]
    public void RenderCsvBlock_should_omit_all_empty_columns()
    {
        var samples = new List<SampledTimeSeriesRow>
        {
            new()
            {
                HeartRateBpm = 150,
            },
        };

        var result = CsvSampleRenderer.RenderCsvBlock(samples);

        result.Should().Contain("heart_rate_bpm");
        result.Should().NotContain("power_w");
        result.Should().NotContain("cadence_rpm");
        result.Should().NotContain("latitude_deg");
    }

    [Fact]
    public void RenderCsvBlock_should_return_empty_for_no_samples()
    {
        var result = CsvSampleRenderer.RenderCsvBlock([]);

        result.Should().BeEmpty();
    }

    [Fact]
    public void RenderCsvBlock_should_include_position_columns_when_present()
    {
        var samples = new List<SampledTimeSeriesRow>
        {
            new()
            {
                HeartRateBpm = 150,
                Position = new GeoCoordinate { LatitudeDegrees = 51.5, LongitudeDegrees = -0.12 },
            },
        };

        var result = CsvSampleRenderer.RenderCsvBlock(samples);

        result.Should().Contain("latitude_deg");
        result.Should().Contain("longitude_deg");
    }
}
