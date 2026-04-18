using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Summaries;

namespace FitToMarkdown.Markdown.Projection;

internal static class SectionDecisionProjector
{
    internal static IReadOnlyList<SectionRenderDecision> ProjectDocumentLevel(
        MarkdownDocumentOptions options,
        FitFileDocument document,
        FitFrontmatter frontmatter,
        FitOverviewMetrics overview,
        IReadOnlyList<SessionSection> sessions,
        RecordStatisticsSummary? globalRecordSummary,
        IReadOnlyList<SampledTimeSeriesRow> globalRecordSamples,
        HeartRateZoneSummary? heartRateZones,
        HrvSummary? hrvSummary,
        IReadOnlyList<DeviceSummaryRow> devices,
        IReadOnlyList<EventTimelineItem> events,
        IReadOnlyList<DeveloperFieldGroup> developerFieldGroups,
        IReadOnlyList<WorkoutStepRow> workoutSteps,
        IReadOnlyList<CoursePointRow> coursePoints,
        IReadOnlyList<SegmentLapTableRow> segmentLapRows)
    {
        var decisions = new List<SectionRenderDecision>();
        int order = 0;

        bool isMonitoring = document.ActivityContent is null && document.MonitoringContent is not null;

        // Frontmatter
        decisions.Add(DecideFrontmatter(ref order, options, frontmatter));

        // Overview
        decisions.Add(DecideOverview(ref order, overview));

        // SessionSummary
        decisions.Add(isMonitoring
            ? ExcludeForMonitoring(ref order, FitMarkdownSectionKey.SessionSummary)
            : DecideSessionSummary(ref order, sessions));

        // SessionDetails — document-level aggregate; individual decisions are per-session
        decisions.Add(isMonitoring
            ? ExcludeForMonitoring(ref order, FitMarkdownSectionKey.SessionDetails)
            : DecideSessionDetails(ref order, sessions));

        // LapDetails — document-level aggregate
        decisions.Add(isMonitoring
            ? ExcludeForMonitoring(ref order, FitMarkdownSectionKey.LapDetails)
            : DecideLapDetails(ref order, options, sessions));

        // LengthDetails
        decisions.Add(isMonitoring
            ? ExcludeForMonitoring(ref order, FitMarkdownSectionKey.LengthDetails)
            : DecideLengthDetails(ref order, sessions));

        // RecordSummary
        decisions.Add(isMonitoring
            ? ExcludeForMonitoring(ref order, FitMarkdownSectionKey.RecordSummary)
            : DecideRecordSummary(ref order, globalRecordSummary, sessions));

        // RecordTimeSeries
        decisions.Add(isMonitoring
            ? ExcludeForMonitoring(ref order, FitMarkdownSectionKey.RecordTimeSeries)
            : DecideRecordTimeSeries(ref order, options, globalRecordSamples, sessions));

        // HeartRateZones
        decisions.Add(isMonitoring
            ? ExcludeForMonitoring(ref order, FitMarkdownSectionKey.HeartRateZones)
            : DecideByPresence(ref order, FitMarkdownSectionKey.HeartRateZones, heartRateZones is not null, heartRateZones?.Zones.Count));

        // HrvData
        decisions.Add(isMonitoring
            ? ExcludeForMonitoring(ref order, FitMarkdownSectionKey.HrvData)
            : DecideByPresence(ref order, FitMarkdownSectionKey.HrvData, hrvSummary is not null && hrvSummary.SampleCount > 0, hrvSummary?.SampleCount));

        // Devices
        decisions.Add(DecideByPresence(ref order, FitMarkdownSectionKey.Devices, devices.Count > 0, devices.Count));

        // Events
        decisions.Add(DecideByPresence(ref order, FitMarkdownSectionKey.Events, events.Count > 0, events.Count));

        // DeveloperFields
        decisions.Add(DecideDeveloperFields(ref order, options, developerFieldGroups));

        // Workout
        decisions.Add(DecideByPresence(ref order, FitMarkdownSectionKey.Workout, workoutSteps.Count > 0, workoutSteps.Count));

        // Course
        decisions.Add(DecideByPresence(ref order, FitMarkdownSectionKey.Course, coursePoints.Count > 0, coursePoints.Count));

        // SegmentLaps
        decisions.Add(DecideByPresence(ref order, FitMarkdownSectionKey.SegmentLaps, segmentLapRows.Count > 0, segmentLapRows.Count));

        // UserProfile
        decisions.Add(DecideUserProfile(ref order, document));

        // DataQuality — always omitted
        decisions.Add(new SectionRenderDecision
        {
            Section = FitMarkdownSectionKey.DataQuality,
            Order = order++,
            ShouldRender = false,
            ItemCount = null,
            OmissionReason = OmissionReasons.UnsupportedCurrentContract,
        });

        return decisions;
    }

