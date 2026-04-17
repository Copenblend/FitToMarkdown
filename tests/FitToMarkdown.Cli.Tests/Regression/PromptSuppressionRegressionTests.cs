using FitToMarkdown.Cli.Commands.Convert;
using FitToMarkdown.Cli.Configuration;
using FitToMarkdown.Cli.Rendering;
using FitToMarkdown.Cli.Services;
using FitToMarkdown.Cli.Tests.Fixtures;
using FluentAssertions;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Regression;

public sealed class PromptSuppressionRegressionTests
{
    private readonly TestConsole _console;
    private readonly FakeCliFileSystem _fileSystem;
    private readonly FakeFitFileParser _parser;
    private readonly ConvertCommandWorkflow _workflow;

    public PromptSuppressionRegressionTests()
    {
        _console = new TestConsole();
        _console.Profile.Width = 120;

        _fileSystem = new FakeCliFileSystem();
        _parser = new FakeFitFileParser();
        var projector = new FakeFitMarkdownProjector();
        var generator = new FakeMarkdownDocumentGenerator();

        var browser = new InteractivePathBrowser(_console, _fileSystem);
        var pathResolver = new InputPathResolver(_fileSystem, browser);
        var discoveryService = new FitFileDiscoveryService(_fileSystem);
        var promptService = new ConvertPromptService(_console);
        var optionsFactory = new MarkdownOptionsFactory();
        var outputPathResolver = new OutputPathResolver();
        var batchRunner = new ConversionBatchRunner(
            _parser, projector, generator, _fileSystem,
            new FitParseOptionsFactory(), optionsFactory, outputPathResolver, promptService);
        var summaryRenderer = new ConvertSummaryRenderer(_console);
        var exceptionRenderer = new CliExceptionRenderer(_console);

        _workflow = new ConvertCommandWorkflow(
            _console, _fileSystem, pathResolver, discoveryService, promptService,
            optionsFactory, outputPathResolver, batchRunner, summaryRenderer, exceptionRenderer);
    }

    [Fact]
    public async Task NoInteraction_mode_converts_single_file_without_prompts()
    {
        _fileSystem.AddFitFile(@"C:\test\activity.fit");
        _parser.Results[@"C:\test\activity.fit"] = ParseResultFactory.CreateSuccessful();

        var settings = new ConvertCommandSettings
        {
            Path = @"C:\test\activity.fit",
            NoInteraction = true,
        };

        var exitCode = await _workflow.ExecuteAsync(settings);

        exitCode.Should().Be(CliExitCodes.Success);
        _fileSystem.WrittenFiles.Should().HaveCount(1);
    }

    [Fact]
    public async Task NoInteraction_mode_converts_directory_without_overwrite_prompt()
    {
        _fileSystem.AddFitFile(@"C:\test\a.fit");
        _fileSystem.AddFitFile(@"C:\test\b.fit");
        _parser.Results[@"C:\test\a.fit"] = ParseResultFactory.CreateSuccessful();
        _parser.Results[@"C:\test\b.fit"] = ParseResultFactory.CreateSuccessful();

        var settings = new ConvertCommandSettings
        {
            Path = @"C:\test",
            NoInteraction = true,
        };

        var exitCode = await _workflow.ExecuteAsync(settings);

        exitCode.Should().Be(CliExitCodes.Success);
        _fileSystem.WrittenFiles.Should().HaveCount(2);
    }

    [Fact]
    public async Task NoInteraction_mode_with_output_directory_skips_directory_prompt()
    {
        _fileSystem.AddFitFile(@"C:\test\activity.fit");
        _parser.Results[@"C:\test\activity.fit"] = ParseResultFactory.CreateSuccessful();

        var settings = new ConvertCommandSettings
        {
            Path = @"C:\test\activity.fit",
            OutputDirectory = @"C:\output",
            NoInteraction = true,
        };

        var exitCode = await _workflow.ExecuteAsync(settings);

        exitCode.Should().Be(CliExitCodes.Success);
        _fileSystem.WrittenFiles.Should().HaveCount(1);
        _fileSystem.WrittenFiles[0].Path.Should().StartWith(@"C:\output");
    }
}
