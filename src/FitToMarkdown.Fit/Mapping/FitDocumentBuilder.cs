using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Models;
using FitToMarkdown.Core.Parsing;
using FitToMarkdown.Core.ValueObjects;
using FitToMarkdown.Fit.Decoding;
using FitToMarkdown.Fit.Grouping;
using FitToMarkdown.Fit.Internal;
using FitToMarkdown.Fit.Recovery;
using FitToMarkdown.Fit.Validation;

namespace FitToMarkdown.Fit.Mapping;

/// <summary>
/// Builds a <see cref="FitParseResult"/> from a decoded snapshot by coordinating mappers, grouping, and recovery.
/// </summary>
internal sealed class FitDocumentBuilder
{
    private readonly FitValidationPolicy _validationPolicy;

    public FitDocumentBuilder(FitValidationPolicy validationPolicy)
    {
        _validationPolicy = validationPolicy;
    }

    public FitDocumentBuilder()
        : this(new FitValidationPolicy())
    {
    }

    /// <summary>
    /// Transforms a decoded snapshot into a full parse result.
    /// </summary>
    /// <param name="snapshot">The decoded snapshot containing accumulated SDK messages.</param>
    /// <param name="options">Parse options governing mapping behavior.</param>
    /// <returns>The assembled parse result.</returns>
    public FitParseResult Build(FitDecodeSnapshot snapshot, FitParseOptions options)
    {
        // 1. Validate
        var validation = _validationPolicy.Validate(snapshot, options);
        var issues = new List<FitParseIssue>(validation.Issues);

        if (!validation.IsValid && !options.AllowPartialExtraction)
        {
            return new FitParseResult
            {
                Metadata = BuildMetadata(snapshot, issues, fileType: null, activityContent: null, recoveryUsed: false),
                Issues = issues,
                FatalError = BuildFatalError(validation),
            };
        }

        // 2. Create developer field resolver if needed
        FitDeveloperFieldResolver? devResolver = null;
        if (options.ResolveDeveloperFields && snapshot.FieldDescriptionMesgs.Count > 0)
        {
            devResolver = new FitDeveloperFieldResolver(snapshot.FieldDescriptionMesgs);
            foreach (var (devIdx, fieldNum) in devResolver.DuplicateDefinitions)
            {
                issues.Add(FitParseIssueFactory.DuplicateDeveloperDefinition(devIdx, fieldNum));
            }
        }

        // 3. Map file-level metadata
        var fileId = FileMetadataMapper.MapFileId(snapshot.FileIdMesgs);
        var fileCreator = FileMetadataMapper.MapFileCreator(snapshot.FileCreatorMesgs);
        var activity = FileMetadataMapper.MapActivity(snapshot.ActivityMesgs);

        // 4. Map devices and profiles
        var deviceInfos = DeviceAndProfileMapper.MapDeviceInfos(snapshot.DeviceInfoMesgs, devResolver);
        var userProfile = DeviceAndProfileMapper.MapUserProfile(snapshot.UserProfileMesgs);
        var zonesTarget = DeviceAndProfileMapper.MapZonesTarget(snapshot.ZonesTargetMesgs);
        var sportProfiles = DeviceAndProfileMapper.MapSportProfiles(snapshot.SportMesgs);

        // 5. Map developer data
        var devDataIds = devResolver is not null
            ? FitDeveloperFieldResolver.MapDeveloperDataIds(snapshot.DeveloperDataIdMesgs)
            : (IReadOnlyList<FitDeveloperDataId>)[];
        var fieldDescs = devResolver is not null
            ? FitDeveloperFieldResolver.MapFieldDescriptions(snapshot.FieldDescriptionMesgs)
            : (IReadOnlyList<FitFieldDescription>)[];

        // 6. Determine file type
        var fileType = FitFileTypeClassifier.Classify(snapshot);

        // 7. Build content based on file type
        FitActivityContent? activityContent = null;
        FitWorkout? workout = null;
        FitCourse? course = null;
        FitMonitoringContent? monitoringContent = null;
        bool recoveryUsed = false;

        if (fileType is FitFileType.Activity or FitFileType.ActivitySummary or null)
        {
            // Map all activity components
            var sessions = SessionAndLapMapper.MapSessions(snapshot.SessionMesgs, devResolver);
            var laps = SessionAndLapMapper.MapLaps(snapshot.LapMesgs, devResolver);
            var lengths = SessionAndLapMapper.MapLengths(snapshot.LengthMesgs, devResolver);
            var segmentLaps = SessionAndLapMapper.MapSegmentLaps(snapshot.SegmentLapMesgs, devResolver);
            var records = RecordMapper.MapRecords(snapshot.RecordMesgs, devResolver);
            var hrvMessages = RecordMapper.MapHrvMessages(snapshot.HrvMesgs);
            var hrMessages = RecordMapper.MapHrMessages(snapshot.HrMesgs);
            var events = EventMapper.MapEvents(snapshot.EventMesgs, devResolver);

            // Recovery if needed
            if (options.RecoverTruncatedActivityFiles && snapshot.HadDecodeFault)
            {
                var recoveryService = new FitSyntheticRecoveryService();
                var recovery = recoveryService.Recover(activity, sessions, laps, records, events, fileId);
                activity = recovery.Activity ?? activity;
                sessions = recovery.Sessions;
                laps = recovery.Laps;
                issues.AddRange(recovery.Issues);
                recoveryUsed = recovery.UsedSyntheticActivity || recovery.UsedSyntheticSessions;
            }

            // Grouping
            var groupingService = new FitActivityGroupingService();
            var grouped = groupingService.Group(sessions, laps, records, lengths, events);

            foreach (var warning in grouped.AmbiguityWarnings)
            {
                issues.Add(FitParseIssueFactory.GroupingAmbiguous(warning));
            }

            // Detect multi-sport and pool swim
            var isMultiSport = grouped.GroupedSessions.Count > 1
                && grouped.GroupedSessions.Any(s => s.Sport == FitSport.Transition);
            var hasPoolSwim = grouped.GroupedSessions.Any(s =>
                s.Sport == FitSport.Swimming && s.SubSport == FitSubSport.LapSwimming);

            activityContent = new FitActivityContent
            {
                Activity = activity,
                UserProfile = userProfile,
                ZonesTarget = zonesTarget,
                Sports = sportProfiles,
                Sessions = grouped.GroupedSessions,
                Events = events,
                HrvMessages = hrvMessages,
                HrMessages = hrMessages,
                SegmentLaps = segmentLaps,
                ClimbProMessages = [],
                IsMultiSport = isMultiSport,
                HasPoolSwim = hasPoolSwim,
            };
        }

        if (fileType == FitFileType.Workout)
        {
            workout = WorkoutAndCourseMapper.MapWorkout(snapshot.WorkoutMesgs, snapshot.WorkoutStepMesgs, devResolver);
        }

        if (fileType == FitFileType.Course)
        {
            course = WorkoutAndCourseMapper.MapCourse(snapshot.CourseMesgs, snapshot.CoursePointMesgs, devResolver);
        }

        if (fileType is FitFileType.MonitoringA or FitFileType.MonitoringDaily || options.IncludeMonitoringMessages)
        {
            var monitoringSamples = MonitoringMapper.MapMonitoring(snapshot.UnhandledMesgs, devResolver);
            if (monitoringSamples.Count > 0)
            {
                var monTimestamps = monitoringSamples
                    .Where(m => m.TimestampUtc.HasValue)
                    .Select(m => m.TimestampUtc!.Value)
                    .ToList();

                var monRange = new FitTimeRange
                {
                    StartTimeUtc = monTimestamps.Count > 0 ? monTimestamps.Min() : null,
                    EndTimeUtc = monTimestamps.Count > 0 ? monTimestamps.Max() : null,
                };
                monitoringContent = new FitMonitoringContent
                {
                    Range = monRange,
                    Samples = monitoringSamples,
                };
            }
        }

        // Monitoring integrity: decode faults in monitoring files are fatal
        if (fileType is FitFileType.MonitoringA or FitFileType.MonitoringDaily && snapshot.HadDecodeFault)
        {
            issues.Add(FitParseIssueFactory.MonitoringIntegrityFailed(
                snapshot.DecodeFaultMessage ?? "Decode fault in monitoring file."));
        }

        // Track unresolved developer fields
        if (devResolver is not null)
        {
            foreach (var (devIdx, fieldNum) in devResolver.UnresolvedFields)
            {
                issues.Add(FitParseIssueFactory.UnresolvedDeveloperField(devIdx, fieldNum));
            }
        }

        // 8. Assemble document
        var parseMetadata = BuildMetadata(snapshot, issues, fileType, activityContent, recoveryUsed);

        var fatalMonitoringError = issues
            .FirstOrDefault(i => i.Code == "fit.monitoring-integrity-failed" && i.Severity == FitParseIssueSeverity.Error);

        var document = new FitFileDocument
        {
            FileId = fileId,
            FileCreator = fileCreator,
            ParseMetadata = parseMetadata,
            ActivityContent = activityContent,
            Workout = workout,
            Course = course,
            MonitoringContent = monitoringContent,
            DeviceInfos = deviceInfos,
            DeveloperDataIds = devDataIds,
            FieldDescriptions = fieldDescs,
        };

        return new FitParseResult
        {
            Document = document,
            Metadata = parseMetadata,
            Issues = issues,
            FatalError = fatalMonitoringError is not null
                ? new FitParseError
                {
                    Code = fatalMonitoringError.Code,
                    Message = fatalMonitoringError.Message,
                    Phase = "Build",
                    Recoverable = false,
                }
                : null,
        };
    }

