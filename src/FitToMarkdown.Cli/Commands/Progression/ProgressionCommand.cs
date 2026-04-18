using Spectre.Console.Cli;

namespace FitToMarkdown.Cli.Commands.Progression;

/// <summary>
/// CLI command that builds or updates a sport progression document from FIT files.
/// </summary>
public sealed class ProgressionCommand : AsyncCommand<ProgressionCommandSettings>
{
    private readonly IProgressionCommandWorkflow _workflow;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressionCommand"/> class.
    /// </summary>
    /// <param name="workflow">The workflow that performs the progression build.</param>
    public ProgressionCommand(IProgressionCommandWorkflow workflow)
    {
        _workflow = workflow;
    }

    /// <inheritdoc />
    protected override Task<int> ExecuteAsync(CommandContext context, ProgressionCommandSettings settings, CancellationToken cancellationToken)
    {
        return _workflow.ExecuteAsync(settings, cancellationToken);
    }
}
