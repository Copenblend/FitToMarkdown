using Spectre.Console.Cli;

namespace FitToMarkdown.Cli.Commands.Version;

/// <summary>
/// CLI command that displays the installed application version.
/// </summary>
public sealed class VersionCommand : Command<VersionCommandSettings>
{
    private readonly IVersionCommandWorkflow _workflow;

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionCommand"/> class.
    /// </summary>
    /// <param name="workflow">The workflow that displays the version.</param>
    public VersionCommand(IVersionCommandWorkflow workflow)
    {
        _workflow = workflow;
    }

    /// <inheritdoc />
    protected override int Execute(CommandContext context, VersionCommandSettings settings, CancellationToken cancellationToken)
    {
        return _workflow.Execute();
    }
}
