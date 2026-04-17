using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace FitToMarkdown.Cli.Hosting;

/// <summary>
/// Bridges <see cref="IServiceCollection"/> into Spectre.Console.Cli's type registration model.
/// </summary>
internal sealed class TypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _services;

    internal TypeRegistrar(IServiceCollection services)
    {
        _services = services;
    }

    /// <inheritdoc />
    public ITypeResolver Build()
    {
        return new TypeResolver(_services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
        }));
    }

    /// <inheritdoc />
    public void Register(Type service, Type implementation)
    {
        _services.AddSingleton(service, implementation);
    }

    /// <inheritdoc />
    public void RegisterInstance(Type service, object implementation)
    {
        _services.AddSingleton(service, implementation);
    }

    /// <inheritdoc />
    public void RegisterLazy(Type service, Func<object> factory)
    {
        _services.AddSingleton(service, _ => factory());
    }
}
