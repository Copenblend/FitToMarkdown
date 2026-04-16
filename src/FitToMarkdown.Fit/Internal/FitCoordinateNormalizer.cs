using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Fit.Internal;

/// <summary>
/// Converts FIT semicircle coordinates to decimal degrees and Core value objects.
/// </summary>
internal static class FitCoordinateNormalizer
{
    /// <summary>Conversion factor from semicircles to decimal degrees.</summary>
    private static readonly double SemicirclesToDegrees = 180.0 / Math.Pow(2, 31);

    /// <summary>
    /// Converts a semicircle value to decimal degrees.
    /// </summary>
    /// <param name="semicircles">The coordinate in semicircles, or <c>null</c>.</param>
    /// <returns>The coordinate in decimal degrees, or <c>null</c> if input is <c>null</c>.</returns>
    public static double? ToDegrees(int? semicircles)
    {
        if (semicircles is null)
        {
            return null;
        }

        return semicircles.Value * SemicirclesToDegrees;
    }

    /// <summary>
    /// Converts latitude and longitude semicircle values to a <see cref="GeoCoordinate"/>.
    /// </summary>
    /// <param name="latSemicircles">Latitude in semicircles, or <c>null</c>.</param>
    /// <param name="lonSemicircles">Longitude in semicircles, or <c>null</c>.</param>
    /// <returns>A <see cref="GeoCoordinate"/>, or <c>null</c> if either input is <c>null</c>.</returns>
    public static GeoCoordinate? ToGeoCoordinate(int? latSemicircles, int? lonSemicircles)
    {
        if (latSemicircles is null || lonSemicircles is null)
        {
            return null;
        }

        return new GeoCoordinate
        {
            LatitudeDegrees = latSemicircles.Value * SemicirclesToDegrees,
            LongitudeDegrees = lonSemicircles.Value * SemicirclesToDegrees,
        };
    }

    /// <summary>
    /// Converts bounding box corner semicircle values to a <see cref="GeoBounds"/>.
    /// </summary>
    /// <param name="necLat">North-east corner latitude in semicircles.</param>
    /// <param name="necLong">North-east corner longitude in semicircles.</param>
    /// <param name="swcLat">South-west corner latitude in semicircles.</param>
    /// <param name="swcLong">South-west corner longitude in semicircles.</param>
    /// <returns>A <see cref="GeoBounds"/>, or <c>null</c> if any input is <c>null</c>.</returns>
    public static GeoBounds? ToGeoBounds(int? necLat, int? necLong, int? swcLat, int? swcLong)
    {
        if (necLat is null || necLong is null || swcLat is null || swcLong is null)
        {
            return null;
        }

        return new GeoBounds
        {
            NorthEast = new GeoCoordinate
            {
                LatitudeDegrees = necLat.Value * SemicirclesToDegrees,
                LongitudeDegrees = necLong.Value * SemicirclesToDegrees,
            },
            SouthWest = new GeoCoordinate
            {
                LatitudeDegrees = swcLat.Value * SemicirclesToDegrees,
                LongitudeDegrees = swcLong.Value * SemicirclesToDegrees,
            },
        };
    }
}
