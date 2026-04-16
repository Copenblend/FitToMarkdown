using FitToMarkdown.Core.Enums;

namespace FitToMarkdown.Core.ValueObjects;

/// <summary>
/// Represents battery status and voltage for a device or event-derived battery reading.
/// </summary>
public sealed record BatterySnapshot
{
    /// <summary>The battery status category.</summary>
    public FitBatteryStatus? Status { get; init; }

    /// <summary>The battery voltage in volts.</summary>
    public double? VoltageVolts { get; init; }

    /// <summary>The battery charge as a percentage.</summary>
    public double? ChargePercent { get; init; }
}
