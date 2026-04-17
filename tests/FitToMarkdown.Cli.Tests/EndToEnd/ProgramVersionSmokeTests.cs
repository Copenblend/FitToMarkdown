using FitToMarkdown.Cli.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace FitToMarkdown.Cli.Tests.EndToEnd;

/// <summary>
/// Process-level smoke tests for the version command.
/// These spawn the CLI as a child process to verify the real entry point.
/// </summary>
public sealed class ProgramVersionSmokeTests : IDisposable
{
    private readonly TemporaryWorkspace _workspace = new();

    [Fact]
    public async Task Version_command_exits_with_zero()
    {
        var result = await CliProcessRunner.RunAsync(["version"], _workspace.Root);

        result.ExitCode.Should().Be(0);
    }

    [Fact]
    public async Task Version_command_outputs_product_name()
    {
        var result = await CliProcessRunner.RunAsync(["version"], _workspace.Root);

        result.StandardOutput.Should().Contain("fittomarkdown");
    }

    public void Dispose() => _workspace.Dispose();
}
