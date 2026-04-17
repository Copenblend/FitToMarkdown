using FitToMarkdown.Cli.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace FitToMarkdown.Cli.Tests.EndToEnd;

/// <summary>
/// Process-level smoke tests for the convert command.
/// These spawn the CLI as a child process to verify real entry point behavior.
/// </summary>
public sealed class ProgramConvertSmokeTests : IDisposable
{
    private readonly TemporaryWorkspace _workspace = new();

    [Fact]
    public async Task Convert_help_exits_with_zero()
    {
        var result = await CliProcessRunner.RunAsync(["convert", "--help"], _workspace.Root);

        result.ExitCode.Should().Be(0);
    }

    [Fact]
    public async Task Convert_no_interaction_missing_path_exits_nonzero()
    {
        var result = await CliProcessRunner.RunAsync(
            ["convert", "--no-interaction"], _workspace.Root);

        result.ExitCode.Should().NotBe(0);
    }

    public void Dispose() => _workspace.Dispose();
}
