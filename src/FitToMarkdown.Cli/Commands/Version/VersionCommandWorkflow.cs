using FitToMarkdown.Cli.Configuration;
using FitToMarkdown.Cli.Rendering;
using FitToMarkdown.Cli.Services;
using Spectre.Console;

namespace FitToMarkdown.Cli.Commands.Version;

internal sealed class VersionCommandWorkflow : IVersionCommandWorkflow
{
    private readonly IAnsiConsole _console;
    private readonly CliVersionProvider _versionProvider;

    internal VersionCommandWorkflow(IAnsiConsole console, CliVersionProvider versionProvider)
    {
        _console = console;
        _versionProvider = versionProvider;
    }

    public int Execute()
    {
        var version = _versionProvider.GetDisplayVersion();
        _console.MarkupLine($"[cyan]ftm[/] {CliMarkup.Escape(version)}");
        return CliExitCodes.Success;
    }
}
