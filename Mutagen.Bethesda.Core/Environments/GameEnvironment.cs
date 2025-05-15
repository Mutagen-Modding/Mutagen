using System.IO.Abstractions;
using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Assets.DI;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Inis.DI;
using Mutagen.Bethesda.Installs;
using Mutagen.Bethesda.Installs.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Implicit.DI;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.DI;
using Mutagen.Bethesda.Plugins.Utility;
using Noggog;
using StrongInject;

namespace Mutagen.Bethesda.Environments;

public sealed class GameEnvironment
{
    public static readonly GameEnvironment Typical = new();

    private GameEnvironment()
    {
    }
        
    public IGameEnvironment<TModSetter, TModGetter> Construct<TModSetter, TModGetter>(
        GameRelease release,
        LinkCachePreferences? linkCachePrefs = null)
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter>
    {
        if (!GameLocations.TryGetGameFolder(release, out var gameFolderPath))
        {
            throw new ArgumentException($"Could not find game folder automatically.");
        }

        return GameEnvironmentState<TModSetter, TModGetter>.Construct(release, gameFolderPath, linkCachePrefs);
    }

    public IGameEnvironment<TModGetter> Construct<TModGetter>(
        GameRelease release,
        LinkCachePreferences? linkCachePrefs = null)
        where TModGetter : class, IModGetter
    {
        if (!GameLocations.TryGetGameFolder(release, out var gameFolderPath))
        {
            throw new ArgumentException($"Could not find game folder automatically.");
        }

        return GameEnvironmentState<TModGetter>.Construct(release, gameFolderPath, linkCachePrefs);
    }

    public IGameEnvironment Construct(
        GameRelease release,
        LinkCachePreferences? linkCachePrefs = null)
    {
        if (!GameLocations.TryGetGameFolder(release, out var gameFolderPath))
        {
            throw new ArgumentException($"Could not find game folder automatically.");
        }

        return GameEnvironmentState.Construct(release, gameFolderPath, linkCachePrefs);
    }
}

public interface IGameEnvironment : IDisposable
{
    DirectoryPath DataFolderPath { get; }
    GameRelease GameRelease { get; }
    FilePath LoadOrderFilePath { get; }
    FilePath? CreationClubListingsFilePath { get; }

    /// <summary>
    /// Load Order object containing all the mods present in the environment.
    /// </summary>
    ILoadOrderGetter<IModListingGetter<IModGetter>> LoadOrder { get; }

    /// <summary>
    /// Convenience Link Cache to use created from the provided Load Order object
    /// </summary>
    ILinkCache LinkCache { get; }
    
    /// <summary>
    /// Convenience Asset Provider created from the environment's context
    /// </summary>
    IAssetProvider AssetProvider { get; }
}

public interface IGameEnvironment<TMod> : IGameEnvironment
    where TMod : class, IModGetter
{
    /// <summary>
    /// Load Order object containing all the mods present in the environment.
    /// </summary>
    new ILoadOrderGetter<IModListingGetter<TMod>> LoadOrder { get; }
}

public interface IGameEnvironment<TModSetter, TModGetter> : IGameEnvironment<TModGetter>
    where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TModSetter, TModGetter>
{
    /// <summary>
    /// Load Order object containing all the mods present in the environment.
    /// </summary>
    new ILoadOrderGetter<IModListingGetter<TModGetter>> LoadOrder { get; }

    /// <summary>
    /// Convenience Link Cache to use created from the provided Load Order object
    /// </summary>
    new ILinkCache<TModSetter, TModGetter> LinkCache { get; }
}

[RegisterModule(typeof(MutagenStrongInjectModule))]
[Register(typeof(GameEnvironmentProvider<>))]
partial class GameEnvironmentProviderContainer<TMod> : IContainer<GameEnvironmentProvider<TMod>>
    where TMod : class, IModGetter
{
    [Instance] private readonly IGameReleaseContext _release;
    [Instance] private readonly IDataDirectoryProvider _dataDirectoryProvider;
    [Instance] private readonly IGameDirectoryProvider _gameDirectoryProvider;
    [Instance] private readonly IGameDirectoryLookup _gameDirectoryLookup;
    [Instance] private readonly IFileSystem _fileSystem = IFileSystemExt.DefaultFilesystem;

    public GameEnvironmentProviderContainer(
        IGameReleaseContext release,
        DirectoryPath gameDir)
    {
        _release = release;
        var constants = GameConstants.Get(release.Release);
        _dataDirectoryProvider = new DataDirectoryInjection(Path.Combine(gameDir, constants.DataFolderRelativePath));
        _gameDirectoryProvider = new GameDirectoryInjection(gameDir);
        _gameDirectoryLookup = new GameDirectoryLookupInjection(_release.Release, gameDir.Path);
    }
}

