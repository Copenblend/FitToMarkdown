using FluentAssertions;
using FitToMarkdown.Fit.Internal;
using Xunit;

namespace FitToMarkdown.Fit.Tests.Internal;

public sealed class FitCoordinateNormalizerTests
{
    private static readonly double SemicirclesToDegrees = 180.0 / Math.Pow(2, 31);

    [Fact]
    public void ToDegrees_WithSemicircles_ReturnsCorrectDegrees()
    {
        // Convert 47.0 degrees to semicircles, then convert back
        int semicircles = (int)(47.0 / SemicirclesToDegrees);
        var result = FitCoordinateNormalizer.ToDegrees(semicircles);
        result.Should().NotBeNull();
        result!.Value.Should().BeApproximately(47.0, 0.0001);
    }

    [Fact]
    public void ToDegrees_WithNull_ReturnsNull()
    {
        var result = FitCoordinateNormalizer.ToDegrees(null);
        result.Should().BeNull();
    }

    [Fact]
    public void ToGeoCoordinate_WithBothValues_ReturnsCoordinate()
    {
        int latSemicircles = (int)(47.0 / SemicirclesToDegrees);
        int lonSemicircles = (int)(8.5 / SemicirclesToDegrees);

        var result = FitCoordinateNormalizer.ToGeoCoordinate(latSemicircles, lonSemicircles);

        result.Should().NotBeNull();
        result!.LatitudeDegrees.Should().BeApproximately(47.0, 0.0001);
        result.LongitudeDegrees.Should().BeApproximately(8.5, 0.0001);
    }

    [Fact]
    public void ToGeoCoordinate_WithMissingLon_ReturnsNull()
    {
        int latSemicircles = (int)(47.0 / SemicirclesToDegrees);
        var result = FitCoordinateNormalizer.ToGeoCoordinate(latSemicircles, null);
        result.Should().BeNull();
    }

    [Fact]
    public void ToGeoBounds_WithAllValues_ReturnsBounds()
    {
        int necLat = (int)(48.0 / SemicirclesToDegrees);
        int necLon = (int)(9.0 / SemicirclesToDegrees);
        int swcLat = (int)(47.0 / SemicirclesToDegrees);
        int swcLon = (int)(8.0 / SemicirclesToDegrees);

        var result = FitCoordinateNormalizer.ToGeoBounds(necLat, necLon, swcLat, swcLon);

        result.Should().NotBeNull();
        result!.NorthEast.LatitudeDegrees.Should().BeApproximately(48.0, 0.0001);
        result.NorthEast.LongitudeDegrees.Should().BeApproximately(9.0, 0.0001);
        result.SouthWest.LatitudeDegrees.Should().BeApproximately(47.0, 0.0001);
        result.SouthWest.LongitudeDegrees.Should().BeApproximately(8.0, 0.0001);
    }

    [Fact]
    public void ToGeoBounds_WithAnyNull_ReturnsNull()
    {
        int necLat = (int)(48.0 / SemicirclesToDegrees);
        int necLon = (int)(9.0 / SemicirclesToDegrees);
        int swcLat = (int)(47.0 / SemicirclesToDegrees);

        var result = FitCoordinateNormalizer.ToGeoBounds(necLat, necLon, swcLat, null);
        result.Should().BeNull();
    }
}
