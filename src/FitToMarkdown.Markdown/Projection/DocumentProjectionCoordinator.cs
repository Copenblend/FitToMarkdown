using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Summaries;

namespace FitToMarkdown.Markdown.Projection;

internal sealed class DocumentProjectionCoordinator
{
    internal FitMarkdownDocument Project(FitFileDocument document, MarkdownDocumentOptions options, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // 1. Project frontmatter
        var frontmatter = FrontmatterProjector.Project(document);

        // 2. Project overview
        var (title, headingTimestamp, overview) = OverviewProjector.Project(document);

        // 3. Project session sections
        IReadOnlyList<SessionSection> sessionSections = SessionSectionProjector.Project(document, options);

        // 4. Project global record summary (fallback when no session-local)
        var globalRecordSummary = sessionSections.Count == 0 || sessionSections.All(s => s.RecordSummary is null)
            ? RecordSummaryProjector.ProjectGlobal(document)
            : null;

        // 5. Project global record samples (fallback)
        IReadOnlyList<SampledTimeSeriesRow> globalRecordSamples = sessionSections.Count == 0 || sessionSections.All(s => s.RecordSamples.Count == 0)
            ? TimeSeriesSampler.SampleGlobal(document, options)
            : [];

        cancellationToken.ThrowIfCancellationRequested();

        // 6. Project supplementary
        var heartRateZones = HeartRateZoneProjector.Project(document);
        var hrvSummary = HrvProjector.Project(document);
        var devices = DeviceProjector.Project(document);
        var events = EventProjector.Project(document);
        var workoutSteps = WorkoutProjector.Project(document);
        var coursePoints = CourseProjector.Project(document);
        var segmentLapRows = SegmentLapProjector.Project(document);
        var developerFieldGroups = options.IncludeDeveloperFields
            ? DeveloperFieldProjector.Project(document)
            : (IReadOnlyList<DeveloperFieldGroup>)[];

        cancellationToken.ThrowIfCancellationRequested();

        // 7. Section decisions
        var sectionDecisions = SectionDecisionProjector.ProjectDocumentLevel(
            options, document, frontmatter, overview, sessionSections,
            globalRecordSummary, globalRecordSamples,
            heartRateZones, hrvSummary, devices, events,
            developerFieldGroups, workoutSteps, coursePoints, segmentLapRows);

        // 8. Token budget compaction
        var budgetResult = TokenBudgetEstimator.Evaluate(options, sectionDecisions, sessionSections);
        if (budgetResult.NeedsCompaction)
        {
            sectionDecisions = budgetResult.CompactedDecisions;
            sessionSections = budgetResult.CompactedSessionSections;
        }

        return new FitMarkdownDocument
        {
            Source = document,
            Title = title,
            HeadingTimestampUtc = headingTimestamp,
            Frontmatter = frontmatter,
            Overview = overview,
            SessionSections = sessionSections,
            GlobalRecordSummary = globalRecordSummary,
            GlobalRecordSamples = globalRecordSamples,
            HeartRateZones = heartRateZones,
            HrvSummary = hrvSummary,
            Devices = devices,
            Events = events,
            WorkoutSteps = workoutSteps,
            CoursePoints = coursePoints,
            SegmentLapRows = segmentLapRows,
            DeveloperFieldGroups = developerFieldGroups,
            SectionDecisions = sectionDecisions,
        };
    }
}
