using FluentAssertions;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.ValueObjects;
using FitToMarkdown.Fit.Recovery;
using Xunit;

namespace FitToMarkdown.Fit.Tests.Unit.Recovery;

public sealed class FitSyntheticRecoveryServiceExtendedTests
{
    private readonly FitSyntheticRecoveryService _sut = new();

    private static readonly DateTimeOffset T0 = new(2024, 1, 1, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset T1 = new(2024, 1, 1, 11, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Recover_NoActivity_NoSessions_HasRecords_CreatesBothSynthetics()
    {
        var records = new List<FitRecord>
        {
            CreateRecord(T0),
            CreateRecord(T0.AddMinutes(30)),
            CreateRecord(T1),
        };

        var result = _sut.Recover(null, [], [], records, [], null);

        result.Activity.Should().NotBeNull();
        result.Activity!.Message.IsSynthetic.Should().BeTrue();
        result.UsedSyntheticActivity.Should().BeTrue();
        result.Sessions.Should().HaveCount(1);
        result.Sessions[0].Message.IsSynthetic.Should().BeTrue();
        result.UsedSyntheticSessions.Should().BeTrue();
    }

    [Fact]
    public void Recover_HasActivity_NoSessions_HasRecords_CreatesSyntheticSessionsOnly()
    {
        var activity = new FitActivity { TimestampUtc = T0 };
        var records = new List<FitRecord>
        {
            CreateRecord(T0),
            CreateRecord(T1),
        };

        var result = _sut.Recover(activity, [], [], records, [], null);

        result.Activity.Should().BeSameAs(activity);
        result.UsedSyntheticActivity.Should().BeFalse();
        result.Sessions.Should().HaveCount(1);
        result.Sessions[0].Message.IsSynthetic.Should().BeTrue();
        result.UsedSyntheticSessions.Should().BeTrue();
    }

    [Fact]
    public void Recover_HasActivity_HasSessions_NoSyntheticAnything()
    {
        var activity = new FitActivity { TimestampUtc = T0 };
        var sessions = new List<FitSession> { CreateSession() };
        var laps = new List<FitLap> { CreateLap() };
        var records = new List<FitRecord> { CreateRecord(T0) };

        var result = _sut.Recover(activity, sessions, laps, records, [], null);

        result.Activity.Should().BeSameAs(activity);
        result.UsedSyntheticActivity.Should().BeFalse();
        result.Sessions.Should().BeSameAs(sessions);
        result.UsedSyntheticSessions.Should().BeFalse();
        result.Laps.Should().BeSameAs(laps);
    }

    [Fact]
    public void Recover_NoRecords_NoRecoveryPossible()
    {
        List<FitRecord> records = [];

        var result = _sut.Recover(null, [], [], records, [], null);

        result.UsedSyntheticActivity.Should().BeFalse();
        result.UsedSyntheticSessions.Should().BeFalse();
        result.Issues.Should().BeEmpty();
    }

    [Fact]
    public void Recover_SyntheticActivity_PreservesTimestampFromRecords()
    {
        var records = new List<FitRecord>
        {
            CreateRecord(T0.AddMinutes(5)),
            CreateRecord(T0.AddMinutes(10)),
            CreateRecord(T1),
        };

        var result = _sut.Recover(null, [], [], records, [], null);

        result.Activity.Should().NotBeNull();
        result.Activity!.TimestampUtc.Should().Be(T0.AddMinutes(5));
    }

    [Fact]
    public void Recover_DoesNotCreateSyntheticDevicesWorkoutsCoursesOrMonitoring()
    {
        // Recovery only synthesizes Activity, Session, and Lap — nothing else
        var records = new List<FitRecord>
        {
            CreateRecord(T0),
            CreateRecord(T1),
        };

        var result = _sut.Recover(null, [], [], records, [], null);

        // The recovery result only has Activity, Sessions, Laps, Issues
        // No devices, workouts, courses, etc. are synthesized
        result.Activity.Should().NotBeNull();
        result.Sessions.Should().HaveCount(1);
        result.Laps.Should().HaveCount(1);
        result.Issues.Should().NotBeEmpty();
        // Verify no unexpected synthetic markers appear
        result.Sessions[0].Message.IsSynthetic.Should().BeTrue();
        result.Laps[0].Message.IsSynthetic.Should().BeTrue();
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
