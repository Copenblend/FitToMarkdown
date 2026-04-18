using Dynastream.Fit;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.ValueObjects;
using FitToMarkdown.Fit.Internal;
using static FitToMarkdown.Fit.Mapping.FitEnumMapper;
using static FitToMarkdown.Fit.Mapping.FitFieldValueConverter;

namespace FitToMarkdown.Fit.Mapping;

/// <summary>
/// Maps DeviceInfoMesg, UserProfileMesg, ZonesTargetMesg, and SportMesg to Core models.
/// </summary>
internal static class DeviceAndProfileMapper
{
    /// <summary>
    /// Maps all DeviceInfoMesg items to Core FitDeviceInfo models.
    /// The first device (deviceIndex 0) is marked as the creator.
    /// </summary>
    public static IReadOnlyList<FitDeviceInfo> MapDeviceInfos(
        List<(DeviceInfoMesg Message, int Sequence)> items,
        FitDeveloperFieldResolver? devResolver)
    {
        if (items.Count == 0)
            return [];

        var results = new List<FitDeviceInfo>(items.Count);

        foreach (var (mesg, seq) in items)
        {
            var deviceIndex = ToNullableUshort(mesg.GetDeviceIndex());
            var batteryStatus = MapBatteryStatus(mesg.GetBatteryStatus());
            var batteryVoltage = ToNullableDouble(mesg.GetBatteryVoltage());
            var battery = batteryStatus is not null || batteryVoltage is not null
                ? new BatterySnapshot
                {
                    Status = batteryStatus,
                    VoltageVolts = batteryVoltage,
                }
                : null;

            results.Add(new FitDeviceInfo
            {
                Message = FitMessageIdentityFactory.Create(seq),
                TimestampUtc = FitTimestampNormalizer.ToUtcDateTimeOffset(mesg.GetTimestamp()),
                DeviceIndex = deviceIndex,
                Role = MapDeviceRole(deviceIndex),
                DeviceType = ToNullableByte(mesg.GetAntDeviceType()),
                ManufacturerId = ToNullableUshort(mesg.GetManufacturer()),
                ManufacturerName = null,
                SerialNumber = ToNullableUint(mesg.GetSerialNumber()),
                ProductId = ToNullableUshort(mesg.GetProduct()),
                ProductName = null,
                SoftwareVersion = ToNullableDouble(mesg.GetSoftwareVersion()),
                HardwareVersion = ToNullableByte(mesg.GetHardwareVersion()),
                Battery = battery,
                SensorPosition = null,
                Descriptor = null,
                AntTransmissionType = ToNullableByte(mesg.GetAntTransmissionType()),
                AntDeviceNumber = ToNullableUshort(mesg.GetAntDeviceNumber()),
                AntNetwork = EnumToString(mesg.GetAntNetwork()),
                SourceType = EnumToString(mesg.GetSourceType()),
                IsCreator = deviceIndex == 0,
                DeveloperFields = devResolver?.ResolveDeveloperFields(mesg) ?? [],
            });
        }

        return results;
    }

    /// <summary>
    /// Maps the first UserProfileMesg to a Core FitUserProfile.
    /// </summary>
    public static FitUserProfile? MapUserProfile(List<(UserProfileMesg Message, int Sequence)> items)
    {
        if (items.Count == 0)
            return null;

        var (mesg, seq) = items[0];

        return new FitUserProfile
        {
            Message = FitMessageIdentityFactory.Create(seq),
            FriendlyName = ByteArrayToString(SafeGet(mesg.GetFriendlyName)),
            Gender = MapGender(mesg.GetGender()),
            AgeYears = ToNullableByte(mesg.GetAge()),
            HeightCentimeters = ToNullableDouble(mesg.GetHeight()) is double h ? h * 100.0 : null,
            WeightKilograms = ToNullableDouble(mesg.GetWeight()),
            Language = EnumToString(mesg.GetLanguage()),
            ElevationSetting = EnumToString(mesg.GetElevSetting()),
            WeightSetting = EnumToString(mesg.GetWeightSetting()),
            RestingHeartRateBpm = ToNullableByte(mesg.GetRestingHeartRate()),
            DefaultMaxRunningHeartRateBpm = ToNullableByte(mesg.GetDefaultMaxRunningHeartRate()),
            DefaultMaxBikingHeartRateBpm = ToNullableByte(mesg.GetDefaultMaxBikingHeartRate()),
            DefaultMaxHeartRateBpm = ToNullableByte(mesg.GetDefaultMaxHeartRate()),
            HeartRateSetting = EnumToString(mesg.GetHrSetting()),
            SpeedSetting = EnumToString(mesg.GetSpeedSetting()),
            DistanceSetting = EnumToString(mesg.GetDistSetting()),
            ActivityClass = null,
            PositionSetting = EnumToString(mesg.GetPositionSetting()),
            TemperatureSetting = EnumToString(mesg.GetTemperatureSetting()),
            HeightSetting = EnumToString(mesg.GetHeightSetting()),
        };
    }

    /// <summary>
    /// Maps the first ZonesTargetMesg to a Core FitZonesTarget.
    /// </summary>
    public static FitZonesTarget? MapZonesTarget(List<(ZonesTargetMesg Message, int Sequence)> items)
    {
        if (items.Count == 0)
            return null;

        var (mesg, seq) = items[0];

        return new FitZonesTarget
        {
            Message = FitMessageIdentityFactory.Create(seq),
            MaximumHeartRateBpm = ToNullableByte(mesg.GetMaxHeartRate()),
            ThresholdHeartRateBpm = ToNullableByte(mesg.GetThresholdHeartRate()),
            FunctionalThresholdPowerWatts = ToNullableUshort(mesg.GetFunctionalThresholdPower()),
            HeartRateCalculationType = EnumToString(mesg.GetHrCalcType()),
            PowerCalculationType = EnumToString(mesg.GetPwrCalcType()),
        };
    }

    /// <summary>
    /// Maps all SportMesg items to Core FitSportProfile models.
    /// </summary>
    public static IReadOnlyList<FitSportProfile> MapSportProfiles(List<(SportMesg Message, int Sequence)> items)
    {
        if (items.Count == 0)
            return [];

        var results = new List<FitSportProfile>(items.Count);

        foreach (var (mesg, seq) in items)
        {
            results.Add(new FitSportProfile
            {
                Message = FitMessageIdentityFactory.Create(seq),
                Sport = MapSport(mesg.GetSport()),
                SubSport = MapSubSport(mesg.GetSubSport()),
                Name = ByteArrayToString(mesg.GetName()),
            });
        }

        return results;
    }
}