/// <summary>
/// A class housing commonly used utilities when interacting with a game environment
/// </summary>
public sealed class GameEnvironmentState :
    IDataDirectoryProvider,
    IPluginListingsPathContext,
    ICreationClubListingsPathProvider,
    IGameEnvironment
{
    private readonly bool _dispose;

    public DirectoryPath DataFolderPath { get; }

    public GameRelease GameRelease { get; }
    public FilePath LoadOrderFilePath { get; }

    public FilePath? CreationClubListingsFilePath { get; }

    public ILinkCache LinkCache { get; }

    public ILoadOrderGetter<IModListingGetter<IModGetter>> LoadOrder { get; }
    
    public IAssetProvider AssetProvider { get; }

    public GameEnvironmentState(
        GameRelease gameRelease,
        DirectoryPath dataFolderPath,
        FilePath loadOrderFilePath,
        FilePath? creationClubListingsFilePath,
        ILoadOrderGetter<IModListingGetter<IModGetter>> loadOrder,
        ILinkCache linkCache,
        IAssetProvider assetProvider,
        bool dispose = true)
    {
        GameRelease = gameRelease;
        LoadOrderFilePath = loadOrderFilePath;
        DataFolderPath = dataFolderPath;
        CreationClubListingsFilePath = creationClubListingsFilePath;
        LoadOrder = loadOrder;
        LinkCache = linkCache;
        AssetProvider = assetProvider;
        _dispose = dispose;
    }

    public void Dispose()
    {
        if (!_dispose) return;
        LoadOrder.Dispose();
        LinkCache.Dispose();
    }

    public static IGameEnvironment Construct(
        GameRelease release,
        DirectoryPath gameFolder,
        LinkCachePreferences? linkCachePrefs = null)
    {
        Warmup.Init();
        var cont = new GameEnvironmentProviderContainer<IModGetter>(
            new GameReleaseInjection(release),
            gameFolder);
        return cont.Resolve().Value.Construct();
    }

    DirectoryPath IDataDirectoryProvider.Path => DataFolderPath;

    FilePath IPluginListingsPathContext.Path => LoadOrderFilePath;

    FilePath? ICreationClubListingsPathProvider.Path => CreationClubListingsFilePath;
}

/// <summary>
/// A class housing commonly used utilities when interacting with a game environment
/// </summary>
public sealed class GameEnvironmentState<TMod> :
    IDataDirectoryProvider,
    IPluginListingsPathContext,
    ICreationClubListingsPathProvider,
    IGameEnvironment<TMod>
    where TMod : class, IModGetter
{
    private readonly bool _dispose;

    public DirectoryPath DataFolderPath { get; }

    public GameRelease GameRelease { get; }
    public FilePath LoadOrderFilePath { get; }

    public FilePath? CreationClubListingsFilePath { get; }

    public ILoadOrderGetter<IModListingGetter<TMod>> LoadOrder { get; }

    public ILinkCache LinkCache { get; }

    public IAssetProvider AssetProvider { get; }

    public GameEnvironmentState(
        GameRelease gameRelease,
        DirectoryPath dataFolderPath,
        FilePath loadOrderFilePath,
        FilePath? creationClubListingsFilePath,
        ILoadOrder<IModListing<TMod>> loadOrder,
        ILinkCache linkCache,
        IAssetProvider assetProvider,
        bool dispose = true)
    {
        GameRelease = gameRelease;
        LoadOrderFilePath = loadOrderFilePath;
        DataFolderPath = dataFolderPath;
        CreationClubListingsFilePath = creationClubListingsFilePath;
        LoadOrder = loadOrder;
        LinkCache = linkCache;
        _dispose = dispose;
        AssetProvider = assetProvider;
    }

    public void Dispose()
    {
        if (!_dispose) return;
        LoadOrder.Dispose();
        LinkCache.Dispose();
    }

    public static IGameEnvironment<TMod> Construct(
        GameRelease release,
        DirectoryPath gameFolder,
        LinkCachePreferences? linkCachePrefs = null)
    {
        Warmup.Init();
        var cont = new GameEnvironmentProviderContainer<TMod>(
            new GameReleaseInjection(release),
            gameFolder);
        return cont.Resolve().Value.Construct();
    }

    DirectoryPath IDataDirectoryProvider.Path => DataFolderPath;

    FilePath IPluginListingsPathContext.Path => LoadOrderFilePath;

    FilePath? ICreationClubListingsPathProvider.Path => CreationClubListingsFilePath;

    ILoadOrderGetter<IModListingGetter<IModGetter>> IGameEnvironment.LoadOrder => LoadOrder;
}

