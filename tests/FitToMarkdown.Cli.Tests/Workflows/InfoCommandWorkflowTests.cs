using FitToMarkdown.Cli.Commands.Info;
using FitToMarkdown.Cli.Configuration;
using FitToMarkdown.Cli.Rendering;
using FitToMarkdown.Cli.Services;
using FitToMarkdown.Cli.Tests.Fixtures;
using FluentAssertions;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Workflows;

public sealed class InfoCommandWorkflowTests
{
    private readonly TestConsole _console;
    private readonly FakeCliFileSystem _fileSystem;
    private readonly FakeFitMetadataInspector _inspector;
    private readonly InfoCommandWorkflow _workflow;

    public InfoCommandWorkflowTests()
    {
        _console = new TestConsole();
        _console.Profile.Width = 120;

        _fileSystem = new FakeCliFileSystem();
        _inspector = new FakeFitMetadataInspector();
        var tableRenderer = new InfoTableRenderer(_console);
        var exceptionRenderer = new CliExceptionRenderer(_console);
        var browser = new InteractivePathBrowser(_console, _fileSystem);
        var pathResolver = new InputPathResolver(_fileSystem, browser);

        _workflow = new InfoCommandWorkflow(
            _console, _fileSystem, _inspector, tableRenderer, exceptionRenderer, pathResolver);
    }

    [Fact]
    public async Task Non_existent_file_returns_InvalidInput()
    {
        var settings = new InfoCommandSettings { Path = @"C:\test\missing.fit" };

        var exitCode = await _workflow.ExecuteAsync(settings);

        exitCode.Should().Be(CliExitCodes.InvalidInput);
        _console.Output.Should().Contain("Error:");
    }

    [Fact]
    public async Task Non_fit_file_returns_InvalidInput()
    {
        _fileSystem.Files[@"C:\test\data.txt"] = new byte[10];

        var settings = new InfoCommandSettings { Path = @"C:\test\data.txt" };

        var exitCode = await _workflow.ExecuteAsync(settings);

        exitCode.Should().Be(CliExitCodes.InvalidInput);
    }

    [Fact]
    public async Task Fatal_inspection_error_returns_TotalFailure()
    {
        _fileSystem.AddFitFile(@"C:\test\corrupt.fit");
        _inspector.Results[@"C:\test\corrupt.fit"] = MetadataInspectionResultFactory.CreateFailed("CRC mismatch");

        var settings = new InfoCommandSettings { Path = @"C:\test\corrupt.fit" };

        var exitCode = await _workflow.ExecuteAsync(settings);

        exitCode.Should().Be(CliExitCodes.TotalFailure);
    }

    [Fact]
    public async Task Null_summary_returns_TotalFailure()
    {
        _fileSystem.AddFitFile(@"C:\test\empty.fit");
        _inspector.Results[@"C:\test\empty.fit"] = new Core.Parsing.FitMetadataInspectionResult();

        var settings = new InfoCommandSettings { Path = @"C:\test\empty.fit" };

        var exitCode = await _workflow.ExecuteAsync(settings);

        exitCode.Should().Be(CliExitCodes.TotalFailure);
    }

    [Fact]
    public async Task Successful_inspection_returns_Success_and_renders_table()
    {
        _fileSystem.AddFitFile(@"C:\test\activity.fit");
        _inspector.Results[@"C:\test\activity.fit"] = MetadataInspectionResultFactory.CreateSuccessful();

        var settings = new InfoCommandSettings { Path = @"C:\test\activity.fit" };

        var exitCode = await _workflow.ExecuteAsync(settings);

        exitCode.Should().Be(CliExitCodes.Success);
        _console.Output.Should().Contain("Running");
        _console.Output.Should().Contain("Garmin");
    }

    [Fact]
    public async Task Successful_inspection_with_devices_renders_device_table()
    {
        _fileSystem.AddFitFile(@"C:\test\activity.fit");
        _inspector.Results[@"C:\test\activity.fit"] = MetadataInspectionResultFactory.CreateWithDevices();

        var settings = new InfoCommandSettings { Path = @"C:\test\activity.fit" };

        var exitCode = await _workflow.ExecuteAsync(settings);

        exitCode.Should().Be(CliExitCodes.Success);
        _console.Output.Should().Contain("Watch");
        _console.Output.Should().Contain("Forerunner 265");
    }
}
