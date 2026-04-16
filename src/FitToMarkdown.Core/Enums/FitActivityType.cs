namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Mirrors the Dynastream FIT profile ActivityType enum. Numeric values are preserved from the FIT SDK.
/// </summary>
public enum FitActivityType : byte
{
    /// <summary>Generic activity type.</summary>
    Generic = 0,

    /// <summary>Running.</summary>
    Running = 1,

    /// <summary>Cycling.</summary>
    Cycling = 2,

    /// <summary>Transition between sports.</summary>
    Transition = 3,

    /// <summary>Fitness equipment.</summary>
    FitnessEquipment = 4,

    /// <summary>Swimming.</summary>
    Swimming = 5,

    /// <summary>Walking.</summary>
    Walking = 6,

    /// <summary>Sedentary.</summary>
    Sedentary = 8,

    /// <summary>All activity types.</summary>
    All = 254,

    /// <summary>Unknown or invalid activity type.</summary>
    Unknown = 255,
}
