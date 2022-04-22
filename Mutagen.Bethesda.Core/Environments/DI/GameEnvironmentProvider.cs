using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Environments.DI;

public interface IGameEnvironmentProvider<TModSetter, TModGetter> 
    where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter 
    where TModGetter : class, IContextGetterMod<TModSetter, TModGetter>
{
    IGameEnvironmentState<TModSetter, TModGetter> Construct(LinkCachePreferences? linkCachePrefs = null);
}
    
public interface IGameEnvironmentProvider<TMod> 
    where TMod : class, IModGetter
{
    IGameEnvironmentState<TMod> Construct(LinkCachePreferences? linkCachePrefs = null);
}
    
public interface IGameEnvironmentProvider
{
    IGameEnvironmentState Construct(LinkCachePreferences? linkCachePrefs = null);
}

public class GameEnvironmentProvider : IGameEnvironmentProvider
{
    private readonly IGameReleaseContext _gameReleaseContext;
    private readonly ILoadOrderImporter _loadOrderImporter;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IPluginListingsPathProvider _pluginListingsPathProvider;
    private readonly ICreationClubListingsPathProvider _cccPath;

    public GameEnvironmentProvider(
        IGameReleaseContext gameReleaseContext,
        ILoadOrderImporter loadOrderImporter,
        IDataDirectoryProvider dataDirectoryProvider,
        IPluginListingsPathProvider pluginListingsPathProvider,
        ICreationClubListingsPathProvider cccPath)
    {
        _gameReleaseContext = gameReleaseContext;
        _loadOrderImporter = loadOrderImporter;
        _dataDirectoryProvider = dataDirectoryProvider;
        _pluginListingsPathProvider = pluginListingsPathProvider;
        _cccPath = cccPath;
    }

    public IGameEnvironmentState Construct(LinkCachePreferences? linkCachePrefs = null)        
    {
        var loadOrder = _loadOrderImporter.Import();

        return new GameEnvironmentState<IModGetter>(
            gameRelease: _gameReleaseContext.Release,
            dataFolderPath: _dataDirectoryProvider.Path,
            loadOrderFilePath: _pluginListingsPathProvider.Path,
            creationClubListingsFilePath: _cccPath.Path,
            loadOrder: loadOrder,
            linkCache: loadOrder.ToUntypedImmutableLinkCache(linkCachePrefs),
            dispose: true);
    }
}

public class GameEnvironmentProvider<TMod> : IGameEnvironmentProvider<TMod>
    where TMod : class, IModGetter
{
    private readonly IGameReleaseContext _gameReleaseContext;
    private readonly ILoadOrderImporter<TMod> _loadOrderImporter;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IPluginListingsPathProvider _pluginListingsPathProvider;
    private readonly ICreationClubListingsPathProvider _cccPath;

    public GameEnvironmentProvider(
        IGameReleaseContext gameReleaseContext,
        ILoadOrderImporter<TMod> loadOrderImporter,
        IDataDirectoryProvider dataDirectoryProvider,
        IPluginListingsPathProvider pluginListingsPathProvider,
        ICreationClubListingsPathProvider cccPath)
    {
        _gameReleaseContext = gameReleaseContext;
        _loadOrderImporter = loadOrderImporter;
        _dataDirectoryProvider = dataDirectoryProvider;
        _pluginListingsPathProvider = pluginListingsPathProvider;
        _cccPath = cccPath;
    }

    public IGameEnvironmentState<TMod> Construct(LinkCachePreferences? linkCachePrefs = null)        
    {
        var loadOrder = _loadOrderImporter.Import();

        return new GameEnvironmentState<TMod>(
            gameRelease: _gameReleaseContext.Release,
            dataFolderPath: _dataDirectoryProvider.Path,
            loadOrderFilePath: _pluginListingsPathProvider.Path,
            creationClubListingsFilePath: _cccPath.Path,
            loadOrder: loadOrder,
            linkCache: loadOrder.ToUntypedImmutableLinkCache(linkCachePrefs),
            dispose: true);
    }
}

public class GameEnvironmentProvider<TModSetter, TModGetter> : IGameEnvironmentProvider<TModSetter, TModGetter>
    where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TModSetter, TModGetter>
{
    private readonly IGameReleaseContext _gameReleaseContext;
    private readonly ILoadOrderImporter<TModGetter> _loadOrderImporter;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IPluginListingsPathProvider _pluginListingsPathProvider;
    private readonly ICreationClubListingsPathProvider _cccPath;

    public GameEnvironmentProvider(
        IGameReleaseContext gameReleaseContext,
        ILoadOrderImporter<TModGetter> loadOrderImporter,
        IDataDirectoryProvider dataDirectoryProvider,
        IPluginListingsPathProvider pluginListingsPathProvider,
        ICreationClubListingsPathProvider cccPath)
    {
        _gameReleaseContext = gameReleaseContext;
        _loadOrderImporter = loadOrderImporter;
        _dataDirectoryProvider = dataDirectoryProvider;
        _pluginListingsPathProvider = pluginListingsPathProvider;
        _cccPath = cccPath;
    }

    public IGameEnvironmentState<TModSetter, TModGetter> Construct(LinkCachePreferences? linkCachePrefs = null)        
    {
        var loadOrder = _loadOrderImporter.Import();

        return new GameEnvironmentState<TModSetter, TModGetter>(
            gameRelease: _gameReleaseContext.Release,
            dataFolderPath: _dataDirectoryProvider.Path,
            loadOrderFilePath: _pluginListingsPathProvider.Path,
            creationClubListingsFilePath: _cccPath.Path,
            loadOrder: loadOrder,
            linkCache: loadOrder.ToImmutableLinkCache<TModSetter, TModGetter>(linkCachePrefs),
            dispose: true);
    }
}