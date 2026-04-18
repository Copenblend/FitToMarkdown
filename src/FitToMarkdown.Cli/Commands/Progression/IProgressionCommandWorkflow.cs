using Spectre.Console.Cli;

namespace FitToMarkdown.Cli.Commands.Progression;

/// <summary>
/// Defines the workflow executed by the <c>progression</c> command.
/// </summary>
public interface IProgressionCommandWorkflow
{
    /// <summary>
    /// Executes the progression workflow with the specified settings.
    /// </summary>
    /// <param name="settings">The command settings parsed from CLI arguments.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A process exit code.</returns>
    Task<int> ExecuteAsync(ProgressionCommandSettings settings, CancellationToken cancellationToken = default);
}
