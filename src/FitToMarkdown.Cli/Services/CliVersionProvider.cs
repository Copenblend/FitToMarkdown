using System.Reflection;

namespace FitToMarkdown.Cli.Services;

internal sealed class CliVersionProvider
{
    internal string GetDisplayVersion()
    {
        return Assembly.GetEntryAssembly()
            ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? "0.0.0";
    }
}
