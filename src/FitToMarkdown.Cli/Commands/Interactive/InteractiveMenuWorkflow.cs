using FitToMarkdown.Cli.Commands.Convert;
using FitToMarkdown.Cli.Commands.Info;
using FitToMarkdown.Cli.Commands.Version;
using FitToMarkdown.Cli.Configuration;
using FitToMarkdown.Cli.Models;
using FitToMarkdown.Cli.Rendering;
using FitToMarkdown.Cli.Services;
using Spectre.Console;

namespace FitToMarkdown.Cli.Commands.Interactive;

internal sealed class InteractiveMenuWorkflow
{
    private readonly IAnsiConsole _console;
    private readonly IConvertCommandWorkflow _convertWorkflow;
    private readonly IInfoCommandWorkflow _infoWorkflow;
    private readonly IVersionCommandWorkflow _versionWorkflow;
    private readonly InputPathResolver _pathResolver;
    private readonly CliExceptionRenderer _exceptionRenderer;

    internal InteractiveMenuWorkflow(
        IAnsiConsole console,
        IConvertCommandWorkflow convertWorkflow,
        IInfoCommandWorkflow infoWorkflow,
        IVersionCommandWorkflow versionWorkflow,
        InputPathResolver pathResolver,
        CliExceptionRenderer exceptionRenderer)
    {
        _console = console;
        _convertWorkflow = convertWorkflow;
        _infoWorkflow = infoWorkflow;
        _versionWorkflow = versionWorkflow;
        _pathResolver = pathResolver;
        _exceptionRenderer = exceptionRenderer;
    }

    internal async Task<int> RunAsync(CancellationToken cancellationToken = default)
    {
        while (true)
        {
            _console.Clear();
            _console.MarkupLine("[cyan]  _____ _____ __  __ [/]");
            _console.MarkupLine("[cyan] |  ___|_   _|  \\/  |[/]");
            _console.MarkupLine("[cyan] | |_    | | | |\\/| |[/]");
            _console.MarkupLine("[cyan] |  _|   | | | |  | |[/]");
            _console.MarkupLine("[cyan] |_|     |_| |_|  |_|[/]");
            _console.MarkupLine("[dim]  FIT to Markdown converter[/]");
            _console.WriteLine();

            string action;
            try
            {
                action = _console.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold]What would you like to do?[/]")
                        .PageSize(6)
                        .WrapAround()
                        .HighlightStyle(CliTheme.HighlightStyle)
                        .AddChoices(
                            "Convert FIT files to Markdown",
                            "Inspect FIT file metadata",
                            "Show version",
                            "Exit"));
            }
            catch (NotSupportedException)
            {
                _console.MarkupLine("[yellow]Interactive mode requires a terminal with keyboard input.[/]");
                _console.MarkupLine("[dim]Run with a command instead: ftm convert, ftm info, or ftm --help[/]");
                return CliExitCodes.InvalidInput;
            }

            if (action == "Exit")
            {
                break;
            }

            switch (action)
            {
                case "Convert FIT files to Markdown":
                    await _convertWorkflow.ExecuteAsync(new ConvertCommandSettings(), cancellationToken)
                        .ConfigureAwait(false);
                    break;

                case "Inspect FIT file metadata":
                    await HandleInspectAsync(cancellationToken).ConfigureAwait(false);
                    break;

                case "Show version":
                    _versionWorkflow.Execute();
                    break;
            }

            _console.WriteLine();
            _console.MarkupLine("[dim]Press any key to return to menu...[/]");
            _console.Input.ReadKey(true);
        }

        return CliExitCodes.Success;
    }

    private async Task HandleInspectAsync(CancellationToken cancellationToken)
    {
        var result = await _pathResolver.ResolveAsync(null, false, cancellationToken).ConfigureAwait(false);

        if (result is null)
        {
            _console.MarkupLine("[dim]Cancelled.[/]");
            return;
        }

        if (result.Kind == InputTargetKind.Directory)
        {
            _exceptionRenderer.RenderInvalidInput("Please select a .fit file, not a directory.");
            return;
        }

        await _infoWorkflow.ExecuteAsync(
            new InfoCommandSettings { Path = result.FullPath },
            cancellationToken).ConfigureAwait(false);
    }
}
