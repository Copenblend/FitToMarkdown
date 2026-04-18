using FitToMarkdown.Cli.Commands.Convert;
using FitToMarkdown.Cli.Configuration;
using FitToMarkdown.Cli.Rendering;
using FitToMarkdown.Cli.Services;
using FitToMarkdown.Cli.Tests.Fixtures;
using FluentAssertions;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Workflows;

public sealed class ConvertCommandWorkflowTests
{
    private readonly TestConsole _console;
    private readonly FakeCliFileSystem _fileSystem;
    private readonly FakeFitFileParser _parser;
    private readonly FakeMarkdownDocumentGenerator _generator;
    private readonly ConvertCommandWorkflow _workflow;

    public ConvertCommandWorkflowTests()
    {
        _console = new TestConsole();
        _console.Profile.Width = 120;

        _fileSystem = new FakeCliFileSystem();
        _parser = new FakeFitFileParser();
        var projector = new FakeFitMarkdownProjector();
        _generator = new FakeMarkdownDocumentGenerator();

        var browser = new InteractivePathBrowser(_console, _fileSystem);
        var pathResolver = new InputPathResolver(_fileSystem, browser);
        var discoveryService = new FitFileDiscoveryService(_fileSystem);
        var promptService = new ConvertPromptService(_console);
        var optionsFactory = new MarkdownOptionsFactory();
        var outputPathResolver = new OutputPathResolver();
        var batchRunner = new ConversionBatchRunner(
            _parser, projector, _generator, _fileSystem, new FitParseOptionsFactory(), optionsFactory, outputPathResolver, promptService);
        var summaryRenderer = new ConvertSummaryRenderer(_console);
        var exceptionRenderer = new CliExceptionRenderer(_console);

        var concurrencyPolicy = new BatchConcurrencyPolicy();

        _workflow = new ConvertCommandWorkflow(
            _console, _fileSystem, pathResolver, discoveryService, promptService,
            optionsFactory, outputPathResolver, batchRunner, summaryRenderer, exceptionRenderer,
            concurrencyPolicy);
    }

    [Fact]
    public async Task Missing_path_with_no_interaction_returns_InvalidInput()
    {
        var settings = new ConvertCommandSettings { Path = null, NoInteraction = true };

        var exitCode = await _workflow.ExecuteAsync(settings);

        exitCode.Should().Be(CliExitCodes.InvalidInput);
        _console.Output.Should().Contain("Error:");
    }

    [Fact]
    public async Task Non_existent_path_returns_InvalidInput()
    {
        var settings = new ConvertCommandSettings { Path = @"C:\test\doesnotexist.fit", NoInteraction = true };

        var exitCode = await _workflow.ExecuteAsync(settings);

        exitCode.Should().Be(CliExitCodes.InvalidInput);
    }

    [Fact]
    public async Task Non_fit_file_returns_InvalidInput()
    {
        _fileSystem.Files[@"C:\test\data.txt"] = new byte[10];

        var settings = new ConvertCommandSettings { Path = @"C:\test\data.txt", NoInteraction = true };

        var exitCode = await _workflow.ExecuteAsync(settings);

        exitCode.Should().Be(CliExitCodes.InvalidInput);
    }

    [Fact]
    public async Task Single_fit_file_converts_successfully()
    {
        _fileSystem.AddFitFile(@"C:\test\activity.fit");
        _parser.Results[@"C:\test\activity.fit"] = ParseResultFactory.CreateSuccessful();

        var settings = new ConvertCommandSettings { Path = @"C:\test\activity.fit", NoInteraction = true };

        var exitCode = await _workflow.ExecuteAsync(settings);

        exitCode.Should().Be(CliExitCodes.Success);
        _fileSystem.WrittenFiles.Should().HaveCount(1);
        _fileSystem.WrittenFiles[0].Content.Should().Be(_generator.ContentToReturn);
    }

    [Fact]
    public async Task Directory_with_no_fit_files_returns_InvalidInput()
    {
        _fileSystem.AddDirectory(@"C:\test");

        var settings = new ConvertCommandSettings { Path = @"C:\test", NoInteraction = true };

        var exitCode = await _workflow.ExecuteAsync(settings);

        exitCode.Should().Be(CliExitCodes.InvalidInput);
        _console.Output.Should().Contain("No .fit files found");
    }

    [Fact]
    public async Task Single_file_parse_failure_returns_TotalFailure()
    {
        _fileSystem.AddFitFile(@"C:\test\bad.fit");
        _parser.Results[@"C:\test\bad.fit"] = ParseResultFactory.CreateFailed("Corrupt file");

        var settings = new ConvertCommandSettings { Path = @"C:\test\bad.fit", NoInteraction = true };

        var exitCode = await _workflow.ExecuteAsync(settings);

        exitCode.Should().Be(CliExitCodes.TotalFailure);
        _fileSystem.WrittenFiles.Should().BeEmpty();
    }

    [Fact]
    public async Task Directory_mixed_results_returns_PartialSuccess()
    {
        _fileSystem.AddFitFile(@"C:\test\good.fit");
        _fileSystem.AddFitFile(@"C:\test\bad.fit");
        _parser.Results[@"C:\test\good.fit"] = ParseResultFactory.CreateSuccessful();
        _parser.Results[@"C:\test\bad.fit"] = ParseResultFactory.CreateFailed("Corrupt");

        var settings = new ConvertCommandSettings { Path = @"C:\test", NoInteraction = true };

        var exitCode = await _workflow.ExecuteAsync(settings);

        exitCode.Should().Be(CliExitCodes.PartialSuccess);
    }

    [Fact]
    public async Task Directory_all_succeed_returns_Success()
    {
        _fileSystem.AddFitFile(@"C:\test\a.fit");
        _fileSystem.AddFitFile(@"C:\test\b.fit");
        _parser.Results[@"C:\test\a.fit"] = ParseResultFactory.CreateSuccessful();
        _parser.Results[@"C:\test\b.fit"] = ParseResultFactory.CreateSuccessful();

        var settings = new ConvertCommandSettings { Path = @"C:\test", NoInteraction = true };

        var exitCode = await _workflow.ExecuteAsync(settings);

        exitCode.Should().Be(CliExitCodes.Success);
        _fileSystem.WrittenFiles.Should().HaveCount(2);
    }

    [Fact]
    public async Task Custom_output_directory_is_used()
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
