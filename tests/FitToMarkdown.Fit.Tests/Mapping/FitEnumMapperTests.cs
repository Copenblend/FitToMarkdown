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

    [Theory]
    [InlineData(49, FitSport.Baseball)]
    [InlineData(53, FitSport.Diving)]
    [InlineData(62, FitSport.Hiit)]
    [InlineData(64, FitSport.Racket)]
    [InlineData(65, FitSport.WheelchairPushWalk)]
    [InlineData(66, FitSport.WheelchairPushRun)]
    [InlineData(67, FitSport.Meditation)]
    [InlineData(69, FitSport.DiscGolf)]
    [InlineData(71, FitSport.Cricket)]
    [InlineData(72, FitSport.Rugby)]
    [InlineData(73, FitSport.Hockey)]
    [InlineData(74, FitSport.Lacrosse)]
    [InlineData(75, FitSport.Volleyball)]
    [InlineData(76, FitSport.WaterTubing)]
    [InlineData(77, FitSport.Wakesurfing)]
    [InlineData(80, FitSport.MixedMartialArts)]
    [InlineData(82, FitSport.Snorkeling)]
    [InlineData(83, FitSport.Dance)]
    [InlineData(84, FitSport.JumpRope)]
    public void MapSport_WithExtendedValue_ReturnsMappedSport(byte rawValue, FitSport expected)
    {
        // Cast raw byte to SDK Sport to simulate newer FIT SDK values
        var sdkSport = (Sport)rawValue;
        var result = FitEnumMapper.MapSport(sdkSport);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(53, FitSubSport.SingleGasDiving)]
    [InlineData(59, FitSubSport.Obstacle)]
    [InlineData(62, FitSubSport.Breathing)]
    [InlineData(67, FitSubSport.Ultra)]
    [InlineData(70, FitSubSport.Hiit)]
    [InlineData(84, FitSubSport.Pickleball)]
    [InlineData(97, FitSubSport.TableTennis)]
    [InlineData(110, FitSubSport.FlyCanopy)]
    [InlineData(119, FitSubSport.FlyIfr)]
    public void MapSubSport_WithExtendedValue_ReturnsMappedSubSport(byte rawValue, FitSubSport expected)
    {
        var sdkSubSport = (SubSport)rawValue;
        var result = FitEnumMapper.MapSubSport(sdkSubSport);
        result.Should().Be(expected);
    }
}
