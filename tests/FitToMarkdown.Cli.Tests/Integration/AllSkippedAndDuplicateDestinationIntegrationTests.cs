using FitToMarkdown.Cli.Models;
using FitToMarkdown.Cli.Services;
using FitToMarkdown.Cli.Tests.Fixtures;
using FluentAssertions;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Integration;

public sealed class AllSkippedAndDuplicateDestinationIntegrationTests
{
    [Fact]
    public async Task All_files_generate_same_output_name_first_succeeds_others_fail_duplicate()
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
        var generator = new FakeMarkdownDocumentGenerator { ForceSameFileName = true };
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

        var summary = await batchRunner.RunAsync(plan, degreeOfParallelism: 1, CancellationToken.None);

        summary.ConvertedCount.Should().Be(1);
        summary.FailedCount.Should().Be(2);
        summary.Results.Where(r => r.Status == ConvertFileResultStatus.Failed)
            .Should().AllSatisfy(r => r.FailureReason.Should().Contain("Duplicate output destination"));
    }

    [Fact]
    public async Task All_files_exist_with_skip_mode_all_skipped()
    {
        var fileSystem = new FakeCliFileSystem();
        fileSystem.AddFitFile(@"C:\data\a.fit");
        fileSystem.AddFitFile(@"C:\data\b.fit");

        var parser = new FakeFitFileParser();
        parser.Results[@"C:\data\a.fit"] = ParseResultFactory.CreateSuccessful();
        parser.Results[@"C:\data\b.fit"] = ParseResultFactory.CreateSuccessful();

        var projector = new FakeFitMarkdownProjector();
        var generator = new FakeMarkdownDocumentGenerator();
        var promptService = new ConvertPromptService(new TestConsole());

        var batchRunner = new ConversionBatchRunner(
            parser, projector, generator, fileSystem,
            new FitParseOptionsFactory(), new MarkdownOptionsFactory(),
            new OutputPathResolver(), promptService);

        // Pre-populate output files so they "exist"
        fileSystem.Files[@"C:\output\test.md"] = [0x00];
        fileSystem.Files[@"C:\output\test_2.md"] = [0x00];

        var plan = new ConvertExecutionPlan
        {
            SourceTarget = new InputTarget(@"C:\data", InputTargetKind.Directory),
            Files =
            [
                new DiscoveredFitFile(@"C:\data\a.fit", "(root)", "a.fit", 1024, DateTimeOffset.UtcNow),
                new DiscoveredFitFile(@"C:\data\b.fit", "(root)", "b.fit", 1024, DateTimeOffset.UtcNow),
            ],
            OutputDirectory = @"C:\output",
            OverwriteMode = ConvertOverwriteMode.Skip,
            NoInteraction = true,
        };

        var summary = await batchRunner.RunAsync(plan, degreeOfParallelism: 1, CancellationToken.None);

        summary.SkippedCount.Should().Be(2);
        summary.ConvertedCount.Should().Be(0);
        summary.FailedCount.Should().Be(0);
    }

    [Fact]
    public async Task Mixed_duplicate_skip_and_success_yields_partial()
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
        // a.fit and b.fit will get the same name (force same), c.fit gets unique
        var generator = new FakeMarkdownDocumentGenerator { ForceSameFileName = true };
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

        var summary = await batchRunner.RunAsync(plan, degreeOfParallelism: 1, CancellationToken.None);

        // First file succeeds, second and third are duplicates (all get same name)
        summary.ConvertedCount.Should().Be(1);
        summary.FailedCount.Should().Be(2);
    }

    [Fact]
    public async Task Unique_output_names_all_succeed()
    {
        var fileSystem = new FakeCliFileSystem();
        fileSystem.AddFitFile(@"C:\data\run1.fit");
        fileSystem.AddFitFile(@"C:\data\run2.fit");

        var parser = new FakeFitFileParser();
        parser.Results[@"C:\data\run1.fit"] = ParseResultFactory.CreateSuccessful();
        parser.Results[@"C:\data\run2.fit"] = ParseResultFactory.CreateSuccessful();

        var projector = new FakeFitMarkdownProjector();
        var generator = new FakeMarkdownDocumentGenerator(); // Default: unique names
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
                new DiscoveredFitFile(@"C:\data\run1.fit", "(root)", "run1.fit", 1024, DateTimeOffset.UtcNow),
                new DiscoveredFitFile(@"C:\data\run2.fit", "(root)", "run2.fit", 1024, DateTimeOffset.UtcNow),
            ],
            OutputDirectory = @"C:\output",
            OverwriteMode = ConvertOverwriteMode.Overwrite,
            NoInteraction = true,
        };

        var summary = await batchRunner.RunAsync(plan, degreeOfParallelism: 1, CancellationToken.None);

        summary.ConvertedCount.Should().Be(2);
        summary.FailedCount.Should().Be(0);
    }
}
