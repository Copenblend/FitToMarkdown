using FitToMarkdown.Cli.Commands.Convert;
using FitToMarkdown.Cli.Configuration;
using FitToMarkdown.Cli.Rendering;
using FitToMarkdown.Cli.Services;
using FitToMarkdown.Cli.Tests.Fixtures;
using FluentAssertions;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Regression;

public sealed class ExitCodePolicyMatrixTests
{
    private readonly TestConsole _console;
    private readonly FakeCliFileSystem _fileSystem;
    private readonly FakeFitFileParser _parser;
    private readonly FakeMarkdownDocumentGenerator _generator;
    private readonly ConvertCommandWorkflow _workflow;

    public ExitCodePolicyMatrixTests()
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
            _parser, projector, _generator, _fileSystem,
            new FitParseOptionsFactory(), optionsFactory, outputPathResolver, promptService);
        var summaryRenderer = new ConvertSummaryRenderer(_console);
        var exceptionRenderer = new CliExceptionRenderer(_console);

        _workflow = new ConvertCommandWorkflow(
            _console, _fileSystem, pathResolver, discoveryService, promptService,
            optionsFactory, outputPathResolver, batchRunner, summaryRenderer, exceptionRenderer);
    }

    [Fact]
    public async Task All_files_succeed_returns_Success()
    {
        _fileSystem.AddFitFile(@"C:\test\a.fit");
        _fileSystem.AddFitFile(@"C:\test\b.fit");
        _parser.Results[@"C:\test\a.fit"] = ParseResultFactory.CreateSuccessful();
        _parser.Results[@"C:\test\b.fit"] = ParseResultFactory.CreateSuccessful();

        var settings = new ConvertCommandSettings { Path = @"C:\test", NoInteraction = true };

        var exitCode = await _workflow.ExecuteAsync(settings);

        exitCode.Should().Be(CliExitCodes.Success);
    }

    [Fact]
    public async Task Some_succeed_some_fail_returns_PartialSuccess()
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
    public async Task All_files_fail_returns_TotalFailure()
    {
        _fileSystem.AddFitFile(@"C:\test\bad1.fit");
        _fileSystem.AddFitFile(@"C:\test\bad2.fit");
        _parser.Results[@"C:\test\bad1.fit"] = ParseResultFactory.CreateFailed("Corrupt");
        _parser.Results[@"C:\test\bad2.fit"] = ParseResultFactory.CreateFailed("Truncated");

        var settings = new ConvertCommandSettings { Path = @"C:\test", NoInteraction = true };

        var exitCode = await _workflow.ExecuteAsync(settings);

        exitCode.Should().Be(CliExitCodes.TotalFailure);
    }

    [Fact]
    public async Task Invalid_input_path_returns_InvalidInput()
    {
        var settings = new ConvertCommandSettings { Path = @"C:\nonexistent\path.fit", NoInteraction = true };

        var exitCode = await _workflow.ExecuteAsync(settings);

        exitCode.Should().Be(CliExitCodes.InvalidInput);
    }

    [Fact]
    public async Task Missing_path_no_interaction_returns_InvalidInput()
    {
        var settings = new ConvertCommandSettings { Path = null, NoInteraction = true };

        var exitCode = await _workflow.ExecuteAsync(settings);

        exitCode.Should().Be(CliExitCodes.InvalidInput);
    }
}
