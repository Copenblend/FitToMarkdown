using System.Diagnostics;

namespace FitToMarkdown.Cli.Tests.Fixtures;

/// <summary>
/// Carries coarse process execution results back to end-to-end tests.
/// </summary>
internal sealed record CliProcessResult(int ExitCode, string StandardOutput, string StandardError);

/// <summary>
/// Runs the built CLI executable for the small Phase 6 process-level end-to-end suite.
/// </summary>
internal static class CliProcessRunner
{
    private static readonly string ExecutablePath = FindExecutable();

    internal static async Task<CliProcessResult> RunAsync(
        string[] arguments,
        string workingDirectory,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(ExecutablePath))
        {
            throw new FileNotFoundException(
                $"CLI executable not found at '{ExecutablePath}'. " +
                "Build the FitToMarkdown.Cli project before running end-to-end tests.",
                ExecutablePath);
        }

        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"\"{ExecutablePath}\" {string.Join(' ', arguments.Select(a => $"\"{a}\""))}",
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        process.Start();

        var stdoutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);

        await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);

        var stdout = await stdoutTask.ConfigureAwait(false);
        var stderr = await stderrTask.ConfigureAwait(false);

        return new CliProcessResult(process.ExitCode, stdout ?? string.Empty, stderr ?? string.Empty);
    }

    private static string FindExecutable()
    {
        // Walk up from the test output directory to find the CLI DLL
        // AppContext.BaseDirectory = tests/Cli.Tests/bin/Debug/net10.0/ → 5 levels up = solution root
        var testDir = AppContext.BaseDirectory;
        var solutionDir = Path.GetFullPath(Path.Combine(testDir, "..", "..", "..", "..", ".."));
        var cliDll = Path.Combine(solutionDir, "src", "FitToMarkdown.Cli", "bin",
#if DEBUG
            "Debug",
#else
            "Release",
#endif
            "net10.0", "FitToMarkdown.Cli.dll");

        return cliDll;
    }
}
