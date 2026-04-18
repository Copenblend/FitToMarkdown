using System.Text;
using Dynastream.Fit;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.ValueObjects;

namespace FitToMarkdown.Fit.Mapping;

/// <summary>
/// Resolves developer field definitions and associates developer field values with their descriptors.
/// </summary>
internal sealed class FitDeveloperFieldResolver
{
    private readonly Dictionary<(byte DevDataIndex, byte FieldDefNum), FieldDescriptionMesg> _lookup;
    private readonly List<(byte DevDataIndex, byte FieldDefNum)> _duplicateDefinitions = [];
    private readonly List<(byte DevDataIndex, byte FieldDefNum)> _unresolvedFields = [];

    /// <summary>Gets the developer field definition keys that had duplicate definitions.</summary>
    internal IReadOnlyList<(byte DevDataIndex, byte FieldDefNum)> DuplicateDefinitions => _duplicateDefinitions;

    /// <summary>Gets the developer field keys that could not be resolved to a definition.</summary>
    internal IReadOnlyList<(byte DevDataIndex, byte FieldDefNum)> UnresolvedFields => _unresolvedFields;

    /// <summary>
    /// Initializes a new instance building a lookup from the accumulated field descriptions.
    /// </summary>
    public FitDeveloperFieldResolver(
        List<(FieldDescriptionMesg Message, int Sequence)> fieldDescriptions)
    {
        _lookup = new Dictionary<(byte, byte), FieldDescriptionMesg>();

        foreach (var (mesg, _) in fieldDescriptions)
        {
            byte? devIdx = mesg.GetDeveloperDataIndex();
            byte? fieldNum = mesg.GetFieldDefinitionNumber();
            if (devIdx.HasValue && fieldNum.HasValue)
            {
                var key = (devIdx.Value, fieldNum.Value);
                if (_lookup.ContainsKey(key))
                {
                    _duplicateDefinitions.Add(key);
                }
                _lookup[key] = mesg;
            }
        }
    }

    /// <summary>
    /// Resolves all developer fields attached to the given message.
    /// </summary>
    public IReadOnlyList<FitDeveloperFieldValue> ResolveDeveloperFields(Mesg mesg)
    {
        var devFields = mesg.DeveloperFields.ToList();
        if (devFields.Count == 0)
            return [];

        var results = new List<FitDeveloperFieldValue>(devFields.Count);

        foreach (var devField in devFields)
        {
            byte devDataIndex = devField.DeveloperDataIndex;
            byte fieldDefNum = devField.Num;
            bool resolved = _lookup.ContainsKey((devDataIndex, fieldDefNum));

            if (!resolved)
            {
                var unresolvedKey = (devDataIndex, fieldDefNum);
                if (!_unresolvedFields.Contains(unresolvedKey))
                {
                    _unresolvedFields.Add(unresolvedKey);
                }
            }

            object? rawValue = devField.GetValue();
            var (kind, numeric, integer, boolean, text) = ClassifyValue(rawValue);

            results.Add(new FitDeveloperFieldValue
            {
                Key = new FitDeveloperFieldKey
                {
                    DeveloperDataIndex = devDataIndex,
                    FieldDefinitionNumber = fieldDefNum,
                },
                DefinitionResolved = resolved,
                FieldName = devField.Name,
                Units = devField.GetUnits(),
                ValueKind = kind,
                NumericValue = numeric,
                IntegerValue = integer,
                BooleanValue = boolean,
                TextValue = text,
                RawDisplayValue = rawValue?.ToString(),
            });
        }

        return results;
    }

    /// <summary>
    /// Maps accumulated DeveloperDataIdMesg items to Core model instances.
    /// </summary>
    public static IReadOnlyList<FitDeveloperDataId> MapDeveloperDataIds(
        List<(DeveloperDataIdMesg Message, int Sequence)> items)
    {
        if (items.Count == 0)
            return [];

        var results = new List<FitDeveloperDataId>(items.Count);

        foreach (var (mesg, sequence) in items)
        {
            byte? devIdx = mesg.GetDeveloperDataIndex();

            results.Add(new FitDeveloperDataId
            {
                Message = FitMessageIdentityFactory.Create(sequence),
                DeveloperDataIndex = devIdx ?? 0,
                DeveloperIdHex = ExtractByteArrayAsHex(mesg, DeveloperDataIdMesg.FieldDefNum.DeveloperId),
                ApplicationIdHex = ExtractByteArrayAsHex(mesg, DeveloperDataIdMesg.FieldDefNum.ApplicationId),
                ManufacturerId = mesg.GetManufacturerId(),
                ApplicationVersion = mesg.GetApplicationVersion(),
            });
        }

        return results;
    }

