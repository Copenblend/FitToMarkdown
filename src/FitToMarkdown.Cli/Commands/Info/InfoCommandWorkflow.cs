using FitToMarkdown.Cli.Configuration;
using FitToMarkdown.Cli.Rendering;
using FitToMarkdown.Cli.Services;
using FitToMarkdown.Core.Abstractions;
using Spectre.Console;

namespace FitToMarkdown.Cli.Commands.Info;

internal sealed class InfoCommandWorkflow : IInfoCommandWorkflow
{
    private readonly IAnsiConsole _console;
    private readonly ICliFileSystem _fileSystem;
    private readonly IFitMetadataInspector _inspector;
    private readonly InfoTableRenderer _tableRenderer;
    private readonly CliExceptionRenderer _exceptionRenderer;

    internal InfoCommandWorkflow(
        IAnsiConsole console,
        ICliFileSystem fileSystem,
        IFitMetadataInspector inspector,
        InfoTableRenderer tableRenderer,
        CliExceptionRenderer exceptionRenderer)
    {
        _console = console;
        _fileSystem = fileSystem;
        _inspector = inspector;
        _tableRenderer = tableRenderer;
        _exceptionRenderer = exceptionRenderer;
    }

    public async Task<int> ExecuteAsync(InfoCommandSettings settings, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.GetFullPath(settings.Path);

        if (!_fileSystem.FileExists(fullPath) ||
            !string.Equals(Path.GetExtension(fullPath), ".fit", StringComparison.OrdinalIgnoreCase))
        {
            _exceptionRenderer.RenderInvalidInput("Path must be an existing .fit file.");
            return CliExitCodes.InvalidInput;
        }

        var result = await _inspector.InspectFileAsync(fullPath, cancellationToken).ConfigureAwait(false);

        if (result.FatalError is not null || result.Summary is null)
        {
            _exceptionRenderer.RenderInvalidInput(result.FatalError?.Message ?? "Unable to read FIT file metadata.");
            return CliExitCodes.TotalFailure;
        }

        _console.Write(_tableRenderer.RenderSummary(result.Summary));

        var devicesTable = _tableRenderer.RenderDevices(result.Summary.Devices);
        if (devicesTable is not null)
        {
            _console.WriteLine();
            _console.Write(devicesTable);
        }

        return CliExitCodes.Success;
    }
}
