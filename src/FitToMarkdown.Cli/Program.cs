using System.Reflection;
using FitToMarkdown.Cli.Configuration;
using Spectre.Console.Cli;

var version = Assembly.GetExecutingAssembly()
    ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
    ?.InformationalVersion ?? "0.0.0";

var app = new CommandApp();
app.Configure(config => CliCommandConfiguration.Configure(config, version));
return app.Run(args);