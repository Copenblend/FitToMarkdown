namespace FitToMarkdown.Cli.Commands.Convert;

/// <summary>
/// Defines the workflow executed by the <c>convert</c> command.
/// </summary>
public interface IConvertCommandWorkflow
{
    /// <summary>
    /// Executes the convert workflow with the specified settings.
    /// </summary>
    /// <param name="settings">The command settings parsed from CLI arguments.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A process exit code.</returns>
    Task<int> ExecuteAsync(ConvertCommandSettings settings, CancellationToken cancellationToken = default);
}
