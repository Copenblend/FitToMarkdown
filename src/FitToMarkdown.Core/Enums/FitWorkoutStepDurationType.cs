namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Mirrors the Dynastream FIT profile WktStepDuration enum. Numeric values are preserved from the FIT SDK.
/// </summary>
public enum FitWorkoutStepDurationType : byte
{
    /// <summary>Time-based duration.</summary>
    Time = 0,

    /// <summary>Distance-based duration.</summary>
    Distance = 1,

    /// <summary>Heart rate less than threshold.</summary>
    HrLessThan = 2,

    /// <summary>Heart rate greater than threshold.</summary>
    HrGreaterThan = 3,

    /// <summary>Calorie-based duration.</summary>
    Calories = 4,

    /// <summary>Open (no target, press lap to advance).</summary>
    Open = 5,

    /// <summary>Repeat until steps complete.</summary>
    RepeatUntilStepsCmplt = 6,

    /// <summary>Repeat until time.</summary>
    RepeatUntilTime = 7,

    /// <summary>Repeat until distance.</summary>
    RepeatUntilDistance = 8,

    /// <summary>Repeat until calories.</summary>
    RepeatUntilCalories = 9,

    /// <summary>Repeat until heart rate less than threshold.</summary>
    RepeatUntilHrLessThan = 10,

    /// <summary>Repeat until heart rate greater than threshold.</summary>
    RepeatUntilHrGreaterThan = 11,

    /// <summary>Repeat until power less than threshold.</summary>
    RepeatUntilPowerLessThan = 12,

    /// <summary>Repeat until power greater than threshold.</summary>
    RepeatUntilPowerGreaterThan = 13,

    /// <summary>Power less than threshold.</summary>
    PowerLessThan = 14,

    /// <summary>Power greater than threshold.</summary>
    PowerGreaterThan = 15,

    /// <summary>TrainingPeaks TSS-based duration.</summary>
    TrainingPeaksTss = 16,

    /// <summary>Repeat until power last lap less than threshold.</summary>
    RepeatUntilPowerLastLapLessThan = 17,

    /// <summary>Repeat until max power last lap less than threshold.</summary>
    RepeatUntilMaxPowerLastLapLessThan = 18,

    /// <summary>3-second power less than threshold.</summary>
    Power3sLessThan = 19,

    /// <summary>10-second power less than threshold.</summary>
    Power10sLessThan = 20,

    /// <summary>30-second power less than threshold.</summary>
    Power30sLessThan = 21,

    /// <summary>3-second power greater than threshold.</summary>
    Power3sGreaterThan = 22,

    /// <summary>10-second power greater than threshold.</summary>
    Power10sGreaterThan = 23,

    /// <summary>30-second power greater than threshold.</summary>
    Power30sGreaterThan = 24,

    /// <summary>Lap power less than threshold.</summary>
    PowerLapLessThan = 25,

    /// <summary>Lap power greater than threshold.</summary>
    PowerLapGreaterThan = 26,

    /// <summary>Repeat until TrainingPeaks TSS.</summary>
    RepeatUntilTrainingPeaksTss = 27,

    /// <summary>Repetition time-based duration.</summary>
    RepetitionTime = 28,

    /// <summary>Unknown or invalid workout step duration type.</summary>
    Unknown = 255,
}
