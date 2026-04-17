using Spectre.Console.Cli;

namespace FitToMarkdown.Cli.Commands.Info;

/// <summary>
/// CLI command that displays FIT file metadata without generating markdown.
/// </summary>
public sealed class InfoCommand : AsyncCommand<InfoCommandSettings>
{
    private readonly IInfoCommandWorkflow _workflow;

    /// <summary>
    /// Initializes a new instance of the <see cref="InfoCommand"/> class.
    /// </summary>
    /// <param name="workflow">The workflow that performs the metadata inspection.</param>
    public InfoCommand(IInfoCommandWorkflow workflow)
    {
        _workflow = workflow;
    }

    /// <inheritdoc />
    protected override Task<int> ExecuteAsync(CommandContext context, InfoCommandSettings settings, CancellationToken cancellationToken)
    {
        return _workflow.ExecuteAsync(settings, cancellationToken);
    }
}
