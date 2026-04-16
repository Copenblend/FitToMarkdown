using Dynastream.Fit;
using FluentAssertions;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Fit.Mapping;
using Xunit;

namespace FitToMarkdown.Fit.Tests.Mapping;

public sealed class FitEnumMapperTests
{
    [Fact]
    public void MapSport_WithRunning_ReturnsFitSportRunning()
    {
        var result = FitEnumMapper.MapSport(Sport.Running);
        result.Should().Be(FitSport.Running);
    }

    [Fact]
    public void MapSport_WithNull_ReturnsNull()
    {
        var result = FitEnumMapper.MapSport(null);
        result.Should().BeNull();
    }

    [Fact]
    public void MapSubSport_WithTreadmill_ReturnsTreadmill()
    {
        var result = FitEnumMapper.MapSubSport(SubSport.Treadmill);
        result.Should().Be(FitSubSport.Treadmill);
    }

    [Fact]
    public void MapEvent_WithTimer_ReturnsTimer()
    {
        var result = FitEnumMapper.MapEvent(Event.Timer);
        result.Should().Be(FitEventKind.Timer);
    }

    [Fact]
    public void MapEventType_WithStart_ReturnsStart()
    {
        var result = FitEnumMapper.MapEventType(EventType.Start);
        result.Should().Be(FitEventAction.Start);
    }

    [Fact]
    public void MapBatteryStatus_WithGoodValue_ReturnsGood()
    {
        // BatteryStatus.Good == 2 in the SDK
        var result = FitEnumMapper.MapBatteryStatus(2);
        result.Should().Be(FitBatteryStatus.Good);
    }

    [Fact]
    public void MapBatteryStatus_WithNull_ReturnsNull()
    {
        var result = FitEnumMapper.MapBatteryStatus(null);
        result.Should().BeNull();
    }

    [Fact]
    public void MapSessionTrigger_WithAutomatic_ReturnsAutomatic()
    {
        // SessionTrigger.ActivityEnd == 0
        var result = FitEnumMapper.MapSessionTrigger(SessionTrigger.ActivityEnd);
        result.Should().Be(FitSessionTrigger.ActivityEnd);
    }

    [Fact]
    public void MapLapTrigger_WithManual_ReturnsManual()
    {
        var result = FitEnumMapper.MapLapTrigger(LapTrigger.Manual);
        result.Should().Be(FitLapTrigger.Manual);
    }

    [Fact]
    public void MapSwimStroke_WithFreestyle_ReturnsFreestyle()
    {
        var result = FitEnumMapper.MapSwimStroke(SwimStroke.Freestyle);
        result.Should().Be(FitSwimStroke.Freestyle);
    }
}
