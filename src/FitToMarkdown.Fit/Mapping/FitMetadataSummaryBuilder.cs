using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.Parsing;
using FitToMarkdown.Fit.Decoding;
using FitToMarkdown.Fit.Internal;
using static FitToMarkdown.Fit.Mapping.FitEnumMapper;
using static FitToMarkdown.Fit.Mapping.FitFieldValueConverter;

namespace FitToMarkdown.Fit.Mapping;

/// <summary>
/// Builds a <see cref="FitMetadataInspectionResult"/> from a metadata-only decode snapshot.
/// </summary>
internal sealed class FitMetadataSummaryBuilder
{
    /// <summary>
    /// Transforms a metadata-only snapshot into an inspection result.
    /// </summary>
    /// <param name="snapshot">The decoded snapshot containing accumulated metadata messages.</param>
    /// <returns>The assembled inspection result.</returns>
    public FitMetadataInspectionResult Build(FitDecodeSnapshot snapshot)
    {
        var fileType = FitFileTypeClassifier.Classify(snapshot);

        // Map FileId basics
        ushort? manufacturerId = null;
        ushort? productId = null;
        uint? serialNumber = null;
        DateTimeOffset? fileTimeCreated = null;

        if (snapshot.FileIdMesgs.Count > 0)
        {
            var fileIdMesg = snapshot.FileIdMesgs[0].Message;
            manufacturerId = ToNullableUshort(fileIdMesg.GetManufacturer());
            productId = ToNullableUshort(fileIdMesg.GetProduct());
            serialNumber = ToNullableUint(fileIdMesg.GetSerialNumber());
            fileTimeCreated = FitTimestampNormalizer.ToUtcDateTimeOffset(fileIdMesg.GetTimeCreated());
        }

        // Map first session basics
        FitSport? sport = null;
        FitSubSport? subSport = null;
        DateTimeOffset? startTimeUtc = null;
        TimeSpan? totalElapsedTime = null;
        TimeSpan? totalTimerTime = null;
        double? totalDistanceMeters = null;

        if (snapshot.SessionMesgs.Count > 0)
        {
            var sessionMesg = snapshot.SessionMesgs[0].Message;
            sport = MapSport(sessionMesg.GetSport());
            subSport = MapSubSport(sessionMesg.GetSubSport());
            startTimeUtc = FitTimestampNormalizer.ToUtcDateTimeOffset(sessionMesg.GetStartTime());
            totalElapsedTime = ToTimeSpan(sessionMesg.GetTotalElapsedTime());
            totalTimerTime = ToTimeSpan(sessionMesg.GetTotalTimerTime());
            totalDistanceMeters = ToNullableDouble(sessionMesg.GetTotalDistance());
        }

        // Map device summaries
        var devices = new List<FitDeviceSummary>(snapshot.DeviceInfoMesgs.Count);
        foreach (var (mesg, _) in snapshot.DeviceInfoMesgs)
        {
            var batteryVoltage = ToNullableDouble(mesg.GetBatteryVoltage());
            var batteryStatus = MapBatteryStatus(mesg.GetBatteryStatus());

            devices.Add(new FitDeviceSummary
            {
                DeviceType = ToNullableByte(mesg.GetAntDeviceType())?.ToString(),
                ManufacturerName = null,
                ProductId = ToNullableUshort(mesg.GetProduct()),
                ProductName = null,
                SerialNumber = ToNullableUint(mesg.GetSerialNumber()),
                BatteryVoltage = batteryVoltage,
                BatteryStatus = batteryStatus?.ToString(),
                Descriptor = null,
            });
        }

        var summary = new FitFileInfoSummary
        {
            InputFilePath = snapshot.SourceName ?? string.Empty,
            FileName = ExtractFileName(snapshot.SourceName),
            FileType = fileType?.ToString(),
            Sport = sport?.ToString(),
            SubSport = subSport?.ToString(),
            ManufacturerName = null,
            ProductId = productId,
            ProductName = null,
            SerialNumber = serialNumber,
            StartTimeUtc = startTimeUtc ?? fileTimeCreated,
            TotalElapsedTime = totalElapsedTime,
            TotalTimerTime = totalTimerTime,
            TotalDistanceMeters = totalDistanceMeters,
            LapCount = snapshot.LapMesgs.Count > 0 ? snapshot.LapMesgs.Count : null,
            RecordCount = snapshot.RecordMesgs.Count > 0 ? snapshot.RecordMesgs.Count : null,
            Devices = devices,
        };

        var status = snapshot.HadDecodeFault
            ? FitParseStatus.PartiallySucceeded
            : FitParseStatus.Succeeded;

        var issues = new List<FitParseIssue>();

        if (snapshot.FileIdMesgs.Count == 0)
        {
            issues.Add(FitParseIssueFactory.MetadataSummaryPartial("Missing File ID in metadata."));
        }

        if (snapshot.SessionMesgs.Count == 0 && snapshot.SportMesgs.Count == 0)
        {
            issues.Add(FitParseIssueFactory.MetadataSummaryPartial("No session or sport metadata available."));
        }

        return new FitMetadataInspectionResult
        {
            Summary = summary,
            Issues = issues,
            Metadata = new FitParseMetadata
            {
                Status = status,
                HadDecodeFault = snapshot.HadDecodeFault,
                IsPartial = snapshot.HadDecodeFault,
                TruncationDetected = snapshot.HadDecodeFault,
                DecodedMessageCount = snapshot.TotalMessageCount,
            },
        };
    }

    private static string ExtractFileName(string? sourceName)
    {
        if (string.IsNullOrWhiteSpace(sourceName))
            return string.Empty;

        return Path.GetFileName(sourceName);
    }
}
