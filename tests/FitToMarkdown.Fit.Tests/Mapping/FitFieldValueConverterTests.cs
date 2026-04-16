using FluentAssertions;
using FitToMarkdown.Fit.Mapping;
using Xunit;

namespace FitToMarkdown.Fit.Tests.Mapping;

public sealed class FitFieldValueConverterTests
{
    [Fact]
    public void ToNullableDouble_WithFloat_ReturnsDouble()
    {
        float? value = 42.5f;
        var result = FitFieldValueConverter.ToNullableDouble(value);
        result.Should().BeApproximately(42.5, 0.001);
    }

    [Fact]
    public void ToNullableDouble_WithNull_ReturnsNull()
    {
        float? value = null;
        var result = FitFieldValueConverter.ToNullableDouble(value);
        result.Should().BeNull();
    }

    [Fact]
    public void ToNullableDouble_WithNaN_ReturnsNull()
    {
        float? value = float.NaN;
        var result = FitFieldValueConverter.ToNullableDouble(value);
        result.Should().BeNull();
    }

    [Fact]
    public void ToNullableDouble_WithInfinity_ReturnsNull()
    {
        float? value = float.PositiveInfinity;
        var result = FitFieldValueConverter.ToNullableDouble(value);
        result.Should().BeNull();
    }

    [Fact]
    public void ToTimeSpan_WithSeconds_ReturnsExpectedDuration()
    {
        float? seconds = 3661.5f;
        var result = FitFieldValueConverter.ToTimeSpan(seconds);
        result.Should().NotBeNull();
        result!.Value.TotalSeconds.Should().BeApproximately(3661.5, 0.01);
    }

    [Fact]
    public void ToTimeSpan_WithNull_ReturnsNull()
    {
        float? seconds = null;
        var result = FitFieldValueConverter.ToTimeSpan(seconds);
        result.Should().BeNull();
    }

    [Fact]
    public void ToNullableUshort_WithInvalid0xFFFF_ReturnsNull()
    {
        ushort? value = 0xFFFF;
        var result = FitFieldValueConverter.ToNullableUshort(value);
        result.Should().BeNull();
    }

    [Fact]
    public void ToNullableUshort_WithValidValue_ReturnsValue()
    {
        ushort? value = 42;
        var result = FitFieldValueConverter.ToNullableUshort(value);
        result.Should().Be(42);
    }

    [Fact]
    public void ToNullableUint_WithInvalid0xFFFFFFFF_ReturnsNull()
    {
        uint? value = 0xFFFFFFFF;
        var result = FitFieldValueConverter.ToNullableUint(value);
        result.Should().BeNull();
    }

    [Fact]
    public void ToNullableByte_WithInvalid0xFF_ReturnsNull()
    {
        byte? value = 0xFF;
        var result = FitFieldValueConverter.ToNullableByte(value);
        result.Should().BeNull();
    }

    [Fact]
    public void EnumToString_WithNull_ReturnsNull()
    {
        DayOfWeek? value = null;
        var result = FitFieldValueConverter.EnumToString(value);
        result.Should().BeNull();
    }

    [Fact]
    public void EnumToString_WithValue_ReturnsString()
    {
        DayOfWeek? value = DayOfWeek.Monday;
        var result = FitFieldValueConverter.EnumToString(value);
        result.Should().Be("Monday");
    }
}
