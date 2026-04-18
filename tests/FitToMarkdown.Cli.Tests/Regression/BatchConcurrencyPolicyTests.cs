using FitToMarkdown.Cli.Services;
using FluentAssertions;
using Xunit;

namespace FitToMarkdown.Cli.Tests.Regression;

public sealed class BatchConcurrencyPolicyTests : IDisposable
{
    private const string EnvVarName = "FITTOMARKDOWN_MAX_CONCURRENCY";
    private string? _originalValue;

    public BatchConcurrencyPolicyTests()
    {
        _originalValue = Environment.GetEnvironmentVariable(EnvVarName);
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(EnvVarName, _originalValue);
    }

    [Fact]
    public void Single_file_returns_degree_one()
    {
        Environment.SetEnvironmentVariable(EnvVarName, null);
        var policy = new BatchConcurrencyPolicy();

        var degree = policy.GetEffectiveDegreeOfParallelism(isDirectoryMode: false, selectedFileCount: 1);

        degree.Should().Be(1);
    }

    [Fact]
    public void Directory_single_file_returns_degree_one()
    {
        Environment.SetEnvironmentVariable(EnvVarName, null);
        var policy = new BatchConcurrencyPolicy();

        var degree = policy.GetEffectiveDegreeOfParallelism(isDirectoryMode: true, selectedFileCount: 1);

        degree.Should().Be(1);
    }

    [Fact]
    public void Directory_no_env_var_returns_min_of_four_and_processor_count()
    {
        Environment.SetEnvironmentVariable(EnvVarName, null);
        var policy = new BatchConcurrencyPolicy();

        var degree = policy.GetEffectiveDegreeOfParallelism(isDirectoryMode: true, selectedFileCount: 5);

        degree.Should().Be(Math.Min(4, Environment.ProcessorCount));
    }

    [Fact]
    public void Env_var_set_to_two_returns_two()
    {
        Environment.SetEnvironmentVariable(EnvVarName, "2");
        var policy = new BatchConcurrencyPolicy();

        var degree = policy.GetEffectiveDegreeOfParallelism(isDirectoryMode: true, selectedFileCount: 5);

        degree.Should().Be(2);
    }

    [Fact]
    public void Env_var_set_to_zero_clamps_to_one()
    {
        Environment.SetEnvironmentVariable(EnvVarName, "0");
        var policy = new BatchConcurrencyPolicy();

        var degree = policy.GetEffectiveDegreeOfParallelism(isDirectoryMode: true, selectedFileCount: 5);

        degree.Should().Be(1);
    }

    [Fact]
    public void Env_var_set_to_hundred_clamps_to_thirty_two()
    {
        Environment.SetEnvironmentVariable(EnvVarName, "100");
        var policy = new BatchConcurrencyPolicy();

        var degree = policy.GetEffectiveDegreeOfParallelism(isDirectoryMode: true, selectedFileCount: 5);

        degree.Should().Be(32);
    }

    [Fact]
    public void Env_var_set_to_invalid_falls_back_to_default()
    {
        Environment.SetEnvironmentVariable(EnvVarName, "abc");
        var policy = new BatchConcurrencyPolicy();

        var degree = policy.GetEffectiveDegreeOfParallelism(isDirectoryMode: true, selectedFileCount: 5);

        degree.Should().Be(Math.Min(4, Environment.ProcessorCount));
    }

    [Fact]
    public void Invalid_env_var_emits_warning()
    {
        Environment.SetEnvironmentVariable(EnvVarName, "abc");
        var policy = new BatchConcurrencyPolicy();

        var warning = policy.GetConfigurationWarning();

        warning.Should().NotBeNull();
        warning.Should().Contain("abc");
    }

    [Fact]
    public void No_env_var_emits_no_warning()
    {
        Environment.SetEnvironmentVariable(EnvVarName, null);
        var policy = new BatchConcurrencyPolicy();

        var warning = policy.GetConfigurationWarning();

        warning.Should().BeNull();
    }

    [Fact]
    public void Valid_env_var_emits_no_warning()
    {
        Environment.SetEnvironmentVariable(EnvVarName, "8");
        var policy = new BatchConcurrencyPolicy();

        var warning = policy.GetConfigurationWarning();

        warning.Should().BeNull();
    }
}
