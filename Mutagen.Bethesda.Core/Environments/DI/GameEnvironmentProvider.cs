﻿using Mutagen.Bethesda.Assets.DI;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Environments.DI;

public interface IGameEnvironmentProvider<TModSetter, TModGetter> 
    where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter 
    where TModGetter : class, IContextGetterMod<TModSetter, TModGetter>
{
    IGameEnvironment<TModSetter, TModGetter> Construct(LinkCachePreferences? linkCachePrefs = null);
}
    
public interface IGameEnvironmentProvider<TMod> 
    where TMod : class, IModGetter
{
    IGameEnvironment<TMod> Construct(LinkCachePreferences? linkCachePrefs = null);
}
    
public interface IGameEnvironmentProvider
{
    IGameEnvironment Construct(LinkCachePreferences? linkCachePrefs = null);
}

public sealed class GameEnvironmentProvider : IGameEnvironmentProvider
{
    private readonly IGameReleaseContext _gameReleaseContext;
    private readonly ILoadOrderImporter _loadOrderImporter;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IPluginListingsPathContext _pluginListingsPathContext;
    private readonly ICreationClubListingsPathProvider _cccPath;
    private readonly IAssetProvider _assetProvider;

    public GameEnvironmentProvider(
        IGameReleaseContext gameReleaseContext,
        ILoadOrderImporter loadOrderImporter,
        IDataDirectoryProvider dataDirectoryProvider,
        IPluginListingsPathContext pluginListingsPathContext,
        ICreationClubListingsPathProvider cccPath,
        IAssetProvider assetProvider)
    {
        _gameReleaseContext = gameReleaseContext;
        _loadOrderImporter = loadOrderImporter;
        _dataDirectoryProvider = dataDirectoryProvider;
        _pluginListingsPathContext = pluginListingsPathContext;
        _cccPath = cccPath;
        _assetProvider = assetProvider;
    }

    public IGameEnvironment Construct(LinkCachePreferences? linkCachePrefs = null)        
    {
        var loadOrder = _loadOrderImporter.Import();

        return new GameEnvironmentState<IModGetter>(
            gameRelease: _gameReleaseContext.Release,
            dataFolderPath: _dataDirectoryProvider.Path,
            loadOrderFilePath: _pluginListingsPathContext.Path,
            creationClubListingsFilePath: _cccPath.Path,
            loadOrder: loadOrder,
            linkCache: loadOrder.ToUntypedImmutableLinkCache(linkCachePrefs),
            assetProvider: _assetProvider,
            dispose: true);
    }
}

public sealed class GameEnvironmentProvider<TMod> : IGameEnvironmentProvider<TMod>
    where TMod : class, IModGetter
{
    private readonly IGameReleaseContext _gameReleaseContext;
    private readonly ILoadOrderImporter<TMod> _loadOrderImporter;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IPluginListingsPathContext _pluginListingsPathContext;
    private readonly ICreationClubListingsPathProvider _cccPath;
    private readonly IAssetProvider _assetProvider;

    public GameEnvironmentProvider(
        IGameReleaseContext gameReleaseContext,
        ILoadOrderImporter<TMod> loadOrderImporter,
        IDataDirectoryProvider dataDirectoryProvider,
        IPluginListingsPathContext pluginListingsPathContext,
        ICreationClubListingsPathProvider cccPath,
        IAssetProvider assetProvider)
    {
        _gameReleaseContext = gameReleaseContext;
        _loadOrderImporter = loadOrderImporter;
        _dataDirectoryProvider = dataDirectoryProvider;
        _pluginListingsPathContext = pluginListingsPathContext;
        _cccPath = cccPath;
        _assetProvider = assetProvider;
    }

    public IGameEnvironment<TMod> Construct(LinkCachePreferences? linkCachePrefs = null)        
    {
        var loadOrder = _loadOrderImporter.Import();

        return new GameEnvironmentState<TMod>(
            gameRelease: _gameReleaseContext.Release,
            dataFolderPath: _dataDirectoryProvider.Path,
            loadOrderFilePath: _pluginListingsPathContext.Path,
            creationClubListingsFilePath: _cccPath.Path,
            loadOrder: loadOrder,
            linkCache: loadOrder.ToUntypedImmutableLinkCache(linkCachePrefs),
            assetProvider: _assetProvider,
            dispose: true);
    }
}

public sealed class GameEnvironmentProvider<TModSetter, TModGetter> : IGameEnvironmentProvider<TModSetter, TModGetter>
    where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TModSetter, TModGetter>
{
    private readonly IGameReleaseContext _gameReleaseContext;
    private readonly ILoadOrderImporter<TModGetter> _loadOrderImporter;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IPluginListingsPathContext _pluginListingsPathContext;
    private readonly ICreationClubListingsPathProvider _cccPath;
    private readonly IAssetProvider _assetProvider;

    public GameEnvironmentProvider(
        IGameReleaseContext gameReleaseContext,
        ILoadOrderImporter<TModGetter> loadOrderImporter,
        IDataDirectoryProvider dataDirectoryProvider,
        IPluginListingsPathContext pluginListingsPathContext,
        ICreationClubListingsPathProvider cccPath,
        IAssetProvider assetProvider)
    {
        _gameReleaseContext = gameReleaseContext;
        _loadOrderImporter = loadOrderImporter;
        _dataDirectoryProvider = dataDirectoryProvider;
        _pluginListingsPathContext = pluginListingsPathContext;
        _cccPath = cccPath;
        _assetProvider = assetProvider;
    }

    public IGameEnvironment<TModSetter, TModGetter> Construct(LinkCachePreferences? linkCachePrefs = null)        
    {
        var loadOrder = _loadOrderImporter.Import();

        return new GameEnvironmentState<TModSetter, TModGetter>(
            gameRelease: _gameReleaseContext.Release,
            dataFolderPath: _dataDirectoryProvider.Path,
            loadOrderFilePath: _pluginListingsPathContext.Path,
            creationClubListingsFilePath: _cccPath.Path,
            loadOrder: loadOrder,
            linkCache: loadOrder.ToImmutableLinkCache<TModSetter, TModGetter>(linkCachePrefs),
            assetProvider: _assetProvider,
            dispose: true);
    }
}