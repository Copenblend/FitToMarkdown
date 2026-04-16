using FluentAssertions;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.ValueObjects;
using FitToMarkdown.Fit.Grouping;
using Xunit;

namespace FitToMarkdown.Fit.Tests.Grouping;

public sealed class FitActivityGroupingServiceTests
{
    private readonly FitActivityGroupingService _sut = new();

    private static readonly DateTimeOffset T0 = new(2024, 1, 1, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset T1 = new(2024, 1, 1, 11, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset T2 = new(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset T3 = new(2024, 1, 1, 13, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Group_SingleSession_AssignsAllLaps()
    {
        var session = CreateSession(0, T0, T1, firstLap: 0, numLaps: 2);
        var lap0 = CreateLap(0, T0, T0.AddMinutes(30));
        var lap1 = CreateLap(1, T0.AddMinutes(30), T1);

        var result = _sut.Group([session], [lap0, lap1], [], [], []);

        result.GroupedSessions.Should().HaveCount(1);
        result.GroupedSessions[0].Laps.Should().HaveCount(2);
    }

    [Fact]
    public void Group_SingleSession_AssignsAllRecords()
    {
        var session = CreateSession(0, T0, T1, firstLap: 0, numLaps: 1);
        var record0 = CreateRecord(0, T0.AddMinutes(10));
        var record1 = CreateRecord(1, T0.AddMinutes(20));

        var result = _sut.Group([session], [], [record0, record1], [], []);

        result.GroupedSessions.Should().HaveCount(1);
        result.GroupedSessions[0].Records.Should().HaveCount(2);
        result.GroupedSessions[0].Records[0].SessionIndex.Should().Be(0);
    }

    [Fact]
    public void Group_EmptySessions_ReturnsEmpty()
    {
        var result = _sut.Group([], [], [], [], []);
        result.GroupedSessions.Should().BeEmpty();
    }

    [Fact]
    public void Group_MultipleSessionsByIndex_AssignsLapsCorrectly()
    {
        var session0 = CreateSession(0, T0, T1, firstLap: 0, numLaps: 2);
        var session1 = CreateSession(1, T2, T3, firstLap: 2, numLaps: 1);

        var lap0 = CreateLap(0, T0, T0.AddMinutes(30));
        var lap1 = CreateLap(1, T0.AddMinutes(30), T1);
        var lap2 = CreateLap(2, T2, T3);

        var result = _sut.Group([session0, session1], [lap0, lap1, lap2], [], [], []);

        result.GroupedSessions.Should().HaveCount(2);
        result.GroupedSessions[0].Laps.Should().HaveCount(2);
        result.GroupedSessions[1].Laps.Should().HaveCount(1);
    }

    [Fact]
    public void Group_MultipleSessionsByTimestamp_AssignsRecordsCorrectly()
    {
        var session0 = CreateSession(0, T0, T1, firstLap: 0, numLaps: 1);
        var session1 = CreateSession(1, T2, T3, firstLap: 1, numLaps: 1);

        var record0 = CreateRecord(0, T0.AddMinutes(10));
        var record1 = CreateRecord(1, T2.AddMinutes(10));

        var result = _sut.Group([session0, session1], [], [record0, record1], [], []);

        result.GroupedSessions.Should().HaveCount(2);
        result.GroupedSessions[0].Records.Should().HaveCount(1);
        result.GroupedSessions[1].Records.Should().HaveCount(1);
    }

    private static FitSession CreateSession(int idx, DateTimeOffset start, DateTimeOffset end, int firstLap, int numLaps)
    {
        return new FitSession
        {
            SessionIndex = idx,
            Range = new FitTimeRange
            {
                StartTimeUtc = start,
                EndTimeUtc = end,
            },
            Metrics = new FitSummaryMetrics
            {
                FirstLapIndex = (ushort)firstLap,
                NumberOfLaps = (ushort)numLaps,
            },
        };
    }

    private static FitLap CreateLap(int idx, DateTimeOffset start, DateTimeOffset end)
    {
        return new FitLap
        {
            LapIndex = idx,
            Range = new FitTimeRange
            {
                StartTimeUtc = start,
                EndTimeUtc = end,
            },
        };
    }

    private static FitRecord CreateRecord(int idx, DateTimeOffset timestamp)
    {
        return new FitRecord
        {
            RecordIndex = idx,
            TimestampUtc = timestamp,
        };
    }
}
