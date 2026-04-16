using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents one developer field value attached to a normalized message.
/// </summary>
public sealed record FitDeveloperFieldValue
{
    /// <summary>Composite key identifying the developer field definition.</summary>
    public FitDeveloperFieldKey Key { get; init; } = new();

    /// <summary>Indicates whether the field definition was resolved against a known DeveloperDataId.</summary>
    public bool DefinitionResolved { get; init; }

    /// <summary>Human-readable field name.</summary>
    public string? FieldName { get; init; }

    /// <summary>Unit of measurement for the field.</summary>
    public string? Units { get; init; }

    /// <summary>Kind of value stored in this developer field.</summary>
    public FitDeveloperValueKind ValueKind { get; init; }

    /// <summary>Numeric value when the field contains a floating-point number.</summary>
    public double? NumericValue { get; init; }

    /// <summary>Integer value when the field contains a whole number.</summary>
    public long? IntegerValue { get; init; }

    /// <summary>Boolean value when the field contains a true/false value.</summary>
    public bool? BooleanValue { get; init; }

    /// <summary>Text value when the field contains a string.</summary>
    public string? TextValue { get; init; }

    /// <summary>Numeric array values when the field contains multiple floating-point numbers.</summary>
    public IReadOnlyList<double> NumericArrayValues { get; init; } = [];

    /// <summary>Integer array values when the field contains multiple whole numbers.</summary>
    public IReadOnlyList<long> IntegerArrayValues { get; init; } = [];

    /// <summary>Text array values when the field contains multiple strings.</summary>
    public IReadOnlyList<string> TextArrayValues { get; init; } = [];

    /// <summary>Raw display value as a formatted string.</summary>
    public string? RawDisplayValue { get; init; }
}
