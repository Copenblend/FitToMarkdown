namespace FitToMarkdown.Cli.Commands.Version;

/// <summary>
/// Defines the workflow executed by the <c>version</c> command.
/// </summary>
public interface IVersionCommandWorkflow
{
    /// <summary>
    /// Executes the version display workflow.
    /// </summary>
    /// <returns>A process exit code.</returns>
    int Execute();
}
