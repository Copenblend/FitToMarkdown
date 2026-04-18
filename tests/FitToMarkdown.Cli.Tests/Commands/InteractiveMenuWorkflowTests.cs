using FitToMarkdown.Cli.Commands.Convert;
using FitToMarkdown.Cli.Commands.Info;
using FitToMarkdown.Cli.Commands.Interactive;
using FitToMarkdown.Cli.Commands.Progression;
using FitToMarkdown.Cli.Commands.Version;
using FitToMarkdown.Cli.Configuration;
using FitToMarkdown.Cli.Rendering;
using FitToMarkdown.Cli.Services;
using FitToMarkdown.Cli.Tests.Fixtures;
using FluentAssertions;
using Spectre.Console;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Commands;

public sealed class InteractiveMenuWorkflowTests
{
    private readonly TestConsole _console;
    private readonly FakeConvertWorkflow _convertWorkflow;
    private readonly FakeInfoWorkflow _infoWorkflow;
    private readonly FakeProgressionWorkflow _progressionWorkflow;
    private readonly FakeVersionWorkflow _versionWorkflow;
    private readonly InteractiveMenuWorkflow _workflow;

    public InteractiveMenuWorkflowTests()
    {
        _console = new TestConsole();
        _console.Profile.Width = 120;
        _console.Profile.Capabilities.Interactive = true;

        _convertWorkflow = new FakeConvertWorkflow();
        _infoWorkflow = new FakeInfoWorkflow();
        _progressionWorkflow = new FakeProgressionWorkflow();
        _versionWorkflow = new FakeVersionWorkflow();

        var fileSystem = new FakeCliFileSystem();
        var browser = new InteractivePathBrowser(_console, fileSystem);
        var pathResolver = new InputPathResolver(fileSystem, browser);
        var exceptionRenderer = new CliExceptionRenderer(_console);

        _workflow = new InteractiveMenuWorkflow(
            _console,
            _convertWorkflow,
            _infoWorkflow,
            _progressionWorkflow,
            _versionWorkflow,
            pathResolver,
            exceptionRenderer);
    }

    [Fact]
    public async Task Exit_choice_returns_success()
    {
        // Navigate to "Exit" (5th item, index 4)
        _console.Input.PushKey(ConsoleKey.DownArrow);
        _console.Input.PushKey(ConsoleKey.DownArrow);
        _console.Input.PushKey(ConsoleKey.DownArrow);
        _console.Input.PushKey(ConsoleKey.DownArrow);
        _console.Input.PushKey(ConsoleKey.Enter);

        var exitCode = await _workflow.RunAsync();

        exitCode.Should().Be(CliExitCodes.Success);
    }

    [Fact]
    public async Task Convert_choice_delegates_to_workflow()
    {
        // Select "Convert" (1st item — already highlighted)
        _console.Input.PushKey(ConsoleKey.Enter);
        // ReadKey after action
        _console.Input.PushKey(ConsoleKey.Enter);
        // Navigate to "Exit" on second loop (5th item)
        _console.Input.PushKey(ConsoleKey.DownArrow);
        _console.Input.PushKey(ConsoleKey.DownArrow);
        _console.Input.PushKey(ConsoleKey.DownArrow);
        _console.Input.PushKey(ConsoleKey.DownArrow);
        _console.Input.PushKey(ConsoleKey.Enter);

        var exitCode = await _workflow.RunAsync();

        exitCode.Should().Be(CliExitCodes.Success);
        _convertWorkflow.WasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Version_choice_delegates_to_workflow()
    {
        // Navigate to "Show version" (4th item, index 3)
        _console.Input.PushKey(ConsoleKey.DownArrow);
        _console.Input.PushKey(ConsoleKey.DownArrow);
        _console.Input.PushKey(ConsoleKey.DownArrow);
        _console.Input.PushKey(ConsoleKey.Enter);
        // ReadKey after action
        _console.Input.PushKey(ConsoleKey.Enter);
        // Navigate to "Exit" on second loop (5th item)
        _console.Input.PushKey(ConsoleKey.DownArrow);
        _console.Input.PushKey(ConsoleKey.DownArrow);
        _console.Input.PushKey(ConsoleKey.DownArrow);
        _console.Input.PushKey(ConsoleKey.DownArrow);
        _console.Input.PushKey(ConsoleKey.Enter);

        var exitCode = await _workflow.RunAsync();

        exitCode.Should().Be(CliExitCodes.Success);
        _versionWorkflow.WasCalled.Should().BeTrue();
    }

    private sealed class FakeConvertWorkflow : IConvertCommandWorkflow
    {
        public bool WasCalled { get; private set; }

        public Task<int> ExecuteAsync(ConvertCommandSettings settings, CancellationToken cancellationToken = default)
        {
            WasCalled = true;
            return Task.FromResult(CliExitCodes.Success);
        }
    }

    private sealed class FakeInfoWorkflow : IInfoCommandWorkflow
    {
        public bool WasCalled { get; private set; }

        public Task<int> ExecuteAsync(InfoCommandSettings settings, CancellationToken cancellationToken = default)
        {
            WasCalled = true;
            return Task.FromResult(CliExitCodes.Success);
        }
    }

    private sealed class FakeProgressionWorkflow : IProgressionCommandWorkflow
    {
        public bool WasCalled { get; private set; }

        public Task<int> ExecuteAsync(ProgressionCommandSettings settings, CancellationToken cancellationToken = default)
        {
            WasCalled = true;
            return Task.FromResult(CliExitCodes.Success);
        }
    }

    private sealed class FakeVersionWorkflow : IVersionCommandWorkflow
    {
        public bool WasCalled { get; private set; }

        public int Execute()
        {
            WasCalled = true;
            return CliExitCodes.Success;
        }
    }
}
