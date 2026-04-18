using FitToMarkdown.Cli.Commands.Convert;
using FitToMarkdown.Cli.Commands.Info;
using FitToMarkdown.Cli.Commands.Version;
using Spectre.Console;
using Spectre.Console.Cli;

namespace FitToMarkdown.Cli.Configuration;

/// <summary>
/// Configures the Spectre.Console.Cli command tree, examples, and global error handling.
/// </summary>
public static class CliCommandConfiguration
{
    /// <summary>
    /// Registers all commands, examples, and the global exception handler on the given configurator.
    /// </summary>
    /// <param name="configurator">The Spectre.Console.Cli configurator.</param>
    /// <param name="applicationVersion">The version string to display for the application.</param>
    public static void Configure(IConfigurator configurator, string applicationVersion)
    {
        configurator.SetApplicationName("ftm");
        configurator.SetApplicationVersion(applicationVersion);
        configurator.Settings.CaseSensitivity = CaseSensitivity.None;

        configurator.AddCommand<ConvertCommand>("convert")
            .WithDescription("Convert one FIT file or a directory of FIT files into markdown documents.")
            .WithExample("convert", "activity.fit")
            .WithExample("convert", "activities", "--no-interaction")
            .WithExample("convert", "activities", "--output", "docs", "--overwrite", "ask-each");

        configurator.AddCommand<InfoCommand>("info")
            .WithDescription("Display FIT metadata without generating markdown.")
            .WithExample("info", "activity.fit");

        configurator.AddCommand<VersionCommand>("version")
            .WithDescription("Display the installed application version.")
            .WithExample("version");

        configurator.SetExceptionHandler((ex, _) =>
        {
            if (ex is CommandRuntimeException)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] {Markup.Escape(ex.Message)}");
                return CliExitCodes.InvalidInput;
            }

            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            return CliExitCodes.UnexpectedError;
        });
    }
}
