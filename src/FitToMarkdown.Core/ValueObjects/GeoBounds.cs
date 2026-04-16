namespace FitToMarkdown.Core.ValueObjects;

/// <summary>
/// Represents a normalized geographic bounding box.
/// </summary>
public sealed record GeoBounds
{
    /// <summary>The north-east corner of the bounding box.</summary>
    public required GeoCoordinate NorthEast { get; init; }

    /// <summary>The south-west corner of the bounding box.</summary>
    public required GeoCoordinate SouthWest { get; init; }
}
