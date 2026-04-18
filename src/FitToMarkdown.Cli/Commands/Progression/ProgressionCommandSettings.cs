using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace FitToMarkdown.Cli.Commands.Progression;

/// <summary>
/// Command-line settings for the <c>progression</c> command.
/// </summary>
public class ProgressionCommandSettings : CommandSettings
{
    /// <summary>Gets the path to a directory containing .fit files.</summary>
    [CommandArgument(0, "[path]")]
    [Description("Path to a directory containing .fit files.")]
    public string? Path { get; init; }

    /// <summary>Gets the output directory for the progression document.</summary>
    [CommandOption("-o|--output <DIRECTORY>")]
    [Description("Output directory for the progression markdown file.")]
    public string? OutputDirectory { get; init; }

    /// <summary>Gets the sport name to filter files by.</summary>
    [CommandOption("-s|--sport <SPORT>")]
    [Description("Sport to build progression for (e.g., Running, Cycling).")]
    public string? Sport { get; init; }

    /// <summary>Gets the path to a single .fit file to add to an existing progression.</summary>
    [CommandOption("--add <FILE>")]
    [Description("Path to a single .fit file to add to an existing progression document.")]
    public string? AddFile { get; init; }

    /// <summary>Gets the path to an existing progression document to add to.</summary>
    [CommandOption("--progression-file <FILE>")]
    [Description("Path to an existing progression .md file to insert into.")]
    public string? ProgressionFile { get; init; }

    /// <summary>Gets a value indicating whether interactive prompts are disabled.</summary>
    [CommandOption("--no-interaction")]
    [Description("Disable interactive prompts.")]
    public bool NoInteraction { get; init; }

    /// <inheritdoc />
    public override ValidationResult Validate()
    {
        if (AddFile is not null && ProgressionFile is null)
        {
            return ValidationResult.Error(
                "The '--progression-file' option is required when using '--add'.");
        }

        if (ProgressionFile is not null && AddFile is null)
        {
            return ValidationResult.Error(
                "The '--add' option is required when using '--progression-file'.");
        }

        if (OutputDirectory is not null && string.IsNullOrWhiteSpace(OutputDirectory))
        {
            return ValidationResult.Error("Output directory cannot be empty or whitespace.");
        }

        return ValidationResult.Success();
    }
}