    /// <summary>
    /// Maps accumulated FieldDescriptionMesg items to Core model instances.
    /// </summary>
    public static IReadOnlyList<FitFieldDescription> MapFieldDescriptions(
        List<(FieldDescriptionMesg Message, int Sequence)> items)
    {
        if (items.Count == 0)
            return [];

        var results = new List<FitFieldDescription>(items.Count);

        foreach (var (mesg, sequence) in items)
        {
            byte? devIdx = mesg.GetDeveloperDataIndex();
            byte? fieldNum = mesg.GetFieldDefinitionNumber();
            byte? baseTypeId = mesg.GetFitBaseTypeId();

            FitDeveloperFieldBaseType baseType = baseTypeId.HasValue
                && Enum.IsDefined(typeof(FitDeveloperFieldBaseType), baseTypeId.Value)
                    ? (FitDeveloperFieldBaseType)baseTypeId.Value
                    : FitDeveloperFieldBaseType.Unknown;

            results.Add(new FitFieldDescription
            {
                Message = FitMessageIdentityFactory.Create(sequence),
                Key = new FitDeveloperFieldKey
                {
                    DeveloperDataIndex = devIdx ?? 0,
                    FieldDefinitionNumber = fieldNum ?? 0,
                },
                BaseType = baseType,
                FieldName = ExtractStringField(mesg, FieldDescriptionMesg.FieldDefNum.FieldName),
                Units = ExtractStringField(mesg, FieldDescriptionMesg.FieldDefNum.Units),
                NativeFieldNumber = mesg.GetNativeFieldNum(),
                NativeMessageNumber = mesg.GetNativeMesgNum(),
                DefinitionResolved = devIdx.HasValue,
            });
        }

        return results;
    }

    private static string? ExtractByteArrayAsHex(Mesg mesg, byte fieldDefNum)
    {
        var field = mesg.GetField(fieldDefNum);
        if (field is null)
            return null;

        int count = field.GetNumValues();
        if (count == 0)
            return null;

        var bytes = new byte[count];
        for (int i = 0; i < count; i++)
        {
            object? val = field.GetValue(i);
            if (val is byte b)
                bytes[i] = b;
        }

        return Convert.ToHexString(bytes);
    }

    private static string? ExtractStringField(Mesg mesg, byte fieldDefNum)
    {
        var field = mesg.GetField(fieldDefNum);
        if (field is null)
            return null;

        int count = field.GetNumValues();
        if (count == 0)
            return null;

        var bytes = new List<byte>(count);
        for (int i = 0; i < count; i++)
        {
            object? val = field.GetValue(i);
            if (val is byte b && b != 0)
                bytes.Add(b);
        }

        return bytes.Count > 0 ? Encoding.UTF8.GetString(bytes.ToArray()) : null;
    }

    private static (FitDeveloperValueKind Kind, double? Numeric, long? Integer, bool? Boolean, string? Text) ClassifyValue(
        object? rawValue)
    {
        if (rawValue is null)
            return (FitDeveloperValueKind.Unknown, null, null, null, null);

        return rawValue switch
        {
            bool b => (FitDeveloperValueKind.Boolean, null, null, b, null),
            string s => (FitDeveloperValueKind.Text, null, null, null, s),
            byte v => (FitDeveloperValueKind.Integer, null, v, null, null),
            sbyte v => (FitDeveloperValueKind.Integer, null, v, null, null),
            short v => (FitDeveloperValueKind.Integer, null, v, null, null),
            ushort v => (FitDeveloperValueKind.Integer, null, v, null, null),
            int v => (FitDeveloperValueKind.Integer, null, v, null, null),
            uint v => (FitDeveloperValueKind.Integer, null, v, null, null),
            long v => (FitDeveloperValueKind.Integer, null, v, null, null),
            ulong v => (FitDeveloperValueKind.Integer, null, unchecked((long)v), null, null),
            float v => (FitDeveloperValueKind.Numeric, v, null, null, null),
            double v => (FitDeveloperValueKind.Numeric, v, null, null, null),
            _ => (FitDeveloperValueKind.Unknown, null, null, null, rawValue.ToString()),
        };
    }
}
