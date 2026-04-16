using System.Globalization;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Markdown.Formatting;

internal static class MarkdownValueFormatter
{
    internal const double MetersPerKilometer = 1000.0;
    internal const double MetersToMiles = 0.000621371;

    internal static string FormatTimestampFrontmatter(DateTimeOffset dto)
    {
        return dto.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
    }

    internal static string FormatTimestampBody(DateTimeOffset dto)
    {
        return dto.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " UTC";
    }

    internal static string FormatTimestampEvent(DateTimeOffset dto)
    {
        return dto.ToUniversalTime().ToString("HH:mm:ss", CultureInfo.InvariantCulture);
    }

    internal static string FormatDuration(TimeSpan ts)
    {
        if (ts.TotalDays >= 1)
        {
            return ts.ToString(@"d\.hh\:mm\:ss", CultureInfo.InvariantCulture);
        }

        if (ts.TotalHours >= 1)
        {
            return ts.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);
        }

        return ts.ToString(@"mm\:ss", CultureInfo.InvariantCulture);
    }

    internal static string FormatDistanceKilometersAndMiles(double meters)
    {
        double km = meters / MetersPerKilometer;
        double miles = meters * MetersToMiles;
        return string.Create(CultureInfo.InvariantCulture, $"{km:F2} km / {miles:F2} mi");
    }

    internal static string FormatDistanceMeters(double meters)
    {
        return meters.ToString("F2", CultureInfo.InvariantCulture);
    }

    internal static string FormatDistanceKilometers(double meters)
    {
        return (meters / MetersPerKilometer).ToString("F2", CultureInfo.InvariantCulture);
    }

    internal static string FormatSpeed(double metersPerSecond)
    {
        return metersPerSecond.ToString("F2", CultureInfo.InvariantCulture);
    }

    internal static string FormatPace(double metersPerSecond)
    {
        if (metersPerSecond <= 0)
            return "--:--";

        double secondsPerKm = MetersPerKilometer / metersPerSecond;
        int minutes = (int)(secondsPerKm / 60);
        int seconds = (int)(secondsPerKm % 60);
        return string.Create(CultureInfo.InvariantCulture, $"{minutes}:{seconds:D2}");
    }

    internal static string FormatPacePer100m(double metersPerSecond)
    {
        if (metersPerSecond <= 0)
            return "--:--";

        double secondsPer100m = 100.0 / metersPerSecond;
        int minutes = (int)(secondsPer100m / 60);
        int seconds = (int)(secondsPer100m % 60);
        return string.Create(CultureInfo.InvariantCulture, $"{minutes}:{seconds:D2}");
    }

    internal static string FormatHeartRate(byte bpm)
    {
        return bpm.ToString(CultureInfo.InvariantCulture);
    }

    internal static string FormatCadenceBody(double rawCadenceRpm, bool isRunning)
    {
        double value = isRunning ? rawCadenceRpm * 2 : rawCadenceRpm;
        return ((int)value).ToString(CultureInfo.InvariantCulture);
    }

    internal static string FormatPower(ushort watts)
    {
        return watts.ToString(CultureInfo.InvariantCulture);
    }

    internal static string FormatAltitude(double meters)
    {
        return meters.ToString("F2", CultureInfo.InvariantCulture);
    }

    internal static string FormatTemperature(double celsius)
    {
        return celsius.ToString("F1", CultureInfo.InvariantCulture);
    }

    internal static string FormatCoordinate(double degrees)
    {
        return degrees.ToString("F6", CultureInfo.InvariantCulture);
    }

    internal static string FormatPercent(double value)
    {
        return value.ToString("F1", CultureInfo.InvariantCulture);
    }

    internal static string FormatCalories(ushort cal)
    {
        return cal.ToString(CultureInfo.InvariantCulture);
    }

    internal static string FormatBattery(BatterySnapshot? battery)
    {
        if (battery is null)
            return string.Empty;

        var parts = new List<string>();

        if (battery.Status is not null)
            parts.Add(battery.Status.Value.ToString());

        if (battery.ChargePercent is not null)
            parts.Add(string.Create(CultureInfo.InvariantCulture, $"{battery.ChargePercent.Value:F0}%"));

        if (battery.VoltageVolts is not null)
            parts.Add(string.Create(CultureInfo.InvariantCulture, $"{battery.VoltageVolts.Value:F2}V"));

        return string.Join(", ", parts);
    }

    internal static string FormatSoftwareVersion(double? version)
    {
        if (version is null)
            return string.Empty;

        return version.Value.ToString("F2", CultureInfo.InvariantCulture);
    }
}
