using FluentAssertions;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.ValueObjects;
using FitToMarkdown.Fit.Recovery;
using Xunit;

namespace FitToMarkdown.Fit.Tests.Recovery;

public sealed class FitSyntheticRecoveryServiceTests
{
    private readonly FitSyntheticRecoveryService _sut = new();

    private static readonly DateTimeOffset T0 = new(2024, 1, 1, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset T1 = new(2024, 1, 1, 11, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Recover_WithExistingActivity_ReturnsOriginal()
    {
        var activity = new FitActivity { TimestampUtc = T0 };
        var sessions = new List<FitSession> { CreateSession() };
        var laps = new List<FitLap> { CreateLap() };
        var records = new List<FitRecord> { CreateRecord(T0), CreateRecord(T1) };

        var result = _sut.Recover(activity, sessions, laps, records, [], null);

        result.Activity.Should().BeSameAs(activity);
        result.UsedSyntheticActivity.Should().BeFalse();
    }

    [Fact]
    public void Recover_WithMissingActivity_CreatesSynthetic()
    {
        var sessions = new List<FitSession> { CreateSession() };
        var laps = new List<FitLap> { CreateLap() };
        var records = new List<FitRecord> { CreateRecord(T0), CreateRecord(T1) };

        var result = _sut.Recover(null, sessions, laps, records, [], null);

        result.Activity.Should().NotBeNull();
        result.Activity!.Message.IsSynthetic.Should().BeTrue();
        result.UsedSyntheticActivity.Should().BeTrue();
        result.Issues.Should().Contain(i => i.Code == "fit.synthetic-activity");
    }

    [Fact]
    public void Recover_WithMissingSessions_CreatesSyntheticSession()
    {
        var activity = new FitActivity { TimestampUtc = T0 };
        var records = new List<FitRecord> { CreateRecord(T0), CreateRecord(T1) };

        var result = _sut.Recover(activity, [], [], records, [], null);

        result.Sessions.Should().HaveCount(1);
        result.Sessions[0].Message.IsSynthetic.Should().BeTrue();
        result.UsedSyntheticSessions.Should().BeTrue();
        result.Issues.Should().Contain(i => i.Code == "fit.synthetic-session");
    }

    [Fact]
    public void Recover_WithMissingLaps_CreatesSyntheticLap()
    {
        var activity = new FitActivity { TimestampUtc = T0 };
        var sessions = new List<FitSession> { CreateSession() };
        var records = new List<FitRecord> { CreateRecord(T0), CreateRecord(T1) };

        var result = _sut.Recover(activity, sessions, [], records, [], null);

        result.Laps.Should().HaveCount(1);
        result.Laps[0].Message.IsSynthetic.Should().BeTrue();
        result.Issues.Should().Contain(i => i.Code == "fit.synthetic-lap");
    }

    [Fact]
    public void Recover_WithNoRecords_ReturnsOriginal()
    {
        var activity = new FitActivity { TimestampUtc = T0 };
        List<FitSession> sessions = [];
        List<FitLap> laps = [];
        List<FitRecord> records = [];

        var result = _sut.Recover(activity, sessions, laps, records, [], null);

        result.Activity.Should().BeSameAs(activity);
        result.Sessions.Should().BeSameAs(sessions);
        result.Laps.Should().BeSameAs(laps);
        result.Issues.Should().BeEmpty();
    }

    private static FitSession CreateSession()
    {
        return new FitSession
        {
            SessionIndex = 0,
            Range = new FitTimeRange
            {
                StartTimeUtc = T0,
                EndTimeUtc = T1,
            },
            Metrics = new FitSummaryMetrics { FirstLapIndex = 0, NumberOfLaps = 1 },
        };
    }

    private static FitLap CreateLap()
    {
        return new FitLap
        {
            LapIndex = 0,
            Range = new FitTimeRange
            {
                StartTimeUtc = T0,
                EndTimeUtc = T1,
            },
        };
    }

    private static FitRecord CreateRecord(DateTimeOffset timestamp)
    {
        return new FitRecord
        {
            TimestampUtc = timestamp,
        };
    }
}
