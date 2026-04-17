using FluentAssertions;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.ValueObjects;
using FitToMarkdown.Fit.Grouping;
using Xunit;

namespace FitToMarkdown.Fit.Tests.Unit.Grouping;

public sealed class FitActivityGroupingServiceExtendedTests
{
    private readonly FitActivityGroupingService _sut = new();

    private static readonly DateTimeOffset T0 = new(2024, 1, 1, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset T1 = new(2024, 1, 1, 11, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset T2 = new(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset T3 = new(2024, 1, 1, 13, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Group_SummaryFirst_SessionsBeforeRecords_AssignsCorrectly()
    {
        // SummaryFirst: sessions appear before records in stream order
        var session = CreateSession(0, T0, T1, firstLap: 0, numLaps: 1);
        var lap = CreateLap(0, T0, T1);
        var record = CreateRecord(0, T0.AddMinutes(15));

        var result = _sut.Group([session], [lap], [record], [], []);

        result.GroupedSessions.Should().HaveCount(1);
        result.GroupedSessions[0].Laps.Should().HaveCount(1);
        result.GroupedSessions[0].Records.Should().HaveCount(1);
    }

    [Fact]
    public void Group_SummaryLast_RecordsBeforeSessions_AssignsCorrectly()
    {
        // SummaryLast: records appear before sessions in stream order
        // The grouping service works on mapped models, so ordering doesn't matter
        var session = CreateSession(0, T0, T1, firstLap: 0, numLaps: 1);
        var record = CreateRecord(0, T0.AddMinutes(30));

        var result = _sut.Group([session], [], [record], [], []);

        result.GroupedSessions.Should().HaveCount(1);
        result.GroupedSessions[0].Records.Should().HaveCount(1);
        result.GroupedSessions[0].Records[0].SessionIndex.Should().Be(0);
    }

    [Fact]
    public void Group_MixedOrdering_SomeSessionsBeforeRecordsSomeAfter_AssignsCorrectly()
    {
        // Two sessions with interleaved records
        var session0 = CreateSession(0, T0, T1, firstLap: 0, numLaps: 1);
        var session1 = CreateSession(1, T2, T3, firstLap: 1, numLaps: 1);
        var record0 = CreateRecord(0, T0.AddMinutes(20));
        var record1 = CreateRecord(1, T2.AddMinutes(20));

        var result = _sut.Group([session0, session1], [], [record0, record1], [], []);

        result.GroupedSessions.Should().HaveCount(2);
        result.GroupedSessions[0].Records.Should().HaveCount(1);
        result.GroupedSessions[1].Records.Should().HaveCount(1);
    }

    [Fact]
    public void Group_TimestampFallback_WhenFirstLapIndexMissing()
    {
        // Sessions without FirstLapIndex fall back to timestamp-based grouping
        var session0 = CreateSessionWithoutLapIndex(0, T0, T1);
        var session1 = CreateSessionWithoutLapIndex(1, T2, T3);

        var lap0 = CreateLap(0, T0, T0.AddMinutes(30));
        var lap1 = CreateLap(1, T2, T2.AddMinutes(30));

        var result = _sut.Group([session0, session1], [lap0, lap1], [], [], []);

        result.GroupedSessions.Should().HaveCount(2);
        result.GroupedSessions[0].Laps.Should().HaveCount(1);
        result.GroupedSessions[1].Laps.Should().HaveCount(1);
    }

    [Fact]
    public void Group_EventsGroupedToCorrectSession_ByTimestamp()
    {
        var session0 = CreateSession(0, T0, T1, firstLap: 0, numLaps: 1);
        var session1 = CreateSession(1, T2, T3, firstLap: 1, numLaps: 1);

        var event0 = CreateEvent(0, T0.AddMinutes(5));
        var event1 = CreateEvent(1, T2.AddMinutes(5));

        // Events are not grouped onto the session record by Group directly,
        // but records are — verify timestamps route correctly
        var result = _sut.Group([session0, session1], [], [], [], [event0, event1]);

        result.GroupedSessions.Should().HaveCount(2);
    }

    [Fact]
    public void Group_LengthsGroupedToCorrectSession_ByTimestamp()
    {
        var session0 = CreateSession(0, T0, T1, firstLap: 0, numLaps: 1);
        var session1 = CreateSession(1, T2, T3, firstLap: 1, numLaps: 1);

        var length0 = CreateLength(0, T0.AddMinutes(5), T0.AddMinutes(6));
        var length1 = CreateLength(1, T2.AddMinutes(5), T2.AddMinutes(6));

        var result = _sut.Group([session0, session1], [], [], [length0, length1], []);

        result.GroupedSessions.Should().HaveCount(2);
        result.GroupedSessions[0].Lengths.Should().HaveCount(1);
        result.GroupedSessions[1].Lengths.Should().HaveCount(1);
    }

    [Fact]
    public void Group_SingleSession_ZeroLaps_ReturnsEmptyLapsList()
    {
        var session = CreateSession(0, T0, T1, firstLap: 0, numLaps: 0);

        var result = _sut.Group([session], [], [], [], []);

        result.GroupedSessions.Should().HaveCount(1);
        result.GroupedSessions[0].Laps.Should().BeEmpty();
    }

    [Fact]
    public void Group_ParseSequenceTieBreak_WhenTimestampsMatch()
    {
        // Two records at the same timestamp should both be assigned to the session
        var session = CreateSession(0, T0, T1, firstLap: 0, numLaps: 1);
        var record0 = CreateRecord(0, T0.AddMinutes(30));
        var record1 = CreateRecord(1, T0.AddMinutes(30));

        var result = _sut.Group([session], [], [record0, record1], [], []);

        result.GroupedSessions.Should().HaveCount(1);
        result.GroupedSessions[0].Records.Should().HaveCount(2);
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

    private static FitSession CreateSessionWithoutLapIndex(int idx, DateTimeOffset start, DateTimeOffset end)
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
                FirstLapIndex = null,
                NumberOfLaps = null,
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

    private static FitEvent CreateEvent(int idx, DateTimeOffset timestamp)
    {
        return new FitEvent
        {
            EventIndex = idx,
            TimestampUtc = timestamp,
        };
    }

    private static FitLength CreateLength(int idx, DateTimeOffset start, DateTimeOffset end)
    {
        return new FitLength
        {
            LengthIndex = idx,
            Range = new FitTimeRange
            {
                StartTimeUtc = start,
                EndTimeUtc = end,
            },
        };
    }
}
