using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Installs.DI;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface IPluginListingsPathContext
{
    /// <summary>
    /// Returns expected location of the plugin load order file.
    /// Throws if the path cannot be determined.
    /// </summary>
    /// <returns>Expected path to load order file</returns>
    /// <exception cref="InvalidOperationException">If path cannot be determined (e.g., on non-Windows platforms)</exception>
    FilePath Path { get; }

    /// <summary>
    /// Attempts to get the expected location of the plugin load order file.
    /// </summary>
    /// <param name="path">The path if it could be determined</param>
    /// <returns>True if the path was determined, false otherwise</returns>
    bool TryGetPath(out FilePath path);
}

public sealed class PluginListingsPathContext : IPluginListingsPathContext
{
    private readonly IPluginListingsPathProvider _provider;
    private readonly IGameReleaseContext _gameReleaseContext;

    public PluginListingsPathContext(
        IPluginListingsPathProvider provider,
        IGameReleaseContext gameReleaseContext)
    {
        _provider = provider;
        _gameReleaseContext = gameReleaseContext;
    }

    /// <inheritdoc />
    public FilePath Path
    {
        get
        {
            var path = _provider.Get(_gameReleaseContext.Release);
            if (path == null)
            {
                throw new InvalidOperationException(
                    $"Could not determine plugin listings path for {_gameReleaseContext.Release}. " +
                    "This typically occurs on non-Windows platforms where the LocalAppData environment variable is not set.");
            }
            return path.Value;
        }
    }

    /// <inheritdoc />
    public bool TryGetPath(out FilePath path)
    {
        var result = _provider.Get(_gameReleaseContext.Release);
        if (result == null)
        {
            path = default;
            return false;
        }
        path = result.Value;
        return true;
    }
}

public sealed record PluginListingsPathInjection(FilePath Path) : IPluginListingsPathContext
{
    public bool TryGetPath(out FilePath path)
    {
        path = Path;
        return true;
    }
}
