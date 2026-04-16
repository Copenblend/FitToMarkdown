using FluentAssertions;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;
using FitToMarkdown.Markdown.Formatting;
using Xunit;

namespace FitToMarkdown.Markdown.Tests;

public sealed class MarkdownValueFormatterTests
{
    [Fact]
    public void FormatTimestampFrontmatter_should_produce_iso8601_utc()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 8, 30, 45, TimeSpan.FromHours(2));

        var result = MarkdownValueFormatter.FormatTimestampFrontmatter(dto);

        result.Should().Be("2024-06-15T06:30:45Z");
    }

    [Fact]
    public void FormatTimestampBody_should_include_utc_suffix()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 8, 30, 45, TimeSpan.Zero);

        var result = MarkdownValueFormatter.FormatTimestampBody(dto);

        result.Should().Be("2024-06-15 08:30:45 UTC");
    }

    [Fact]
    public void FormatDuration_should_use_mm_ss_for_short_durations()
    {
        var ts = TimeSpan.FromMinutes(28).Add(TimeSpan.FromSeconds(15));

        var result = MarkdownValueFormatter.FormatDuration(ts);

        result.Should().Be("28:15");
    }

    [Fact]
    public void FormatDuration_should_use_hh_mm_ss_for_hour_plus()
    {
        var ts = new TimeSpan(1, 23, 45);

        var result = MarkdownValueFormatter.FormatDuration(ts);

        result.Should().Be("01:23:45");
    }

    [Fact]
    public void FormatDuration_should_use_day_format_for_24h_plus()
    {
        var ts = new TimeSpan(1, 2, 30, 15);

        var result = MarkdownValueFormatter.FormatDuration(ts);

        result.Should().Be("1.02:30:15");
    }

    [Fact]
    public void FormatDistanceKilometersAndMiles_should_format_both_units()
    {
        var result = MarkdownValueFormatter.FormatDistanceKilometersAndMiles(10000);

        result.Should().Contain("10.00 km");
        result.Should().Contain("6.21 mi");
    }

    [Fact]
    public void FormatPace_should_return_min_sec_per_km()
    {
        // 3.70 m/s = 1000/3.70 = 270.27 s/km = 4:30/km
        var result = MarkdownValueFormatter.FormatPace(3.70);

        result.Should().Be("4:30");
    }

    [Fact]
    public void FormatPace_should_handle_zero_speed()
    {
        MarkdownValueFormatter.FormatPace(0).Should().Be("--:--");
        MarkdownValueFormatter.FormatPace(-1).Should().Be("--:--");
    }

    [Fact]
    public void FormatCadenceBody_should_double_for_running()
    {
        var result = MarkdownValueFormatter.FormatCadenceBody(88, isRunning: true);

        result.Should().Be("176");
    }

    [Fact]
    public void FormatCadenceBody_should_not_double_for_cycling()
    {
        var result = MarkdownValueFormatter.FormatCadenceBody(88, isRunning: false);

        result.Should().Be("88");
    }

    [Fact]
    public void FormatBattery_should_combine_status_and_voltage()
    {
        var battery = new BatterySnapshot
        {
            Status = FitBatteryStatus.Good,
            VoltageVolts = 3.95,
            ChargePercent = 82,
        };

        var result = MarkdownValueFormatter.FormatBattery(battery);

        result.Should().Contain("Good");
        result.Should().Contain("82%");
        result.Should().Contain("3.95V");
    }

    [Fact]
    public void FormatBattery_should_return_empty_for_null()
    {
        MarkdownValueFormatter.FormatBattery(null).Should().BeEmpty();
    }

    [Fact]
    public void FormatPacePer100m_should_format_correctly()
    {
        // 1.5 m/s → 100/1.5 = 66.67s = 1:06
        var result = MarkdownValueFormatter.FormatPacePer100m(1.5);

        result.Should().Be("1:06");
    }

    [Fact]
    public void FormatPacePer100m_should_handle_zero_speed()
    {
        MarkdownValueFormatter.FormatPacePer100m(0).Should().Be("--:--");
    }

    [Fact]
    public void FormatCoordinate_should_produce_six_decimal_places()
    {
        var result = MarkdownValueFormatter.FormatCoordinate(51.123456789);

        result.Should().Be("51.123457");
    }

    [Fact]
    public void FormatPercent_should_produce_one_decimal_place()
    {
        var result = MarkdownValueFormatter.FormatPercent(45.678);

        result.Should().Be("45.7");
    }
}
