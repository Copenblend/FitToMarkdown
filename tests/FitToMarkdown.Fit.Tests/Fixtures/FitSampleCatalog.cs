using FitToMarkdown.Core.Enums;

namespace FitToMarkdown.Fit.Tests.Fixtures;

/// <summary>
/// Centralizes lookup of real FIT samples. Individual tests reference samples by key
/// rather than duplicating relative file-system paths.
/// </summary>
internal static class FitSampleCatalog
{
    private static readonly string TestDataRoot = Path.Combine(
        AppContext.BaseDirectory, "TestData");

    private static readonly Dictionary<string, FitSampleDefinition> Samples = BuildCatalog();

    internal static FitSampleDefinition Get(string key)
    {
        if (!Samples.TryGetValue(key, out var sample))
        {
            throw new InvalidOperationException(
                $"FIT sample key '{key}' not found in catalog. " +
                $"Available keys: {string.Join(", ", Samples.Keys.Order())}");
        }

        return sample;
    }

    internal static IReadOnlyList<FitSampleDefinition> GetByCategory(string category)
    {
        var matches = Samples.Values
            .Where(s => s.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (matches.Count == 0)
        {
            throw new InvalidOperationException(
                $"No FIT samples found for category '{category}'. " +
                $"Available categories: {string.Join(", ", Samples.Values.Select(s => s.Category).Distinct().Order())}");
        }

        return matches;
    }

    internal static string GetAbsolutePath(FitSampleDefinition sample)
    {
        var path = Path.Combine(TestDataRoot, sample.RelativePath);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                $"FIT sample file not found at '{path}' for key '{sample.Key}'. " +
                "Ensure the TestData directory is populated.",
                path);
        }

        return path;
    }

    internal static bool HasTestData()
    {
        return Directory.Exists(TestDataRoot) &&
               Directory.EnumerateFiles(TestDataRoot, "*.fit", SearchOption.AllDirectories).Any();
    }

    private static Dictionary<string, FitSampleDefinition> BuildCatalog()
    {
        var catalog = new Dictionary<string, FitSampleDefinition>(StringComparer.OrdinalIgnoreCase);

        Add(catalog, "activity-clean", "Activity/clean_activity.fit", "Clean single-sport activity", "Activity",
            FitParseStatus.Succeeded, FitMessageOrderingMode.SummaryLast, false, false);

        Add(catalog, "activity-summary-first", "Activity/summary_first.fit", "Summary-first activity", "Activity",
            FitParseStatus.Succeeded, FitMessageOrderingMode.SummaryFirst, false, false);

        Add(catalog, "truncated-recoverable", "ActivityTruncated/truncated_recoverable.fit", "Truncated with usable records", "ActivityTruncated",
            FitParseStatus.PartiallySucceeded, FitMessageOrderingMode.Unknown, true, true);

        Add(catalog, "truncated-unrecoverable", "ActivityTruncated/truncated_empty.fit", "Truncated with no recovery path", "ActivityTruncated",
            FitParseStatus.Failed, FitMessageOrderingMode.Unknown, false, false);

        Add(catalog, "multisport", "MultiSport/triathlon.fit", "Multisport triathlon", "MultiSport",
            FitParseStatus.Succeeded, FitMessageOrderingMode.SummaryLast, false, false);

        Add(catalog, "pool-swim", "PoolSwim/pool_swim.fit", "Pool swim with lengths", "PoolSwim",
            FitParseStatus.Succeeded, FitMessageOrderingMode.SummaryLast, false, false);

        Add(catalog, "workout", "Workout/tempo_run.fit", "Workout definition", "Workout",
            FitParseStatus.Succeeded, FitMessageOrderingMode.Unknown, false, false);

        Add(catalog, "course", "Course/park_loop.fit", "Course definition", "Course",
            FitParseStatus.Succeeded, FitMessageOrderingMode.Unknown, false, false);

        Add(catalog, "monitoring", "Monitoring/daily_monitoring.fit", "Daily monitoring", "Monitoring",
            FitParseStatus.Succeeded, FitMessageOrderingMode.Unknown, false, false);

        Add(catalog, "developer-fields", "DeveloperFields/developer_fields.fit", "Developer field file", "DeveloperFields",
            FitParseStatus.Succeeded, FitMessageOrderingMode.SummaryLast, false, false);

        Add(catalog, "corrupt-header", "Corrupt/corrupt_header.fit", "Corrupt file header", "Corrupt",
            FitParseStatus.Failed, FitMessageOrderingMode.Unknown, false, false);

        return catalog;
    }

    private static void Add(
        Dictionary<string, FitSampleDefinition> catalog,
        string key, string relativePath, string scenario, string category,
        FitParseStatus status, FitMessageOrderingMode ordering,
        bool syntheticActivity, bool syntheticSessions)
    {
        catalog[key] = new FitSampleDefinition(
            key, relativePath, scenario, category, status, ordering, syntheticActivity, syntheticSessions);
    }
}
