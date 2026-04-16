using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Summaries;

namespace FitToMarkdown.Markdown.Projection;

internal static class TokenBudgetEstimator
{
    private const int CharsPerToken = 4;

    internal static TokenBudgetResult Evaluate(
        MarkdownDocumentOptions options,
        IReadOnlyList<SectionRenderDecision> decisions,
        IReadOnlyList<SessionSection> sessions)
    {
        if (options.ApproximateTokenBudget <= 0)
        {
            return new TokenBudgetResult
            {
                NeedsCompaction = false,
                CompactedDecisions = decisions,
                CompactedSessionSections = sessions,
            };
        }

        int estimatedTokens = EstimateTotalTokens(decisions, sessions);

        if (estimatedTokens <= options.ApproximateTokenBudget)
        {
            return new TokenBudgetResult
            {
                NeedsCompaction = false,
                CompactedDecisions = decisions,
                CompactedSessionSections = sessions,
            };
        }

        // Apply compaction in priority order
        var compactedDecisions = new List<SectionRenderDecision>(decisions);
        var compactedSessions = sessions.Select(s => s).ToList();

        // Step 1: Omit per-lap detail
        estimatedTokens = CompactLapDetails(compactedDecisions, compactedSessions, estimatedTokens, options.ApproximateTokenBudget);
        if (estimatedTokens <= options.ApproximateTokenBudget)
        {
            return BuildResult(compactedDecisions, compactedSessions);
        }

        // Step 2: Reduce sample rows
        estimatedTokens = CompactRecordSamples(compactedDecisions, compactedSessions, estimatedTokens, options.ApproximateTokenBudget);
        if (estimatedTokens <= options.ApproximateTokenBudget)
        {
            return BuildResult(compactedDecisions, compactedSessions);
        }

        // Step 3: Omit global summary when session-local exists
        estimatedTokens = CompactDuplicateGlobalSummary(compactedDecisions, compactedSessions, estimatedTokens);
        if (estimatedTokens <= options.ApproximateTokenBudget)
        {
            return BuildResult(compactedDecisions, compactedSessions);
        }

        // Step 4: Compact column sets — mark record time series as omitted
        CompactTimeSeries(compactedDecisions, compactedSessions);

        return BuildResult(compactedDecisions, compactedSessions);
    }

    private static int EstimateTotalTokens(
        IReadOnlyList<SectionRenderDecision> decisions,
        IReadOnlyList<SessionSection> sessions)
    {
        int totalChars = 0;

        foreach (var decision in decisions)
        {
            if (!decision.ShouldRender) continue;
            totalChars += EstimateSectionCharacters(decision, sessions);
        }

        foreach (var session in sessions)
        {
            foreach (var sd in session.SectionDecisions)
            {
                if (!sd.ShouldRender) continue;
                totalChars += EstimateSessionSectionCharacters(sd, session);
            }
        }

        return (totalChars + CharsPerToken - 1) / CharsPerToken;
    }

    private static int EstimateSectionCharacters(SectionRenderDecision decision, IReadOnlyList<SessionSection> sessions)
    {
        int items = decision.ItemCount ?? 1;

        return decision.Section switch
        {
            FitMarkdownSectionKey.Frontmatter => 300,
            FitMarkdownSectionKey.Overview => 250,
            FitMarkdownSectionKey.SessionSummary => 150 * items,
            FitMarkdownSectionKey.SessionDetails => 200 * items,
            FitMarkdownSectionKey.LapDetails => 80 * items,
            FitMarkdownSectionKey.LengthDetails => 60 * items,
            FitMarkdownSectionKey.RecordSummary => 100 + (40 * items),
            FitMarkdownSectionKey.RecordTimeSeries => 50 * items,
            FitMarkdownSectionKey.HeartRateZones => 100 + (30 * items),
            FitMarkdownSectionKey.HrvData => 200,
            FitMarkdownSectionKey.Devices => 80 * items,
            FitMarkdownSectionKey.Events => 40 * items,
            FitMarkdownSectionKey.DeveloperFields => 60 * items,
            FitMarkdownSectionKey.Workout => 50 * items,
            FitMarkdownSectionKey.Course => 50 * items,
            FitMarkdownSectionKey.SegmentLaps => 60 * items,
            FitMarkdownSectionKey.UserProfile => 150,
            FitMarkdownSectionKey.DataQuality => 0,
            _ => 100,
        };
    }

    private static int EstimateSessionSectionCharacters(SectionRenderDecision decision, SessionSection session)
    {
        int items = decision.ItemCount ?? 1;

        return decision.Section switch
        {
            FitMarkdownSectionKey.LapDetails => 80 * items,
            FitMarkdownSectionKey.LengthDetails => 60 * items,
            FitMarkdownSectionKey.RecordSummary => 100 + (40 * items),
            FitMarkdownSectionKey.RecordTimeSeries => 50 * items,
            FitMarkdownSectionKey.SessionDetails => 200,
            _ => 50,
        };
    }

