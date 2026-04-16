namespace FitToMarkdown.Fit.Mapping;

/// <summary>
/// Provides helper methods for converting FIT SDK typed accessor values to Core representations.
/// SDK typed accessors already apply scale/offset; these methods handle sentinel and null filtering.
/// </summary>
internal static class FitFieldValueConverter
{
    /// <summary>Returns null for null, NaN, or infinity; otherwise casts to double.</summary>
    public static double? ToNullableDouble(float? value)
    {
        if (value is null || float.IsNaN(value.Value) || float.IsInfinity(value.Value))
            return null;
        return value.Value;
    }

    /// <summary>Returns null for null, NaN, or infinity; otherwise returns the value.</summary>
    public static double? ToNullableDouble(double? value)
    {
        if (value is null || double.IsNaN(value.Value) || double.IsInfinity(value.Value))
            return null;
        return value.Value;
    }

    /// <summary>Converts seconds to <see cref="TimeSpan"/>; returns null if null or NaN.</summary>
    public static TimeSpan? ToTimeSpan(float? seconds)
    {
        if (seconds is null || float.IsNaN(seconds.Value) || float.IsInfinity(seconds.Value))
            return null;
        return TimeSpan.FromSeconds(seconds.Value);
    }

    /// <summary>Converts seconds to <see cref="TimeSpan"/>; returns null if null or NaN.</summary>
    public static TimeSpan? ToTimeSpan(double? seconds)
    {
        if (seconds is null || double.IsNaN(seconds.Value) || double.IsInfinity(seconds.Value))
            return null;
        return TimeSpan.FromSeconds(seconds.Value);
    }

    /// <summary>Returns null if the value is the FIT invalid sentinel (0xFFFF).</summary>
    public static ushort? ToNullableUshort(ushort? value)
    {
        return value == 0xFFFF ? null : value;
    }

    /// <summary>Returns null if the value is the FIT invalid sentinel (0xFFFFFFFF).</summary>
    public static uint? ToNullableUint(uint? value)
    {
        return value == 0xFFFFFFFF ? null : value;
    }

    /// <summary>Returns null if the value is the FIT invalid sentinel (0xFF).</summary>
    public static byte? ToNullableByte(byte? value)
    {
        return value == 0xFF ? null : value;
    }

    /// <summary>Converts a nullable enum to its string name, or null.</summary>
    public static string? EnumToString<T>(T? value) where T : struct, Enum
    {
        return value?.ToString();
    }

    /// <summary>Converts a FIT SDK byte-array string field to a .NET string, or null.</summary>
    public static string? ByteArrayToString(byte[]? bytes)
    {
        if (bytes is null || bytes.Length == 0)
            return null;
        var s = System.Text.Encoding.UTF8.GetString(bytes).TrimEnd('\0');
        return s.Length > 0 ? s : null;
    }
}
