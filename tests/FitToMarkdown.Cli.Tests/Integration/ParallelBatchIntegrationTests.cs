using FitToMarkdown.Cli.Models;
using FitToMarkdown.Cli.Services;
using FitToMarkdown.Cli.Tests.Fixtures;
using FluentAssertions;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Integration;

public sealed class ParallelBatchIntegrationTests
{
    [Fact]
    public async Task Three_files_with_degree_two_all_convert_successfully()
    {
        var fileSystem = new FakeCliFileSystem();
        fileSystem.AddFitFile(@"C:\data\a.fit");
        fileSystem.AddFitFile(@"C:\data\b.fit");
        fileSystem.AddFitFile(@"C:\data\c.fit");

        var parser = new FakeFitFileParser();
        parser.Results[@"C:\data\a.fit"] = ParseResultFactory.CreateSuccessful();
        parser.Results[@"C:\data\b.fit"] = ParseResultFactory.CreateSuccessful();
        parser.Results[@"C:\data\c.fit"] = ParseResultFactory.CreateSuccessful();

        var projector = new FakeFitMarkdownProjector();
        var generator = new FakeMarkdownDocumentGenerator();
        var promptService = new ConvertPromptService(new TestConsole());

        var batchRunner = new ConversionBatchRunner(
            parser, projector, generator, fileSystem,
            new FitParseOptionsFactory(), new MarkdownOptionsFactory(),
            new OutputPathResolver(), promptService);

        var plan = new ConvertExecutionPlan
        {
            SourceTarget = new InputTarget(@"C:\data", InputTargetKind.Directory),
            Files =
            [
                new DiscoveredFitFile(@"C:\data\a.fit", "(root)", "a.fit", 1024, DateTimeOffset.UtcNow),
                new DiscoveredFitFile(@"C:\data\b.fit", "(root)", "b.fit", 1024, DateTimeOffset.UtcNow),
                new DiscoveredFitFile(@"C:\data\c.fit", "(root)", "c.fit", 1024, DateTimeOffset.UtcNow),
            ],
            OutputDirectory = @"C:\output",
            OverwriteMode = ConvertOverwriteMode.Overwrite,
            NoInteraction = true,
        };

        var summary = await batchRunner.RunAsync(plan, degreeOfParallelism: 2, CancellationToken.None);

        summary.ConvertedCount.Should().Be(3);
        summary.FailedCount.Should().Be(0);
        summary.SkippedCount.Should().Be(0);
        fileSystem.WrittenFiles.Should().HaveCount(3);
    }

    [Fact]
    public async Task Single_file_with_high_degree_still_works()
    {
        var fileSystem = new FakeCliFileSystem();
        fileSystem.AddFitFile(@"C:\data\single.fit");

        var parser = new FakeFitFileParser();
        parser.Results[@"C:\data\single.fit"] = ParseResultFactory.CreateSuccessful();

        var projector = new FakeFitMarkdownProjector();
        var generator = new FakeMarkdownDocumentGenerator();
        var promptService = new ConvertPromptService(new TestConsole());

        var batchRunner = new ConversionBatchRunner(
            parser, projector, generator, fileSystem,
            new FitParseOptionsFactory(), new MarkdownOptionsFactory(),
            new OutputPathResolver(), promptService);

        var plan = new ConvertExecutionPlan
        {
            SourceTarget = new InputTarget(@"C:\data\single.fit", InputTargetKind.File),
            Files =
            [
                new DiscoveredFitFile(@"C:\data\single.fit", "(root)", "single.fit", 1024, DateTimeOffset.UtcNow),
            ],
            OutputDirectory = @"C:\output",
            OverwriteMode = ConvertOverwriteMode.Overwrite,
            NoInteraction = true,
        };

        var summary = await batchRunner.RunAsync(plan, degreeOfParallelism: 4, CancellationToken.None);

        summary.ConvertedCount.Should().Be(1);
        summary.FailedCount.Should().Be(0);
    }

    [Fact]
    public async Task Multiple_files_with_one_parse_failure_yields_partial_success()
    {
        var fileSystem = new FakeCliFileSystem();
        fileSystem.AddFitFile(@"C:\data\good1.fit");
        fileSystem.AddFitFile(@"C:\data\bad.fit");
        fileSystem.AddFitFile(@"C:\data\good2.fit");

        var parser = new FakeFitFileParser();
        parser.Results[@"C:\data\good1.fit"] = ParseResultFactory.CreateSuccessful();
        parser.Results[@"C:\data\bad.fit"] = ParseResultFactory.CreateFailed("Corrupt file");
        parser.Results[@"C:\data\good2.fit"] = ParseResultFactory.CreateSuccessful();

        var projector = new FakeFitMarkdownProjector();
        var generator = new FakeMarkdownDocumentGenerator();
        var promptService = new ConvertPromptService(new TestConsole());

        var batchRunner = new ConversionBatchRunner(
            parser, projector, generator, fileSystem,
            new FitParseOptionsFactory(), new MarkdownOptionsFactory(),
            new OutputPathResolver(), promptService);

        var plan = new ConvertExecutionPlan
        {
            SourceTarget = new InputTarget(@"C:\data", InputTargetKind.Directory),
            Files =
            [
                new DiscoveredFitFile(@"C:\data\good1.fit", "(root)", "good1.fit", 1024, DateTimeOffset.UtcNow),
                new DiscoveredFitFile(@"C:\data\bad.fit", "(root)", "bad.fit", 1024, DateTimeOffset.UtcNow),
                new DiscoveredFitFile(@"C:\data\good2.fit", "(root)", "good2.fit", 1024, DateTimeOffset.UtcNow),
            ],
            OutputDirectory = @"C:\output",
            OverwriteMode = ConvertOverwriteMode.Overwrite,
            NoInteraction = true,
        };

        var summary = await batchRunner.RunAsync(plan, degreeOfParallelism: 2, CancellationToken.None);

        summary.ConvertedCount.Should().Be(2);
        summary.FailedCount.Should().Be(1);
        summary.Results.Single(r => r.Status == ConvertFileResultStatus.Failed)
            .FailureReason.Should().Contain("Corrupt");
    }
}