[RegisterModule(typeof(MutagenStrongInjectModule))]
[Register(typeof(GameEnvironmentProvider<,>))]
partial class GameEnvironmentProviderGenericContainer<TModSetter, TModGetter> : IContainer<GameEnvironmentProvider<TModSetter, TModGetter>>
    where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TModSetter, TModGetter>
{
    [Instance] private readonly IGameReleaseContext _release;
    [Instance] private readonly IDataDirectoryProvider _dataDirectoryProvider;
    [Instance] private readonly IGameDirectoryProvider _gameDirectoryProvider;
    [Instance] private readonly IGameDirectoryLookup _gameDirectoryLookup;
    [Instance] private readonly IFileSystem _fileSystem = IFileSystemExt.DefaultFilesystem;

    public GameEnvironmentProviderGenericContainer(
        IGameReleaseContext release,
        DirectoryPath gameDir)
    {
        _release = release;
        var constants = GameConstants.Get(release.Release);
        _dataDirectoryProvider = new DataDirectoryInjection(Path.Combine(gameDir, constants.DataFolderRelativePath));
        _gameDirectoryProvider = new GameDirectoryInjection(gameDir);
        _gameDirectoryLookup = new GameDirectoryLookupInjection(_release.Release, gameDir.Path);
    }
}

/// <summary>
/// A class housing commonly used utilities when interacting with a game environment
/// </summary>
public sealed class GameEnvironmentState<TModSetter, TModGetter> : 
    IDataDirectoryProvider, 
    IPluginListingsPathContext,
    ICreationClubListingsPathProvider,
    IGameEnvironment<TModSetter, TModGetter> 
    where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TModSetter, TModGetter>
{
    private readonly bool _dispose;

    public DirectoryPath DataFolderPath { get; }

    public GameRelease GameRelease { get; }
    public FilePath LoadOrderFilePath { get; }

    public FilePath? CreationClubListingsFilePath { get; }

    /// <summary>
    /// Load Order object containing all the mods present in the environment.
    /// </summary>
    public ILoadOrderGetter<IModListingGetter<TModGetter>> LoadOrder { get; }

    /// <summary>
    /// Convenience Link Cache to use created from the provided Load Order object
    /// </summary>
    public ILinkCache<TModSetter, TModGetter> LinkCache { get; }
    
    /// <summary>
    /// Convenience Asset Provider created from the environment's context
    /// </summary>
    public IAssetProvider AssetProvider { get; }

    public GameEnvironmentState(
        GameRelease gameRelease,
        DirectoryPath dataFolderPath,
        FilePath loadOrderFilePath,
        FilePath? creationClubListingsFilePath,
        ILoadOrderGetter<IModListingGetter<TModGetter>> loadOrder,
        ILinkCache<TModSetter, TModGetter> linkCache,
        IAssetProvider assetProvider,
        bool dispose = true)
    {
        GameRelease = gameRelease;
        LoadOrderFilePath = loadOrderFilePath;
        DataFolderPath = dataFolderPath;
        CreationClubListingsFilePath = creationClubListingsFilePath;
        LoadOrder = loadOrder;
        LinkCache = linkCache;
        AssetProvider = assetProvider;
        _dispose = dispose;
    }

    public void Dispose()
    {
        if (!_dispose) return;
        LoadOrder.Dispose();
        LinkCache.Dispose();
    }

    public static IGameEnvironment<TModSetter, TModGetter> Construct(
        GameRelease release,
        DirectoryPath gameFolder,
        LinkCachePreferences? linkCachePrefs = null)
    {
        Warmup.Init();
        var cont = new GameEnvironmentProviderGenericContainer<TModSetter, TModGetter>(
            new GameReleaseInjection(release),
            gameFolder);
        return cont.Resolve().Value.Construct();
    }

    DirectoryPath IDataDirectoryProvider.Path => DataFolderPath;

    FilePath IPluginListingsPathContext.Path => LoadOrderFilePath;

    FilePath? ICreationClubListingsPathProvider.Path => CreationClubListingsFilePath;

    ILinkCache IGameEnvironment.LinkCache => LinkCache;

    ILoadOrderGetter<IModListingGetter<IModGetter>> IGameEnvironment.LoadOrder => LoadOrder;
}