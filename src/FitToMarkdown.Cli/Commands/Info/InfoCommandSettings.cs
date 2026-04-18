using System.ComponentModel;
using Spectre.Console.Cli;

namespace FitToMarkdown.Cli.Commands.Info;

/// <summary>
/// Command-line settings for the <c>info</c> command.
/// </summary>
public class InfoCommandSettings : CommandSettings
{
    /// <summary>Gets the path to a .fit file.</summary>
    [CommandArgument(0, "[path]")]
    [Description("Path to a .fit file.")]
    public string? Path { get; init; }
}
