using Dynastream.Fit;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Fit.Internal;
using static FitToMarkdown.Fit.Mapping.FitEnumMapper;
using static FitToMarkdown.Fit.Mapping.FitFieldValueConverter;

namespace FitToMarkdown.Fit.Mapping;

/// <summary>
/// Maps FileIdMesg, FileCreatorMesg, and ActivityMesg to Core file metadata models.
/// </summary>
internal static class FileMetadataMapper
{
    /// <summary>
    /// Maps the first FileIdMesg to a Core FitFileId.
    /// </summary>
    public static FitFileId? MapFileId(List<(FileIdMesg Message, int Sequence)> items)
    {
        if (items.Count == 0)
            return null;

        var (mesg, seq) = items[0];

        return new FitFileId
        {
            Message = FitMessageIdentityFactory.Create(seq),
            FileType = MapFileType(mesg.GetType()),
            ManufacturerId = ToNullableUshort(mesg.GetManufacturer()),
            ManufacturerName = null,
            ProductId = ToNullableUshort(mesg.GetProduct()),
            ProductName = null,
            SerialNumber = ToNullableUint(mesg.GetSerialNumber()),
            TimeCreatedUtc = FitTimestampNormalizer.ToUtcDateTimeOffset(mesg.GetTimeCreated()),
            Number = ToNullableUshort(mesg.GetNumber()),
        };
    }

    /// <summary>
    /// Maps the first FileCreatorMesg to a Core FitFileCreator.
    /// </summary>
    public static FitFileCreator? MapFileCreator(List<(FileCreatorMesg Message, int Sequence)> items)
    {
        if (items.Count == 0)
            return null;

        var (mesg, seq) = items[0];

        return new FitFileCreator
        {
            Message = FitMessageIdentityFactory.Create(seq),
            SoftwareVersion = (double?)ToNullableUshort(mesg.GetSoftwareVersion()),
            HardwareVersion = ToNullableByte(mesg.GetHardwareVersion()),
        };
    }

    /// <summary>
    /// Maps the first ActivityMesg to a Core FitActivity.
    /// </summary>
    public static FitActivity? MapActivity(List<(ActivityMesg Message, int Sequence)> items)
    {
        if (items.Count == 0)
            return null;

        var (mesg, seq) = items[0];
        var utcTimestamp = FitTimestampNormalizer.ToUtcDateTimeOffset(mesg.GetTimestamp());
        var localFitTs = ToFitDateTime(mesg.GetLocalTimestamp());
        var localOffset = FitTimestampNormalizer.DeriveLocalTimeOffset(mesg.GetTimestamp(), localFitTs);

        return new FitActivity
        {
            Message = FitMessageIdentityFactory.Create(seq),
            TimestampUtc = utcTimestamp,
            TotalTimerTime = ToTimeSpan(mesg.GetTotalTimerTime()),
            NumberOfSessions = ToNullableUshort(mesg.GetNumSessions()),
            Type = null, // Activity.type enum (Manual/AutoMultiSport) does not map to FitActivityType
            Event = MapEvent(mesg.GetEvent()),
            EventType = MapEventType(mesg.GetEventType()),
            LocalTimestamp = FitTimestampNormalizer.ToUtcDateTimeOffset(localFitTs),
            LocalTimeOffset = localOffset,
        };
    }

    private static Dynastream.Fit.DateTime? ToFitDateTime(uint? rawTimestamp) =>
        rawTimestamp.HasValue ? new Dynastream.Fit.DateTime(rawTimestamp.Value) : null;
}
