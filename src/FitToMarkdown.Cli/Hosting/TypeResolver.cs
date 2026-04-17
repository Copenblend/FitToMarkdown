using Spectre.Console.Cli;

namespace FitToMarkdown.Cli.Hosting;

/// <summary>
/// Resolves services from a built <see cref="IServiceProvider"/>.
/// </summary>
internal sealed class TypeResolver : ITypeResolver, IDisposable
{
    private readonly IServiceProvider _provider;

    internal TypeResolver(IServiceProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    /// <inheritdoc />
    public object? Resolve(Type? type)
    {
        return type is null ? null : _provider.GetService(type);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_provider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
