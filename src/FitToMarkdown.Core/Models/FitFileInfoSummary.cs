namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the compact metadata summary consumed by the CLI info command.
/// </summary>
public sealed record FitFileInfoSummary
{
    /// <summary>Full path to the input FIT file.</summary>
    public string InputFilePath { get; init; } = string.Empty;

    /// <summary>File name without path.</summary>
    public string FileName { get; init; } = string.Empty;

    /// <summary>FIT file type descriptor.</summary>
    public string? FileType { get; init; }

    /// <summary>Primary sport descriptor.</summary>
    public string? Sport { get; init; }

    /// <summary>Sub-sport descriptor.</summary>
    public string? SubSport { get; init; }

    /// <summary>Resolved manufacturer display name.</summary>
    public string? ManufacturerName { get; init; }

    /// <summary>Product identifier.</summary>
    public ushort? ProductId { get; init; }

    /// <summary>Resolved product display name.</summary>
    public string? ProductName { get; init; }

    /// <summary>Device serial number.</summary>
    public uint? SerialNumber { get; init; }

    /// <summary>Activity start time in UTC.</summary>
    public DateTimeOffset? StartTimeUtc { get; init; }

    /// <summary>Total elapsed time including pauses.</summary>
    public TimeSpan? TotalElapsedTime { get; init; }

    /// <summary>Total timer time excluding pauses.</summary>
    public TimeSpan? TotalTimerTime { get; init; }

    /// <summary>Total distance in meters.</summary>
    public double? TotalDistanceMeters { get; init; }

    /// <summary>Number of laps in the activity.</summary>
    public int? LapCount { get; init; }

    /// <summary>Number of records in the activity.</summary>
    public int? RecordCount { get; init; }

    /// <summary>Compact device summaries for all devices.</summary>
    public IReadOnlyList<FitDeviceSummary> Devices { get; init; } = [];
}
