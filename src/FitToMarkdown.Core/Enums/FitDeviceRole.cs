namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Stable device classification for summaries and markdown output ordering.
/// </summary>
public enum FitDeviceRole : byte
{
    /// <summary>The primary recording device that created the file.</summary>
    Creator = 0,

    /// <summary>Heart-rate monitor strap or optical sensor.</summary>
    HeartRateMonitor = 1,

    /// <summary>Power meter (crank, pedal, hub, or spider).</summary>
    PowerMeter = 2,

    /// <summary>Foot pod for running cadence and pace.</summary>
    FootPod = 3,

    /// <summary>Stand-alone cadence sensor.</summary>
    CadenceSensor = 4,

    /// <summary>Stand-alone speed sensor.</summary>
    SpeedSensor = 5,

    /// <summary>Combined speed and cadence sensor.</summary>
    SpeedCadenceSensor = 6,

    /// <summary>Rear-facing radar unit.</summary>
    Radar = 7,

    /// <summary>ANT+ or BLE bike light.</summary>
    BikeLight = 8,

    /// <summary>Smart trainer or stationary fitness equipment.</summary>
    FitnessEquipment = 9,

    /// <summary>Remote control accessory.</summary>
    Remote = 10,

    /// <summary>Device role could not be determined.</summary>
    Unknown = 255,
}