    internal static IReadOnlyList<SectionRenderDecision> ProjectSessionLevel(
        MarkdownDocumentOptions options,
        SessionSection session,
        int sessionIndex)
    {
        var decisions = new List<SectionRenderDecision>();
        int order = 0;

        // SessionDetails — always render for a session that exists
        decisions.Add(new SectionRenderDecision
        {
            Section = FitMarkdownSectionKey.SessionDetails,
            Order = order++,
            ShouldRender = true,
            ItemCount = 1,
        });

        // LapDetails
        decisions.Add(DecideSessionLapDetails(ref order, options, session));

        // LengthDetails
        bool hasLengths = session.LengthRows.Count > 0;
        decisions.Add(new SectionRenderDecision
        {
            Section = FitMarkdownSectionKey.LengthDetails,
            Order = order++,
            ShouldRender = hasLengths,
            ItemCount = session.LengthRows.Count,
            OmissionReason = hasLengths ? null : OmissionReasons.NoData,
        });

        // RecordSummary
        bool hasRecordSummary = session.RecordSummary is not null;
        decisions.Add(new SectionRenderDecision
        {
            Section = FitMarkdownSectionKey.RecordSummary,
            Order = order++,
            ShouldRender = hasRecordSummary,
            ItemCount = session.RecordSummary?.Metrics.Count,
            OmissionReason = hasRecordSummary ? null : OmissionReasons.NoData,
        });

        // RecordTimeSeries
        bool hasSamples = session.RecordSamples.Count > 0 && options.IncludeRecordSamples;
        decisions.Add(new SectionRenderDecision
        {
            Section = FitMarkdownSectionKey.RecordTimeSeries,
            Order = order++,
            ShouldRender = hasSamples,
            ItemCount = session.RecordSamples.Count,
            OmissionReason = hasSamples
                ? null
                : (!options.IncludeRecordSamples ? OmissionReasons.DisabledByOptions : OmissionReasons.NoData),
        });

        return decisions;
    }

    private static SectionRenderDecision DecideFrontmatter(ref int order, MarkdownDocumentOptions options, FitFrontmatter frontmatter)
    {
        if (!options.IncludeFrontmatter)
        {
            return new SectionRenderDecision
            {
                Section = FitMarkdownSectionKey.Frontmatter,
                Order = order++,
                ShouldRender = false,
                OmissionReason = OmissionReasons.DisabledByOptions,
            };
        }

        bool hasData = frontmatter.FileType is not null;
        return new SectionRenderDecision
        {
            Section = FitMarkdownSectionKey.Frontmatter,
            Order = order++,
            ShouldRender = hasData,
            OmissionReason = hasData ? null : OmissionReasons.NoData,
        };
    }

    private static SectionRenderDecision DecideOverview(ref int order, FitOverviewMetrics overview)
    {
        bool hasData = overview.PrimarySport is not null
            || overview.TotalDistanceMeters is not null
            || overview.TotalElapsedTime is not null;

        return new SectionRenderDecision
        {
            Section = FitMarkdownSectionKey.Overview,
            Order = order++,
            ShouldRender = hasData,
            OmissionReason = hasData ? null : OmissionReasons.NoData,
        };
    }

    private static SectionRenderDecision DecideSessionSummary(ref int order, IReadOnlyList<SessionSection> sessions)
    {
        bool hasData = sessions.Count > 0;
        return new SectionRenderDecision
        {
            Section = FitMarkdownSectionKey.SessionSummary,
            Order = order++,
            ShouldRender = hasData,
            ItemCount = sessions.Count,
            OmissionReason = hasData ? null : OmissionReasons.NoData,
        };
    }

