using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace FitToMarkdown.Cli.Commands.Convert;

/// <summary>
/// Command-line settings for the <c>convert</c> command.
/// </summary>
public class ConvertCommandSettings : CommandSettings
{
    /// <summary>Gets the path to a .fit file or directory containing .fit files.</summary>
    [CommandArgument(0, "[path]")]
    [Description("Path to a .fit file or directory containing .fit files.")]
    public string? Path { get; init; }

    /// <summary>Gets the output directory for generated markdown files.</summary>
    [CommandOption("-o|--output <DIRECTORY>")]
    [Description("Output directory for generated markdown files.")]
    public string? OutputDirectory { get; init; }

    /// <summary>Gets the overwrite mode: skip, overwrite, or ask-each.</summary>
    [CommandOption("--overwrite <MODE>")]
    [Description("Overwrite mode: skip, overwrite, or ask-each.")]
    public string? Overwrite { get; init; }

    /// <summary>Gets a value indicating whether interactive prompts are disabled.</summary>
    [CommandOption("--no-interaction")]
    [Description("Disable interactive prompts.")]
    public bool NoInteraction { get; init; }

    /// <inheritdoc />
    public override ValidationResult Validate()
    {
        if (NoInteraction && Overwrite is not null &&
            Overwrite.Equals("ask-each", StringComparison.OrdinalIgnoreCase))
        {
            return ValidationResult.Error(
                "Cannot use '--overwrite ask-each' with '--no-interaction'.");
        }

        if (OutputDirectory is not null && string.IsNullOrWhiteSpace(OutputDirectory))
        {
            return ValidationResult.Error("Output directory cannot be empty or whitespace.");
        }

        if (Overwrite is not null &&
            !Overwrite.Equals("skip", StringComparison.OrdinalIgnoreCase) &&
            !Overwrite.Equals("overwrite", StringComparison.OrdinalIgnoreCase) &&
            !Overwrite.Equals("ask-each", StringComparison.OrdinalIgnoreCase))
        {
            return ValidationResult.Error(
                $"Invalid overwrite mode '{Overwrite}'. Must be one of: skip, overwrite, ask-each.");
        }

        return ValidationResult.Success();
    }
}
