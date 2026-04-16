using FluentAssertions;
using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Summaries;
using FitToMarkdown.Markdown.Projection;
using FitToMarkdown.Markdown.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Markdown.Tests;

public sealed class TokenBudgetEstimatorTests
{
    private static IReadOnlyList<SectionRenderDecision> CreateRenderAllDecisions()
    {
        return
        [
            new SectionRenderDecision { Section = FitMarkdownSectionKey.Frontmatter, Order = 0, ShouldRender = true },
            new SectionRenderDecision { Section = FitMarkdownSectionKey.Overview, Order = 1, ShouldRender = true },
            new SectionRenderDecision { Section = FitMarkdownSectionKey.SessionSummary, Order = 2, ShouldRender = true, ItemCount = 1 },
            new SectionRenderDecision { Section = FitMarkdownSectionKey.SessionDetails, Order = 3, ShouldRender = true, ItemCount = 1 },
            new SectionRenderDecision { Section = FitMarkdownSectionKey.LapDetails, Order = 4, ShouldRender = true, ItemCount = 2 },
            new SectionRenderDecision { Section = FitMarkdownSectionKey.LengthDetails, Order = 5, ShouldRender = false },
            new SectionRenderDecision { Section = FitMarkdownSectionKey.RecordSummary, Order = 6, ShouldRender = true, ItemCount = 3 },
            new SectionRenderDecision { Section = FitMarkdownSectionKey.RecordTimeSeries, Order = 7, ShouldRender = true, ItemCount = 50 },
            new SectionRenderDecision { Section = FitMarkdownSectionKey.HeartRateZones, Order = 8, ShouldRender = false },
            new SectionRenderDecision { Section = FitMarkdownSectionKey.HrvData, Order = 9, ShouldRender = false },
            new SectionRenderDecision { Section = FitMarkdownSectionKey.Devices, Order = 10, ShouldRender = true, ItemCount = 1 },
            new SectionRenderDecision { Section = FitMarkdownSectionKey.Events, Order = 11, ShouldRender = true, ItemCount = 3 },
            new SectionRenderDecision { Section = FitMarkdownSectionKey.DeveloperFields, Order = 12, ShouldRender = false },
            new SectionRenderDecision { Section = FitMarkdownSectionKey.Workout, Order = 13, ShouldRender = false },
            new SectionRenderDecision { Section = FitMarkdownSectionKey.Course, Order = 14, ShouldRender = false },
            new SectionRenderDecision { Section = FitMarkdownSectionKey.SegmentLaps, Order = 15, ShouldRender = false },
            new SectionRenderDecision { Section = FitMarkdownSectionKey.UserProfile, Order = 16, ShouldRender = false },
            new SectionRenderDecision { Section = FitMarkdownSectionKey.DataQuality, Order = 17, ShouldRender = false },
        ];
    }

    [Fact]
    public void Evaluate_should_skip_compaction_when_no_budget()
    {
        var options = MarkdownTestFixtures.CreateDefaultOptions() with { ApproximateTokenBudget = 0 };
        var decisions = CreateRenderAllDecisions();
        var sessions = new List<SessionSection>
        {
            new()
            {
                Session = new Core.Models.FitSession
                {
                    SessionIndex = 0,
                    Sport = FitSport.Running,
                },
                LapRows = [new LapTableRow { LapNumber = 1 }],
                SectionDecisions = [new SectionRenderDecision { Section = FitMarkdownSectionKey.LapDetails, ShouldRender = true }],
            },
        };

        var result = TokenBudgetEstimator.Evaluate(options, decisions, sessions);

        result.NeedsCompaction.Should().BeFalse();
        result.CompactedDecisions.Should().BeSameAs(decisions);
    }

    [Fact]
    public void Evaluate_should_skip_compaction_when_under_budget()
    {
        // Large budget should not trigger compaction
        var options = MarkdownTestFixtures.CreateDefaultOptions() with { ApproximateTokenBudget = 100_000 };
        var decisions = CreateRenderAllDecisions();
        var sessions = new List<SessionSection>
        {
            new()
            {
                Session = new Core.Models.FitSession
                {
                    SessionIndex = 0,
                    Sport = FitSport.Running,
                },
                LapRows = [new LapTableRow { LapNumber = 1 }, new LapTableRow { LapNumber = 2 }],
                SectionDecisions = [new SectionRenderDecision { Section = FitMarkdownSectionKey.LapDetails, ShouldRender = true }],
            },
        };

        var result = TokenBudgetEstimator.Evaluate(options, decisions, sessions);

        result.NeedsCompaction.Should().BeFalse();
    }

    [Fact]
    public void Evaluate_should_compact_lap_details_first()
    {
        // Very small budget to force compaction
        var options = MarkdownTestFixtures.CreateDefaultOptions() with { ApproximateTokenBudget = 10 };
        var decisions = CreateRenderAllDecisions();
        var sessions = new List<SessionSection>
        {
            new()
            {
                Session = new Core.Models.FitSession
                {
                    SessionIndex = 0,
                    Sport = FitSport.Running,
                },
                LapRows = [new LapTableRow { LapNumber = 1 }, new LapTableRow { LapNumber = 2 }],
                SectionDecisions =
                [
                    new SectionRenderDecision { Section = FitMarkdownSectionKey.SessionDetails, ShouldRender = true },
                    new SectionRenderDecision { Section = FitMarkdownSectionKey.LapDetails, ShouldRender = true, ItemCount = 2 },
                    new SectionRenderDecision { Section = FitMarkdownSectionKey.RecordSummary, ShouldRender = true },
                    new SectionRenderDecision { Section = FitMarkdownSectionKey.RecordTimeSeries, ShouldRender = true, ItemCount = 50 },
                ],
            },
        };

        var result = TokenBudgetEstimator.Evaluate(options, decisions, sessions);

        result.NeedsCompaction.Should().BeTrue();
    }
}
