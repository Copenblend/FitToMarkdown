namespace FitToMarkdown.Cli.Tests.Fixtures;

/// <summary>
/// Maps named CLI sample scenarios onto the shared real FIT test-data corpus.
/// Path resolution uses the Fit test project's TestData directory.
/// </summary>
internal static class CliSampleCatalog
{
    private static readonly string TestDataRoot = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..",
        "FitToMarkdown.Fit.Tests", "TestData");

    private static readonly Dictionary<string, CliSampleDefinition> Samples = BuildCatalog();

    internal static CliSampleDefinition Get(string key)
    {
        if (!Samples.TryGetValue(key, out var sample))
        {
            throw new InvalidOperationException(
                $"CLI sample key '{key}' not found in catalog. " +
                $"Available keys: {string.Join(", ", Samples.Keys.Order())}");
        }

        return sample;
    }

    internal static IReadOnlyList<CliSampleDefinition> GetByCategory(string category)
    {
        var matches = Samples.Values
            .Where(s => s.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (matches.Count == 0)
        {
            throw new InvalidOperationException(
                $"No CLI samples found for category '{category}'.");
        }

        return matches;
    }

    internal static string GetAbsolutePath(CliSampleDefinition sample)
    {
        var path = Path.GetFullPath(Path.Combine(TestDataRoot, sample.RelativePath));

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                $"CLI sample file not found at '{path}' for key '{sample.Key}'.",
                path);
        }

        return path;
    }

    internal static bool HasTestData()
    {
        var root = Path.GetFullPath(TestDataRoot);
        return Directory.Exists(root) &&
               Directory.EnumerateFiles(root, "*.fit", SearchOption.AllDirectories).Any();
    }

    private static Dictionary<string, CliSampleDefinition> BuildCatalog()
    {
        var catalog = new Dictionary<string, CliSampleDefinition>(StringComparer.OrdinalIgnoreCase);

        catalog["activity-clean"] = new("activity-clean", "Activity/clean_activity.fit", "Activity", true, false);
        catalog["truncated-recoverable"] = new("truncated-recoverable", "ActivityTruncated/truncated_recoverable.fit", "ActivityTruncated", true, true);
        catalog["corrupt-header"] = new("corrupt-header", "Corrupt/corrupt_header.fit", "Corrupt", false, false);
        catalog["multisport"] = new("multisport", "MultiSport/triathlon.fit", "MultiSport", true, false);
        catalog["pool-swim"] = new("pool-swim", "PoolSwim/pool_swim.fit", "PoolSwim", true, false);
        catalog["workout"] = new("workout", "Workout/tempo_run.fit", "Workout", true, false);
        catalog["course"] = new("course", "Course/park_loop.fit", "Course", true, false);
        catalog["monitoring"] = new("monitoring", "Monitoring/daily_monitoring.fit", "Monitoring", true, false);

        return catalog;
    }
}
