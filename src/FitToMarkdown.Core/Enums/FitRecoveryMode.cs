namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Records which recovery strategy produced the final result.
/// </summary>
public enum FitRecoveryMode : byte
{
    /// <summary>No recovery was needed; file parsed cleanly.</summary>
    None = 0,

    /// <summary>Partial data was recovered on a best-effort basis.</summary>
    BestEffortPartial = 1,

    /// <summary>A synthetic activity wrapper was generated around raw data.</summary>
    SyntheticActivity = 2,

    /// <summary>A synthetic session was generated from available records.</summary>
    SyntheticSession = 3,
}
