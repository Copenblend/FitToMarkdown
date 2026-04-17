using FitToMarkdown.Cli.Configuration;
using FitToMarkdown.Cli.Rendering;
using FluentAssertions;
using Spectre.Console.Testing;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Rendering;

public sealed class CliExceptionRendererTests
{
    private readonly TestConsole _console;
    private readonly CliExceptionRenderer _renderer;

    public CliExceptionRendererTests()
    {
        _console = new TestConsole();
        _console.Profile.Width = 120;
        _renderer = new CliExceptionRenderer(_console);
    }

    [Fact]
    public void RenderInvalidInput_outputs_Error_prefix()
    {
        _renderer.RenderInvalidInput("Something went wrong");

        _console.Output.Should().Contain("Error:");
        _console.Output.Should().Contain("Something went wrong");
    }

    [Fact]
    public void RenderWarning_outputs_Warning_prefix()
    {
        _renderer.RenderWarning("Proceed with caution");

        _console.Output.Should().Contain("Warning:");
        _console.Output.Should().Contain("Proceed with caution");
    }

    [Fact]
    public void MapUnexpectedToExitCode_returns_UnexpectedError()
    {
        var exitCode = _renderer.MapUnexpectedToExitCode(new InvalidOperationException("test"));

        exitCode.Should().Be(CliExitCodes.UnexpectedError);
    }
}
