namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Mirrors the Dynastream FIT profile SubSport enum. Numeric values are preserved from the FIT SDK.
/// </summary>
public enum FitSubSport : byte
{
    /// <summary>Generic sub-sport.</summary>
    Generic = 0,

    /// <summary>Treadmill running.</summary>
    Treadmill = 1,

    /// <summary>Street running.</summary>
    Street = 2,

    /// <summary>Trail running.</summary>
    Trail = 3,

    /// <summary>Track running.</summary>
    Track = 4,

    /// <summary>Spin cycling.</summary>
    Spin = 5,

    /// <summary>Indoor cycling.</summary>
    IndoorCycling = 6,

    /// <summary>Road cycling.</summary>
    Road = 7,

    /// <summary>Mountain biking.</summary>
    Mountain = 8,

    /// <summary>Downhill cycling.</summary>
    Downhill = 9,

    /// <summary>Recumbent cycling.</summary>
    Recumbent = 10,

    /// <summary>Cyclocross.</summary>
    Cyclocross = 11,

    /// <summary>Hand cycling.</summary>
    HandCycling = 12,

    /// <summary>Track cycling.</summary>
    TrackCycling = 13,

    /// <summary>Indoor rowing.</summary>
    IndoorRowing = 14,

    /// <summary>Elliptical trainer.</summary>
    Elliptical = 15,

    /// <summary>Stair climbing.</summary>
    StairClimbing = 16,

    /// <summary>Lap swimming (pool).</summary>
    LapSwimming = 17,

    /// <summary>Open water swimming.</summary>
    OpenWater = 18,

    /// <summary>Flexibility training.</summary>
    FlexibilityTraining = 19,

    /// <summary>Strength training.</summary>
    StrengthTraining = 20,

    /// <summary>Warm-up.</summary>
    WarmUp = 21,

    /// <summary>Match.</summary>
    Match = 22,

    /// <summary>Exercise.</summary>
    Exercise = 23,

    /// <summary>Challenge.</summary>
    Challenge = 24,

    /// <summary>Indoor skiing.</summary>
    IndoorSkiing = 25,

    /// <summary>Cardio training.</summary>
    CardioTraining = 26,

    /// <summary>Indoor walking.</summary>
    IndoorWalking = 27,

    /// <summary>E-bike fitness.</summary>
    EBikeFitness = 28,

    /// <summary>BMX cycling.</summary>
    Bmx = 29,

    /// <summary>Casual walking.</summary>
    CasualWalking = 30,

    /// <summary>Speed walking.</summary>
    SpeedWalking = 31,

    /// <summary>Bike-to-run transition.</summary>
    BikeToRunTransition = 32,

    /// <summary>Run-to-bike transition.</summary>
    RunToBikeTransition = 33,

    /// <summary>Swim-to-bike transition.</summary>
    SwimToBikeTransition = 34,

    /// <summary>ATV (all-terrain vehicle).</summary>
    Atv = 35,

    /// <summary>Motocross.</summary>
    Motocross = 36,

    /// <summary>Backcountry skiing or snowboarding.</summary>
    Backcountry = 37,

    /// <summary>Resort skiing or snowboarding.</summary>
    Resort = 38,

    /// <summary>RC drone flying.</summary>
    RcDrone = 39,

    /// <summary>Wingsuit flying.</summary>
    Wingsuit = 40,

    /// <summary>Whitewater rafting or kayaking.</summary>
    Whitewater = 41,

    /// <summary>Skate skiing.</summary>
    SkateSkiing = 42,

    /// <summary>Yoga.</summary>
    Yoga = 43,

    /// <summary>Pilates.</summary>
    Pilates = 44,

    /// <summary>Indoor running.</summary>
    IndoorRunning = 45,

    /// <summary>Gravel cycling.</summary>
    GravelCycling = 46,

    /// <summary>E-bike mountain biking.</summary>
    EBikeMountain = 47,

    /// <summary>Commuting.</summary>
    Commuting = 48,

    /// <summary>Mixed surface cycling.</summary>
    MixedSurface = 49,

    /// <summary>Navigate.</summary>
    Navigate = 50,

    /// <summary>Track me.</summary>
    TrackMe = 51,

    /// <summary>Map.</summary>
    Map = 52,

    /// <summary>All sub-sports.</summary>
    All = 254,

    /// <summary>Unknown or invalid sub-sport.</summary>
    Unknown = 255,
}
