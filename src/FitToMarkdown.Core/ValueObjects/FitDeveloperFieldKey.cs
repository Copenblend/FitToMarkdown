namespace FitToMarkdown.Core.ValueObjects;

/// <summary>
/// Represents the stable composite identity of one developer field definition.
/// </summary>
public sealed record FitDeveloperFieldKey
{
    /// <summary>The developer data index identifying the application or data source.</summary>
    public ushort DeveloperDataIndex { get; init; }

    /// <summary>The field definition number within the developer data index.</summary>
    public byte FieldDefinitionNumber { get; init; }
}
