using FitToMarkdown.Cli.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace FitToMarkdown.Cli.Tests.EndToEnd;

public sealed class ProgramConcurrencySmokeTests : IDisposable
{
    private readonly TemporaryWorkspace _workspace = new();
    private string? _originalEnvValue;

    public ProgramConcurrencySmokeTests()
    {
        _originalEnvValue = Environment.GetEnvironmentVariable("FITTOMARKDOWN_MAX_CONCURRENCY");
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable("FITTOMARKDOWN_MAX_CONCURRENCY", _originalEnvValue);
        _workspace.Dispose();
    }

    [Fact]
    public async Task Convert_help_with_valid_concurrency_env_exits_zero()
    {
        Environment.SetEnvironmentVariable("FITTOMARKDOWN_MAX_CONCURRENCY", "1");

        var result = await CliProcessRunner.RunAsync(["convert", "--help"], _workspace.Root);

        result.ExitCode.Should().Be(0);
    }

    [Fact]
    public async Task Convert_help_with_invalid_concurrency_env_exits_zero()
    {
        Environment.SetEnvironmentVariable("FITTOMARKDOWN_MAX_CONCURRENCY", "abc");

        var result = await CliProcessRunner.RunAsync(["convert", "--help"], _workspace.Root);

        result.ExitCode.Should().Be(0);
    }
}