    private static SectionRenderDecision DecideSessionDetails(ref int order, IReadOnlyList<SessionSection> sessions)
    {
        bool hasData = sessions.Count > 0;
        return new SectionRenderDecision
        {
            Section = FitMarkdownSectionKey.SessionDetails,
            Order = order++,
            ShouldRender = hasData,
            ItemCount = sessions.Count,
            OmissionReason = hasData ? null : OmissionReasons.NoData,
        };
    }

    private static SectionRenderDecision DecideLapDetails(ref int order, MarkdownDocumentOptions options, IReadOnlyList<SessionSection> sessions)
    {
        int totalLaps = sessions.Sum(s => s.LapRows.Count);

        if (totalLaps == 0)
        {
            return new SectionRenderDecision
            {
                Section = FitMarkdownSectionKey.LapDetails,
                Order = order++,
                ShouldRender = false,
                ItemCount = 0,
                OmissionReason = OmissionReasons.NoData,
            };
        }

        // If all sessions have exactly 1 lap and collapse is enabled
        if (options.CollapseSingleLapSections && sessions.All(s => s.LapRows.Count <= 1))
        {
            return new SectionRenderDecision
            {
                Section = FitMarkdownSectionKey.LapDetails,
                Order = order++,
                ShouldRender = false,
                ItemCount = totalLaps,
                OmissionReason = OmissionReasons.SingleLapCollapsed,
            };
        }

        return new SectionRenderDecision
        {
            Section = FitMarkdownSectionKey.LapDetails,
            Order = order++,
            ShouldRender = true,
            ItemCount = totalLaps,
        };
    }

    private static SectionRenderDecision DecideLengthDetails(ref int order, IReadOnlyList<SessionSection> sessions)
    {
        int totalLengths = sessions.Sum(s => s.LengthRows.Count);
        bool hasData = totalLengths > 0;

        return new SectionRenderDecision
        {
            Section = FitMarkdownSectionKey.LengthDetails,
            Order = order++,
            ShouldRender = hasData,
            ItemCount = totalLengths,
            OmissionReason = hasData ? null : OmissionReasons.NoData,
        };
    }

    private static SectionRenderDecision DecideRecordSummary(
        ref int order,
        RecordStatisticsSummary? globalRecordSummary,
        IReadOnlyList<SessionSection> sessions)
    {
        bool hasGlobal = globalRecordSummary is not null;
        bool hasSessionLocal = sessions.Any(s => s.RecordSummary is not null);

        if (!hasGlobal && !hasSessionLocal)
        {
            return new SectionRenderDecision
            {
                Section = FitMarkdownSectionKey.RecordSummary,
                Order = order++,
                ShouldRender = false,
                ItemCount = 0,
                OmissionReason = OmissionReasons.NoData,
            };
        }

        // If session-local summaries exist and global also exists, the global is a duplicate
        if (hasGlobal && hasSessionLocal)
        {
            return new SectionRenderDecision
            {
                Section = FitMarkdownSectionKey.RecordSummary,
                Order = order++,
                ShouldRender = true,
                ItemCount = globalRecordSummary!.Metrics.Count,
                OmissionReason = null,
            };
        }

        int itemCount = hasGlobal
            ? globalRecordSummary!.Metrics.Count
            : sessions.Where(s => s.RecordSummary is not null).Sum(s => s.RecordSummary!.Metrics.Count);

        return new SectionRenderDecision
        {
            Section = FitMarkdownSectionKey.RecordSummary,
            Order = order++,
            ShouldRender = true,
            ItemCount = itemCount,
        };
    }

