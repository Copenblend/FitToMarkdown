using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.Parsing;

namespace FitToMarkdown.Core.Documents;

/// <summary>
/// Represents one normalized FIT file and all Core-owned parse output for that file.
/// </summary>
public sealed record FitFileDocument
{
    /// <summary>File ID message from the FIT file.</summary>
    public FitFileId? FileId { get; init; }

    /// <summary>File creator message, if present.</summary>
    public FitFileCreator? FileCreator { get; init; }

    /// <summary>Explicit parser state, recovery state, and coverage metadata.</summary>
    public FitParseMetadata ParseMetadata { get; init; } = new();

    /// <summary>Parsed activity content. Non-null when the file is an Activity type.</summary>
    public FitActivityContent? ActivityContent { get; init; }

    /// <summary>Workout definition, if present.</summary>
    public FitWorkout? Workout { get; init; }

    /// <summary>Course definition, if present.</summary>
    public FitCourse? Course { get; init; }

    /// <summary>Parsed monitoring content. Non-null when the file is a Monitoring type.</summary>
    public FitMonitoringContent? MonitoringContent { get; init; }

    /// <summary>All device info messages parsed from the file.</summary>
    public IReadOnlyList<FitDeviceInfo> DeviceInfos { get; init; } = [];

    /// <summary>Developer data ID messages parsed from the file.</summary>
    public IReadOnlyList<FitDeveloperDataId> DeveloperDataIds { get; init; } = [];

    /// <summary>Developer field description messages parsed from the file.</summary>
    public IReadOnlyList<FitFieldDescription> FieldDescriptions { get; init; } = [];
}
