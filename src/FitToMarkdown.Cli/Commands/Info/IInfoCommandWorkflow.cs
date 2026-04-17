namespace FitToMarkdown.Cli.Commands.Info;

/// <summary>
/// Defines the workflow executed by the <c>info</c> command.
/// </summary>
public interface IInfoCommandWorkflow
{
    /// <summary>
    /// Executes the info workflow with the specified settings.
    /// </summary>
    /// <param name="settings">The command settings parsed from CLI arguments.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A process exit code.</returns>
    Task<int> ExecuteAsync(InfoCommandSettings settings, CancellationToken cancellationToken = default);
}
