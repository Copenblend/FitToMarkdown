using Dynastream.Fit;

namespace FitToMarkdown.Fit.Tests.Fixtures;

/// <summary>
/// Fluent builder for constructing Dynastream.Fit SDK messages with deterministic field values.
/// Reduces boilerplate across unit tests that need synthetic snapshot content.
/// </summary>
internal static class FitSdkMessageBuilder
{
    private static readonly System.DateTime GarminEpoch = new(1989, 12, 31, 0, 0, 0, DateTimeKind.Utc);

    internal static FileIdMesg CreateFileId(
        Dynastream.Fit.File fileType = Dynastream.Fit.File.Activity,
        ushort manufacturer = 1,
        ushort product = 1234,
        uint serialNumber = 123456789)
    {
        var mesg = new FileIdMesg();
        mesg.SetType(fileType);
        mesg.SetManufacturer(manufacturer);
        mesg.SetProduct(product);
        mesg.SetSerialNumber(serialNumber);
        mesg.SetTimeCreated(new Dynastream.Fit.DateTime(ToGarminTimestamp(new DateTimeOffset(2024, 6, 15, 8, 0, 0, TimeSpan.Zero))));
        return mesg;
    }

    internal static ActivityMesg CreateActivity(
        DateTimeOffset timestampUtc,
        byte numberOfSessions = 1)
    {
        var mesg = new ActivityMesg();
        mesg.SetTimestamp(new Dynastream.Fit.DateTime(ToGarminTimestamp(timestampUtc)));
        mesg.SetNumSessions(numberOfSessions);
        mesg.SetType(Activity.Manual);
        mesg.SetEvent(Event.Activity);
        mesg.SetEventType(EventType.Stop);
        return mesg;
    }

    internal static SessionMesg CreateSession(
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        Sport sport = Sport.Running,
        SubSport subSport = SubSport.Generic,
        ushort firstLapIndex = 0,
        ushort numLaps = 1)
    {
        var mesg = new SessionMesg();
        mesg.SetTimestamp(new Dynastream.Fit.DateTime(ToGarminTimestamp(endTime)));
        mesg.SetStartTime(new Dynastream.Fit.DateTime(ToGarminTimestamp(startTime)));
        mesg.SetSport(sport);
        mesg.SetSubSport(subSport);
        mesg.SetFirstLapIndex(firstLapIndex);
        mesg.SetNumLaps(numLaps);
        mesg.SetTotalElapsedTime((float)(endTime - startTime).TotalSeconds);
        mesg.SetTotalTimerTime((float)(endTime - startTime).TotalSeconds);
        return mesg;
    }

    internal static LapMesg CreateLap(
        DateTimeOffset startTime,
        DateTimeOffset endTime)
    {
        var mesg = new LapMesg();
        mesg.SetTimestamp(new Dynastream.Fit.DateTime(ToGarminTimestamp(endTime)));
        mesg.SetStartTime(new Dynastream.Fit.DateTime(ToGarminTimestamp(startTime)));
        mesg.SetTotalElapsedTime((float)(endTime - startTime).TotalSeconds);
        mesg.SetTotalTimerTime((float)(endTime - startTime).TotalSeconds);
        return mesg;
    }

    internal static RecordMesg CreateRecord(
        DateTimeOffset timestampUtc,
        int? heartRate = null,
        float? enhancedSpeed = null,
        float? enhancedAltitude = null,
        byte? cadence = null)
    {
        var mesg = new RecordMesg();
        mesg.SetTimestamp(new Dynastream.Fit.DateTime(ToGarminTimestamp(timestampUtc)));
        if (heartRate.HasValue) mesg.SetHeartRate((byte)heartRate.Value);
        if (enhancedSpeed.HasValue) mesg.SetEnhancedSpeed(enhancedSpeed.Value);
        if (enhancedAltitude.HasValue) mesg.SetEnhancedAltitude(enhancedAltitude.Value);
        if (cadence.HasValue) mesg.SetCadence(cadence.Value);
        return mesg;
    }

    internal static EventMesg CreateEvent(
        DateTimeOffset timestampUtc,
        Event eventType = Event.Timer,
        EventType eventAction = EventType.Start)
    {
        var mesg = new EventMesg();
        mesg.SetTimestamp(new Dynastream.Fit.DateTime(ToGarminTimestamp(timestampUtc)));
        mesg.SetEvent(eventType);
        mesg.SetEventType(eventAction);
        return mesg;
    }

    internal static DeviceInfoMesg CreateDeviceInfo(
        ushort manufacturer = 1,
        ushort product = 1234,
        uint serialNumber = 123456789,
        byte deviceIndex = 0)
    {
        var mesg = new DeviceInfoMesg();
        mesg.SetManufacturer(manufacturer);
        mesg.SetProduct(product);
        mesg.SetSerialNumber(serialNumber);
        mesg.SetDeviceIndex(deviceIndex);
        return mesg;
    }

    private static uint ToGarminTimestamp(DateTimeOffset timestamp)
    {
        return (uint)(timestamp.UtcDateTime - GarminEpoch).TotalSeconds;
    }
}
