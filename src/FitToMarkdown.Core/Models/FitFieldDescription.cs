using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Core.Models;

/// <summary>
/// Represents the normalized FieldDescriptionMesg from a FIT file.
/// </summary>
public sealed record FitFieldDescription
{
    /// <summary>Parse-order identity and recovery provenance.</summary>
    public FitMessageIdentity Message { get; init; } = new();

    /// <summary>Composite key identifying the developer field definition.</summary>
    public FitDeveloperFieldKey Key { get; init; } = new();

    /// <summary>Base type of the developer field.</summary>
    public FitDeveloperFieldBaseType BaseType { get; init; }

    /// <summary>Human-readable field name.</summary>
    public string? FieldName { get; init; }

    /// <summary>Unit of measurement for the field.</summary>
    public string? Units { get; init; }

    /// <summary>Native field number this developer field overrides.</summary>
    public byte? NativeFieldNumber { get; init; }

    /// <summary>Native message number this developer field is associated with.</summary>
    public ushort? NativeMessageNumber { get; init; }

    /// <summary>Indicates whether the field definition was resolved against a known DeveloperDataId.</summary>
    public bool DefinitionResolved { get; init; }
}
