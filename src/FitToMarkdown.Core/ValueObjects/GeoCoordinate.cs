namespace FitToMarkdown.Core.ValueObjects;

/// <summary>
/// Represents one normalized FIT coordinate converted from semicircles to decimal degrees.
/// </summary>
public sealed record GeoCoordinate
{
    /// <summary>Latitude in decimal degrees.</summary>
    public double LatitudeDegrees { get; init; }

    /// <summary>Longitude in decimal degrees.</summary>
    public double LongitudeDegrees { get; init; }
}
