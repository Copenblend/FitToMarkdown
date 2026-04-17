using Spectre.Console.Cli;

namespace FitToMarkdown.Cli.Commands.Convert;

/// <summary>
/// CLI command that converts one or more FIT files into markdown documents.
/// </summary>
public sealed class ConvertCommand : AsyncCommand<ConvertCommandSettings>
{
    private readonly IConvertCommandWorkflow _workflow;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertCommand"/> class.
    /// </summary>
    /// <param name="workflow">The workflow that performs the conversion.</param>
    public ConvertCommand(IConvertCommandWorkflow workflow)
    {
        _workflow = workflow;
    }

    /// <inheritdoc />
    protected override Task<int> ExecuteAsync(CommandContext context, ConvertCommandSettings settings, CancellationToken cancellationToken)
    {
        return _workflow.ExecuteAsync(settings, cancellationToken);
    }
}
