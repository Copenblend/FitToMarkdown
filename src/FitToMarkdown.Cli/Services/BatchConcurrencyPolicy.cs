namespace FitToMarkdown.Cli.Services;

/// <summary>
/// Resolves the effective degree of parallelism for directory-batch conversion.
/// </summary>
internal sealed class BatchConcurrencyPolicy
{
    private const string EnvVarName = "FITTOMARKDOWN_MAX_CONCURRENCY";
    private const int MinDegree = 1;
    private const int MaxDegree = 32;
    private const int DefaultMaxDegree = 4;

    /// <summary>
    /// Gets the effective degree of parallelism for the batch.
    /// </summary>
    internal int GetEffectiveDegreeOfParallelism(bool isDirectoryMode, int selectedFileCount)
    {
        if (!isDirectoryMode || selectedFileCount <= 1)
            return 1;

        var envValue = Environment.GetEnvironmentVariable(EnvVarName);
        if (string.IsNullOrWhiteSpace(envValue))
            return Math.Min(DefaultMaxDegree, Environment.ProcessorCount);

        if (int.TryParse(envValue, out var parsed))
            return Math.Clamp(parsed, MinDegree, MaxDegree);

        // Invalid — fall back to default (warning emitted separately)
        return Math.Min(DefaultMaxDegree, Environment.ProcessorCount);
    }

    /// <summary>
    /// Gets a warning if the configured environment variable value is invalid.
    /// </summary>
    internal string? GetConfigurationWarning()
    {
        var envValue = Environment.GetEnvironmentVariable(EnvVarName);
        if (string.IsNullOrWhiteSpace(envValue))
            return null;

        if (!int.TryParse(envValue, out _))
            return $"Invalid {EnvVarName} value '{envValue}'. Using default concurrency.";

        return null;
    }
}