    private static int CompactLapDetails(
        List<SectionRenderDecision> decisions,
        List<SessionSection> sessions,
        int currentTokens,
        int budget)
    {
        // Omit document-level lap details
        for (int i = 0; i < decisions.Count; i++)
        {
            if (decisions[i].Section == FitMarkdownSectionKey.LapDetails && decisions[i].ShouldRender)
            {
                int saved = EstimateSectionCharacters(decisions[i], sessions) / CharsPerToken;
                decisions[i] = decisions[i] with
                {
                    ShouldRender = false,
                    OmissionReason = SectionDecisionProjector.OmissionReasons.TokenBudgetCompact,
                };
                currentTokens -= saved;
            }
        }

        // Omit session-level lap details
        for (int s = 0; s < sessions.Count; s++)
        {
            var sessionDecisions = sessions[s].SectionDecisions.ToList();
            for (int i = 0; i < sessionDecisions.Count; i++)
            {
                if (sessionDecisions[i].Section == FitMarkdownSectionKey.LapDetails && sessionDecisions[i].ShouldRender)
                {
                    int saved = EstimateSessionSectionCharacters(sessionDecisions[i], sessions[s]) / CharsPerToken;
                    sessionDecisions[i] = sessionDecisions[i] with
                    {
                        ShouldRender = false,
                        OmissionReason = SectionDecisionProjector.OmissionReasons.TokenBudgetCompact,
                    };
                    currentTokens -= saved;
                }
            }
            sessions[s] = sessions[s] with { SectionDecisions = sessionDecisions };
        }

        return currentTokens;
    }

    private static int CompactRecordSamples(
        List<SectionRenderDecision> decisions,
        List<SessionSection> sessions,
        int currentTokens,
        int budget)
    {
        // Halve sample rows by reconstructing sessions with truncated samples
        for (int s = 0; s < sessions.Count; s++)
        {
            var samples = sessions[s].RecordSamples;
            if (samples.Count > 2)
            {
                int newCount = samples.Count / 2;
                int savedChars = (samples.Count - newCount) * 50;
                sessions[s] = sessions[s] with
                {
                    RecordSamples = samples.Take(newCount).ToList(),
                };
                currentTokens -= savedChars / CharsPerToken;
            }
        }

        // Update document-level record time series item count
        for (int i = 0; i < decisions.Count; i++)
        {
            if (decisions[i].Section == FitMarkdownSectionKey.RecordTimeSeries && decisions[i].ShouldRender)
            {
                int totalSamples = sessions.Sum(session => session.RecordSamples.Count);
                decisions[i] = decisions[i] with { ItemCount = totalSamples };
            }
        }

        return currentTokens;
    }

    private static int CompactDuplicateGlobalSummary(
        List<SectionRenderDecision> decisions,
        List<SessionSection> sessions,
        int currentTokens)
    {
        bool hasSessionLocal = sessions.Any(s => s.RecordSummary is not null);
        if (!hasSessionLocal) return currentTokens;

        for (int i = 0; i < decisions.Count; i++)
        {
            if (decisions[i].Section == FitMarkdownSectionKey.RecordSummary && decisions[i].ShouldRender)
            {
                int saved = EstimateSectionCharacters(decisions[i], sessions) / CharsPerToken;
                decisions[i] = decisions[i] with
                {
                    ShouldRender = false,
                    OmissionReason = SectionDecisionProjector.OmissionReasons.DuplicateGlobalSummary,
                };
                currentTokens -= saved;
            }
        }

        return currentTokens;
    }

    private static void CompactTimeSeries(
        List<SectionRenderDecision> decisions,
        List<SessionSection> sessions)
    {
        for (int i = 0; i < decisions.Count; i++)
        {
            if (decisions[i].Section == FitMarkdownSectionKey.RecordTimeSeries && decisions[i].ShouldRender)
            {
                decisions[i] = decisions[i] with
                {
                    ShouldRender = false,
                    OmissionReason = SectionDecisionProjector.OmissionReasons.TokenBudgetCompact,
                };
            }
        }

        for (int s = 0; s < sessions.Count; s++)
        {
            var sessionDecisions = sessions[s].SectionDecisions.ToList();
            for (int i = 0; i < sessionDecisions.Count; i++)
            {
                if (sessionDecisions[i].Section == FitMarkdownSectionKey.RecordTimeSeries && sessionDecisions[i].ShouldRender)
                {
                    sessionDecisions[i] = sessionDecisions[i] with
                    {
                        ShouldRender = false,
                        OmissionReason = SectionDecisionProjector.OmissionReasons.TokenBudgetCompact,
                    };
                }
            }
            sessions[s] = sessions[s] with { SectionDecisions = sessionDecisions };
        }
    }

    private static TokenBudgetResult BuildResult(
        List<SectionRenderDecision> decisions,
        List<SessionSection> sessions)
    {
        return new TokenBudgetResult
        {
            NeedsCompaction = true,
            CompactedDecisions = decisions,
            CompactedSessionSections = sessions,
        };
    }
}

internal readonly struct TokenBudgetResult
{
    internal bool NeedsCompaction { get; init; }
    internal IReadOnlyList<SectionRenderDecision> CompactedDecisions { get; init; }
    internal IReadOnlyList<SessionSection> CompactedSessionSections { get; init; }
}
