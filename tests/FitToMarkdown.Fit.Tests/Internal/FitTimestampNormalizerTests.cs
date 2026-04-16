using FluentAssertions;
using FitToMarkdown.Fit.Internal;
using Xunit;

namespace FitToMarkdown.Fit.Tests.Internal;

public sealed class FitTimestampNormalizerTests
{
    [Fact]
    public void ToUtcDateTimeOffset_WithFitDateTime_ReturnsUtcDateTimeOffset()
    {
        // 1,000,000,000 seconds after the Garmin epoch (1989-12-31T00:00:00 UTC)
        var fitDateTime = new Dynastream.Fit.DateTime(1000000000);
        var result = FitTimestampNormalizer.ToUtcDateTimeOffset(fitDateTime);

        result.Should().NotBeNull();
        result!.Value.Offset.Should().Be(TimeSpan.Zero); // UTC
        result.Value.Should().Be(new DateTimeOffset(fitDateTime.GetDateTime(), TimeSpan.Zero));
    }

    [Fact]
    public void ToUtcDateTimeOffset_WithNull_ReturnsNull()
    {
        var result = FitTimestampNormalizer.ToUtcDateTimeOffset(null);
        result.Should().BeNull();
    }

    [Fact]
    public void DeriveLocalTimeOffset_WithValidTimestamps_ReturnsOffset()
    {
        // UTC timestamp and a local timestamp 5 hours ahead
        var utcTimestamp = new Dynastream.Fit.DateTime(1000000000);
        var localTimestamp = new Dynastream.Fit.DateTime(1000000000 + (5 * 3600));

        var result = FitTimestampNormalizer.DeriveLocalTimeOffset(utcTimestamp, localTimestamp);

        result.Should().NotBeNull();
        result!.Value.Should().Be(TimeSpan.FromHours(5));
    }

    [Fact]
    public void DeriveLocalTimeOffset_WithNullUtc_ReturnsNull()
    {
        var localTimestamp = new Dynastream.Fit.DateTime(1000000000);
        var result = FitTimestampNormalizer.DeriveLocalTimeOffset(null, localTimestamp);
        result.Should().BeNull();
    }
}
