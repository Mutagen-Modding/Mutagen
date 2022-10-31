using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Installs.DI;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface IPluginListingsPathContext
{
    /// <summary>
    /// Returns expected location of the plugin load order file
    /// </summary>
    /// <returns>Expected path to load order file</returns>
    FilePath Path { get; }
}

public sealed class PluginListingsPathContext : IPluginListingsPathContext
{
    private readonly IPluginListingsPathProvider _provider;
    private readonly IGameInstallModeContext _gameInstallModeContext;
    private readonly IGameReleaseContext _gameReleaseContext;

    public PluginListingsPathContext(
        IPluginListingsPathProvider provider,
        IGameInstallModeContext gameInstallModeContext, 
        IGameReleaseContext gameReleaseContext)
    {
        _provider = provider;
        _gameInstallModeContext = gameInstallModeContext;
        _gameReleaseContext = gameReleaseContext;
    }

    /// <inheritdoc />
    public FilePath Path => _provider.Get(_gameReleaseContext.Release, _gameInstallModeContext.InstallMode);
}

public sealed record PluginListingsPathInjection(FilePath Path) : IPluginListingsPathContext;
