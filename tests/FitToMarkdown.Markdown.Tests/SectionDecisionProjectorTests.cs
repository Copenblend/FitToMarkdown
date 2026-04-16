using FluentAssertions;
using FitToMarkdown.Core.Abstractions;
using FitToMarkdown.Core.Documents;
using FitToMarkdown.Core.Enums;
using FitToMarkdown.Core.Summaries;
using FitToMarkdown.Markdown.Projection;
using FitToMarkdown.Markdown.Tests.TestHelpers;
using Xunit;

namespace FitToMarkdown.Markdown.Tests;

public sealed class SectionDecisionProjectorTests
{
    private static (MarkdownDocumentOptions Options, FitFileDocument Document, FitFrontmatter Frontmatter,
        FitOverviewMetrics Overview, IReadOnlyList<SessionSection> Sessions) BuildMinimalContext(
        MarkdownDocumentOptions? options = null)
    {
        var doc = MarkdownTestFixtures.CreateStandardRunningActivity();
        var fm = FrontmatterProjector.Project(doc);
        var (_, _, overview) = OverviewProjector.Project(doc);
        var opts = options ?? MarkdownTestFixtures.CreateDefaultOptions();

        var sessions = new List<SessionSection>
        {
            new()
            {
                Session = doc.ActivityContent!.Sessions[0],
                LapRows =
                [
                    new LapTableRow { LapNumber = 1, DistanceMeters = 3000 },
                    new LapTableRow { LapNumber = 2, DistanceMeters = 3200 },
                ],
            },
        };

        return (opts, doc, fm, overview, sessions);
    }

    [Fact]
    public void ProjectDocumentLevel_should_include_frontmatter_when_enabled()
    {
        var (opts, doc, fm, overview, sessions) = BuildMinimalContext();

        var decisions = SectionDecisionProjector.ProjectDocumentLevel(
            opts, doc, fm, overview, sessions, null, [], null, null, [], [], [], [], [], []);

        var fmDecision = decisions.First(d => d.Section == FitMarkdownSectionKey.Frontmatter);
        fmDecision.ShouldRender.Should().BeTrue();
    }

    [Fact]
    public void ProjectDocumentLevel_should_omit_frontmatter_when_disabled()
    {
        var opts = MarkdownTestFixtures.CreateDefaultOptions() with { IncludeFrontmatter = false };
        var (_, doc, fm, overview, sessions) = BuildMinimalContext(opts);

        var decisions = SectionDecisionProjector.ProjectDocumentLevel(
            opts, doc, fm, overview, sessions, null, [], null, null, [], [], [], [], [], []);

        var fmDecision = decisions.First(d => d.Section == FitMarkdownSectionKey.Frontmatter);
        fmDecision.ShouldRender.Should().BeFalse();
        fmDecision.OmissionReason.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ProjectDocumentLevel_should_always_omit_data_quality()
    {
        var (opts, doc, fm, overview, sessions) = BuildMinimalContext();

        var decisions = SectionDecisionProjector.ProjectDocumentLevel(
            opts, doc, fm, overview, sessions, null, [], null, null, [], [], [], [], [], []);

        var dqDecision = decisions.First(d => d.Section == FitMarkdownSectionKey.DataQuality);
        dqDecision.ShouldRender.Should().BeFalse();
    }

    [Fact]
    public void ProjectDocumentLevel_should_omit_empty_sections_with_no_data()
    {
        var (opts, doc, fm, overview, sessions) = BuildMinimalContext();

        var decisions = SectionDecisionProjector.ProjectDocumentLevel(
            opts, doc, fm, overview, sessions, null, [], null, null, [], [], [], [], [], []);

        // HRV, HeartRateZones, Devices, Events — all empty
        var hrvDecision = decisions.First(d => d.Section == FitMarkdownSectionKey.HrvData);
        hrvDecision.ShouldRender.Should().BeFalse();

        var devicesDecision = decisions.First(d => d.Section == FitMarkdownSectionKey.Devices);
        devicesDecision.ShouldRender.Should().BeFalse();
    }

    [Fact]
    public void ProjectSessionLevel_should_collapse_single_lap()
    {
        var opts = MarkdownTestFixtures.CreateDefaultOptions();
        var session = new SessionSection
        {
            Session = new Core.Models.FitSession
            {
                SessionIndex = 0,
                Sport = FitSport.Running,
            },
            LapRows = [new LapTableRow { LapNumber = 1, DistanceMeters = 5000 }],
        };

        var decisions = SectionDecisionProjector.ProjectSessionLevel(opts, session, 0);

        var lapDecision = decisions.First(d => d.Section == FitMarkdownSectionKey.LapDetails);
        lapDecision.ShouldRender.Should().BeFalse();
    }

    [Fact]
    public void ProjectSessionLevel_should_show_multiple_laps()
    {
        var opts = MarkdownTestFixtures.CreateDefaultOptions();
        var session = new SessionSection
        {
            Session = new Core.Models.FitSession
            {
                SessionIndex = 0,
                Sport = FitSport.Running,
            },
            LapRows =
            [
                new LapTableRow { LapNumber = 1, DistanceMeters = 3000 },
                new LapTableRow { LapNumber = 2, DistanceMeters = 3200 },
            ],
        };

        var decisions = SectionDecisionProjector.ProjectSessionLevel(opts, session, 0);

        var lapDecision = decisions.First(d => d.Section == FitMarkdownSectionKey.LapDetails);
        lapDecision.ShouldRender.Should().BeTrue();
    }
}
