using FluentAssertions;

namespace FitToMarkdown.Cli.Tests.Assertions;

/// <summary>
/// Centralizes semantic console output assertions. Avoids hard-coding fragile
/// border, spinner, or elapsed-time details from Spectre.Console rendering.
/// </summary>
internal static class ConsoleAssertions
{
    internal static void AssertContainsAll(string output, params string[] expectedFragments)
    {
        output.Should().NotBeNullOrEmpty("console output should not be empty");

        foreach (var fragment in expectedFragments)
        {
            output.Should().Contain(fragment,
                $"console output should contain '{fragment}'");
        }
    }

    internal static void AssertContainsNone(string output, params string[] forbiddenFragments)
    {
        foreach (var fragment in forbiddenFragments)
        {
            output.Should().NotContain(fragment,
                $"console output should not contain '{fragment}'");
        }
    }

    internal static void AssertExitCode(int actual, int expected)
    {
        actual.Should().Be(expected,
            $"exit code should be {expected} but was {actual}");
    }
}