    private static FitParseMetadata BuildMetadata(
        FitDecodeSnapshot snapshot,
        List<FitParseIssue> issues,
        FitFileType? fileType,
        FitActivityContent? activityContent,
        bool recoveryUsed)
    {
        FitParseStatus status;
        if (issues.Any(i => i.Severity == FitParseIssueSeverity.Error))
            status = FitParseStatus.Failed;
        else if (snapshot.HadDecodeFault || issues.Count > 0)
            status = FitParseStatus.PartiallySucceeded;
        else
            status = FitParseStatus.Succeeded;

        // Derive timestamps from session data or record data
        DateTimeOffset? firstTimestamp = null;
        DateTimeOffset? lastTimestamp = null;

        if (activityContent is not null)
        {
            var allRecords = activityContent.Sessions
                .SelectMany(s => s.Records)
                .Where(r => r.TimestampUtc.HasValue)
                .Select(r => r.TimestampUtc!.Value)
                .ToList();

            if (allRecords.Count > 0)
            {
                firstTimestamp = allRecords.Min();
                lastTimestamp = allRecords.Max();
            }
            else if (activityContent.Sessions.Count > 0)
            {
                firstTimestamp = activityContent.Sessions
                    .Where(s => s.Range.StartTimeUtc.HasValue)
                    .Select(s => s.Range.StartTimeUtc!.Value)
                    .DefaultIfEmpty()
                    .Min();
                lastTimestamp = activityContent.Sessions
                    .Where(s => s.Range.EndTimeUtc.HasValue)
                    .Select(s => s.Range.EndTimeUtc!.Value)
                    .DefaultIfEmpty()
                    .Max();
                if (firstTimestamp == default) firstTimestamp = null;
                if (lastTimestamp == default) lastTimestamp = null;
            }
        }

        var recoveryMode = recoveryUsed ? FitRecoveryMode.SyntheticActivity : FitRecoveryMode.None;

        return new FitParseMetadata
        {
            Status = status,
            OrderingMode = FitMessageOrderingMode.Unknown,
            RecoveryMode = recoveryMode,
            IsPartial = snapshot.HadDecodeFault,
            TruncationDetected = snapshot.HadDecodeFault,
            HadDecodeFault = snapshot.HadDecodeFault,
            IsMultiSport = activityContent?.IsMultiSport ?? false,
            HasPoolSwim = activityContent?.HasPoolSwim ?? false,
            HasDeveloperFields = snapshot.DeveloperDataIdMesgs.Count > 0,
            DecodedMessageCount = snapshot.TotalMessageCount,
            DroppedMessageCount = 0,
            FirstTimestampUtc = firstTimestamp,
            LastTimestampUtc = lastTimestamp,
        };
    }

    private static FitParseError BuildFatalError(FitValidationResult validation)
    {
        var firstError = validation.Issues.FirstOrDefault(i => i.Severity == FitParseIssueSeverity.Error);

        return new FitParseError
        {
            Code = firstError?.Code ?? "FIT_VALIDATION_FAILED",
            Message = firstError?.Message ?? "Validation failed with blocking errors.",
            Phase = "Validation",
            Recoverable = false,
        };
    }
}
