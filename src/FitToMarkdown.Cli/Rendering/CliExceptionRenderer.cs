using FitToMarkdown.Cli.Configuration;
using Spectre.Console;

namespace FitToMarkdown.Cli.Rendering;

internal sealed class CliExceptionRenderer
{
    private readonly IAnsiConsole _console;

    internal CliExceptionRenderer(IAnsiConsole console)
    {
        _console = console;
    }

    internal void RenderInvalidInput(string message)
    {
        _console.MarkupLine($"[red]Error:[/] {CliMarkup.Escape(message)}");
    }

    internal void RenderWarning(string message)
    {
        _console.MarkupLine($"[yellow]Warning:[/] {CliMarkup.Escape(message)}");
    }

    internal void RenderUnexpected(Exception exception)
    {
        _console.WriteException(exception, ExceptionFormats.ShortenEverything);
    }

    internal int MapUnexpectedToExitCode(Exception exception)
    {
        return CliExitCodes.UnexpectedError;
    }
}
