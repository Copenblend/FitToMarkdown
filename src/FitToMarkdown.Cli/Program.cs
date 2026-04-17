using System.Reflection;
using FitToMarkdown.Cli.Configuration;
using FitToMarkdown.Cli.DependencyInjection;
using FitToMarkdown.Cli.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

var version = Assembly.GetExecutingAssembly()
    ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
    ?.InformationalVersion ?? "0.0.0";

var services = new ServiceCollection();
services.AddFitToMarkdownApplication();

var registrar = new TypeRegistrar(services);
var app = new CommandApp(registrar);
app.Configure(config => CliCommandConfiguration.Configure(config, version));
return app.Run(args);