namespace FitToMarkdown.Core.Enums;

/// <summary>
/// Mirrors the Dynastream FIT profile FitBaseType constants. The SDK defines this as a static class
/// with byte constants, but Core models it as an enum. Numeric values are preserved from the FIT SDK.
/// </summary>
public enum FitDeveloperFieldBaseType : byte
{
    /// <summary>Enum base type.</summary>
    Enum = 0,

    /// <summary>Signed 8-bit integer.</summary>
    Sint8 = 1,

    /// <summary>Unsigned 8-bit integer.</summary>
    Uint8 = 2,

    /// <summary>Null-terminated UTF-8 string.</summary>
    String = 7,

    /// <summary>Unsigned 8-bit integer (zero invalid).</summary>
    Uint8z = 10,

    /// <summary>Byte array.</summary>
    Byte = 13,

    /// <summary>Signed 16-bit integer.</summary>
    Sint16 = 131,

    /// <summary>Unsigned 16-bit integer.</summary>
    Uint16 = 132,

    /// <summary>Signed 32-bit integer.</summary>
    Sint32 = 133,

    /// <summary>Unsigned 32-bit integer.</summary>
    Uint32 = 134,

    /// <summary>32-bit floating point.</summary>
    Float32 = 136,

    /// <summary>64-bit floating point.</summary>
    Float64 = 137,

    /// <summary>Unsigned 16-bit integer (zero invalid).</summary>
    Uint16z = 139,

    /// <summary>Unsigned 32-bit integer (zero invalid).</summary>
    Uint32z = 140,

    /// <summary>Signed 64-bit integer.</summary>
    Sint64 = 142,

    /// <summary>Unsigned 64-bit integer.</summary>
    Uint64 = 143,

    /// <summary>Unsigned 64-bit integer (zero invalid).</summary>
    Uint64z = 144,

    /// <summary>Unknown or invalid base type.</summary>
    Unknown = 255,
}
