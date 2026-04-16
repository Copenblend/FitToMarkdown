namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Mirrors the Dynastream FIT profile SessionTrigger enum. Numeric values are preserved from the FIT SDK.
/// </summary>
public enum FitSessionTrigger : byte
{
    /// <summary>Session triggered by activity end.</summary>
    ActivityEnd = 0,

    /// <summary>Manually triggered session.</summary>
    Manual = 1,

    /// <summary>Auto multi-sport session trigger.</summary>
    AutoMultiSport = 2,

    /// <summary>Fitness equipment session trigger.</summary>
    FitnessEquipment = 3,

    /// <summary>Unknown or invalid session trigger.</summary>
    Unknown = 255,
}