    private static SectionRenderDecision DecideRecordTimeSeries(
        ref int order,
        MarkdownDocumentOptions options,
        IReadOnlyList<SampledTimeSeriesRow> globalRecordSamples,
        IReadOnlyList<SessionSection> sessions)
    {
        if (!options.IncludeRecordSamples)
        {
            return new SectionRenderDecision
            {
                Section = FitMarkdownSectionKey.RecordTimeSeries,
                Order = order++,
                ShouldRender = false,
                OmissionReason = OmissionReasons.DisabledByOptions,
            };
        }

        int totalSamples = globalRecordSamples.Count + sessions.Sum(s => s.RecordSamples.Count);
        bool hasData = totalSamples > 0;

        return new SectionRenderDecision
        {
            Section = FitMarkdownSectionKey.RecordTimeSeries,
            Order = order++,
            ShouldRender = hasData,
            ItemCount = totalSamples,
            OmissionReason = hasData ? null : OmissionReasons.NoData,
        };
    }

    private static SectionRenderDecision DecideByPresence(ref int order, FitMarkdownSectionKey key, bool hasData, int? itemCount)
    {
        return new SectionRenderDecision
        {
            Section = key,
            Order = order++,
            ShouldRender = hasData,
            ItemCount = itemCount,
            OmissionReason = hasData ? null : OmissionReasons.NoData,
        };
    }

    private static SectionRenderDecision ExcludeForMonitoring(ref int order, FitMarkdownSectionKey key)
    {
        return new SectionRenderDecision
        {
            Section = key,
            Order = order++,
            ShouldRender = false,
            ItemCount = 0,
            OmissionReason = OmissionReasons.MonitoringFileType,
        };
    }

    private static SectionRenderDecision DecideDeveloperFields(
        ref int order,
        MarkdownDocumentOptions options,
        IReadOnlyList<DeveloperFieldGroup> groups)
    {
        if (!options.IncludeDeveloperFields)
        {
            return new SectionRenderDecision
            {
                Section = FitMarkdownSectionKey.DeveloperFields,
                Order = order++,
                ShouldRender = false,
                OmissionReason = OmissionReasons.DisabledByOptions,
            };
        }

        bool hasData = groups.Count > 0;
        return new SectionRenderDecision
        {
            Section = FitMarkdownSectionKey.DeveloperFields,
            Order = order++,
            ShouldRender = hasData,
            ItemCount = groups.Count,
            OmissionReason = hasData ? null : OmissionReasons.NoData,
        };
    }

    private static SectionRenderDecision DecideUserProfile(ref int order, FitFileDocument document)
    {
        bool hasData = document.ActivityContent?.UserProfile is not null;
        return new SectionRenderDecision
        {
            Section = FitMarkdownSectionKey.UserProfile,
            Order = order++,
            ShouldRender = hasData,
            ItemCount = hasData ? 1 : 0,
            OmissionReason = hasData ? null : OmissionReasons.NoData,
        };
    }

    private static SectionRenderDecision DecideSessionLapDetails(
        ref int order,
        MarkdownDocumentOptions options,
        SessionSection session)
    {
        int lapCount = session.LapRows.Count;

        if (lapCount == 0)
        {
            return new SectionRenderDecision
            {
                Section = FitMarkdownSectionKey.LapDetails,
                Order = order++,
                ShouldRender = false,
                ItemCount = 0,
                OmissionReason = OmissionReasons.NoData,
            };
        }

        if (options.CollapseSingleLapSections && lapCount == 1)
        {
            return new SectionRenderDecision
            {
                Section = FitMarkdownSectionKey.LapDetails,
                Order = order++,
                ShouldRender = false,
                ItemCount = 1,
                OmissionReason = OmissionReasons.SingleLapCollapsed,
            };
        }

        return new SectionRenderDecision
        {
            Section = FitMarkdownSectionKey.LapDetails,
            Order = order++,
            ShouldRender = true,
            ItemCount = lapCount,
        };
    }

    internal static class OmissionReasons
    {
        internal const string NoData = "no-data";
        internal const string DisabledByOptions = "disabled-by-options";
        internal const string SingleLapCollapsed = "single-lap-collapsed";
        internal const string LapDetailThresholdExceeded = "lap-detail-threshold-exceeded";
        internal const string DuplicateGlobalSummary = "duplicate-global-summary";
        internal const string TokenBudgetCompact = "token-budget-compact";
        internal const string MonitoringFileType = "monitoring-file-type";
        internal const string UnsupportedCurrentContract = "unsupported-current-contract";
    }
}
