using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Installs;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Implicit.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.DI;
using Noggog;

namespace Mutagen.Bethesda.Environments;

public class GameEnvironment
{
    public static readonly GameEnvironment Typical = new();

    private GameEnvironment()
    {
    }
        
    public IGameEnvironmentState<TModSetter, TModGetter> Construct<TModSetter, TModGetter>(
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

    public IGameEnvironmentState<TModGetter> Construct<TModGetter>(
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

    public IGameEnvironmentState Construct(
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

public interface IGameEnvironmentState : IDisposable
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
}

public interface IGameEnvironmentState<TMod> : IGameEnvironmentState 
    where TMod : class, IModGetter
{
    /// <summary>
    /// Load Order object containing all the mods present in the environment.
    /// </summary>
    new ILoadOrder<IModListing<TMod>> LoadOrder { get; }
}

public interface IGameEnvironmentState<TModSetter, TModGetter> : IGameEnvironmentState<TModGetter> 
    where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TModSetter, TModGetter>
{
    /// <summary>
    /// Load Order object containing all the mods present in the environment.
    /// </summary>
    new ILoadOrder<IModListing<TModGetter>> LoadOrder { get; }

    /// <summary>
    /// Convenience Link Cache to use created from the provided Load Order object
    /// </summary>
    new ILinkCache<TModSetter, TModGetter> LinkCache { get; }
}

/// <summary>
/// A class housing commonly used utilities when interacting with a game environment
/// </summary>
public class GameEnvironmentState :
    IDataDirectoryProvider,
    IPluginListingsPathProvider,
    ICreationClubListingsPathProvider,
    IGameEnvironmentState
{
    private readonly bool _dispose;

    public DirectoryPath DataFolderPath { get; }

    public GameRelease GameRelease { get; }
    public FilePath LoadOrderFilePath { get; }

    public FilePath? CreationClubListingsFilePath { get; }

    public ILinkCache LinkCache { get; }

    public ILoadOrderGetter<IModListingGetter<IModGetter>> LoadOrder { get; }

    public GameEnvironmentState(
        GameRelease gameRelease,
        DirectoryPath dataFolderPath,
        FilePath loadOrderFilePath,
        FilePath? creationClubListingsFilePath,
        ILoadOrderGetter<IModListingGetter<IModGetter>> loadOrder,
        ILinkCache linkCache,
        bool dispose = true)
    {
        GameRelease = gameRelease;
        LoadOrderFilePath = loadOrderFilePath;
        DataFolderPath = dataFolderPath;
        CreationClubListingsFilePath = creationClubListingsFilePath;
        LoadOrder = loadOrder;
        LinkCache = linkCache;
        _dispose = dispose;
    }

    public void Dispose()
    {
        if (!_dispose) return;
        LoadOrder.Dispose();
        LinkCache.Dispose();
    }

    public static IGameEnvironmentState Construct(
        GameRelease release,
        DirectoryPath gameFolder,
        LinkCachePreferences? linkCachePrefs = null)
    {
        Warmup.Init();
        var dataDirectory = new DataDirectoryInjection(Path.Combine(gameFolder, "Data"));
        var gameReleaseInjection = new GameReleaseInjection(release);
        var pluginRawListingsReader = new PluginRawListingsReader(
            IFileSystemExt.DefaultFilesystem,
            new PluginListingsParser(
                new ModListingParser(
                    new HasEnabledMarkersProvider(
                        gameReleaseInjection))));
        var category = new GameCategoryContext(gameReleaseInjection);
        var pluginListingsPathProvider = new PluginListingsPathProvider(gameReleaseInjection);
        var creationClubListingsPathProvider = new CreationClubListingsPathProvider(
            category,
            new CreationClubEnabledProvider(
                category),
            new GameDirectoryInjection(gameFolder));
        return new GameEnvironmentProvider<IModGetter>(
                gameReleaseInjection,
                new LoadOrderImporter<IModGetter>(
                    IFileSystemExt.DefaultFilesystem,
                    dataDirectory,
                    new LoadOrderListingsProvider(
                        new OrderListings(),
                        new ImplicitListingsProvider(
                            IFileSystemExt.DefaultFilesystem,
                            dataDirectory,
                            new ImplicitListingModKeyProvider(
                                gameReleaseInjection)),
                        new PluginListingsProvider(
                            gameReleaseInjection,
                            new TimestampedPluginListingsProvider(
                                new TimestampAligner(IFileSystemExt.DefaultFilesystem),
                                new TimestampedPluginListingsPreferences() { ThrowOnMissingMods = false },
                                pluginRawListingsReader,
                                dataDirectory,
                                pluginListingsPathProvider),
                            new EnabledPluginListingsProvider(
                                pluginRawListingsReader,
                                pluginListingsPathProvider)),
                        new CreationClubListingsProvider(
                            IFileSystemExt.DefaultFilesystem,
                            dataDirectory,
                            creationClubListingsPathProvider,
                            new CreationClubRawListingsReader())),
                    new ModImporter(
                        IFileSystemExt.DefaultFilesystem,
                        gameReleaseInjection)),
                dataDirectory,
                pluginListingsPathProvider,
                creationClubListingsPathProvider)
            .Construct();
    }

    DirectoryPath IDataDirectoryProvider.Path => DataFolderPath;

    FilePath IPluginListingsPathProvider.Path => LoadOrderFilePath;

    FilePath? ICreationClubListingsPathProvider.Path => CreationClubListingsFilePath;
}

/// <summary>
/// A class housing commonly used utilities when interacting with a game environment
/// </summary>
public class GameEnvironmentState<TMod> :
    IDataDirectoryProvider,
    IPluginListingsPathProvider,
    ICreationClubListingsPathProvider,
    IGameEnvironmentState<TMod>
    where TMod : class, IModGetter
{
    private readonly bool _dispose;

    public DirectoryPath DataFolderPath { get; }

    public GameRelease GameRelease { get; }
    public FilePath LoadOrderFilePath { get; }

    public FilePath? CreationClubListingsFilePath { get; }

    public ILoadOrder<IModListing<TMod>> LoadOrder { get; }

    public ILinkCache LinkCache { get; }

    public GameEnvironmentState(
        GameRelease gameRelease,
        DirectoryPath dataFolderPath,
        FilePath loadOrderFilePath,
        FilePath? creationClubListingsFilePath,
        ILoadOrder<IModListing<TMod>> loadOrder,
        ILinkCache linkCache,
        bool dispose = true)
    {
        GameRelease = gameRelease;
        LoadOrderFilePath = loadOrderFilePath;
        DataFolderPath = dataFolderPath;
        CreationClubListingsFilePath = creationClubListingsFilePath;
        LoadOrder = loadOrder;
        LinkCache = linkCache;
        _dispose = dispose;
    }

    public void Dispose()
    {
        if (!_dispose) return;
        LoadOrder.Dispose();
        LinkCache.Dispose();
    }

    public static IGameEnvironmentState<TMod> Construct(
        GameRelease release,
        DirectoryPath gameFolder,
        LinkCachePreferences? linkCachePrefs = null)
    {
        Warmup.Init();
        var dataDirectory = new DataDirectoryInjection(Path.Combine(gameFolder, "Data"));
        var gameReleaseInjection = new GameReleaseInjection(release);
        var pluginRawListingsReader = new PluginRawListingsReader(
            IFileSystemExt.DefaultFilesystem,
            new PluginListingsParser(
                new ModListingParser(
                    new HasEnabledMarkersProvider(
                        gameReleaseInjection))));
        var category = new GameCategoryContext(gameReleaseInjection);
        var pluginListingsPathProvider = new PluginListingsPathProvider(gameReleaseInjection);
        var creationClubListingsPathProvider = new CreationClubListingsPathProvider(
            category,
            new CreationClubEnabledProvider(
                category),
            new GameDirectoryInjection(gameFolder));
        return new GameEnvironmentProvider<TMod>(
                gameReleaseInjection,
                new LoadOrderImporter<TMod>(
                    IFileSystemExt.DefaultFilesystem,
                    dataDirectory,
                    new LoadOrderListingsProvider(
                        new OrderListings(),
                        new ImplicitListingsProvider(
                            IFileSystemExt.DefaultFilesystem,
                            dataDirectory,
                            new ImplicitListingModKeyProvider(
                                gameReleaseInjection)),
                        new PluginListingsProvider(
                            gameReleaseInjection,
                            new TimestampedPluginListingsProvider(
                                new TimestampAligner(IFileSystemExt.DefaultFilesystem),
                                new TimestampedPluginListingsPreferences() { ThrowOnMissingMods = false },
                                pluginRawListingsReader,
                                dataDirectory,
                                pluginListingsPathProvider),
                            new EnabledPluginListingsProvider(
                                pluginRawListingsReader,
                                pluginListingsPathProvider)),
                        new CreationClubListingsProvider(
                            IFileSystemExt.DefaultFilesystem,
                            dataDirectory,
                            creationClubListingsPathProvider,
                            new CreationClubRawListingsReader())),
                    new ModImporter<TMod>(
                        IFileSystemExt.DefaultFilesystem,
                        gameReleaseInjection)),
                dataDirectory,
                pluginListingsPathProvider,
                creationClubListingsPathProvider)
            .Construct();
    }

    DirectoryPath IDataDirectoryProvider.Path => DataFolderPath;

    FilePath IPluginListingsPathProvider.Path => LoadOrderFilePath;

    FilePath? ICreationClubListingsPathProvider.Path => CreationClubListingsFilePath;

    ILoadOrderGetter<IModListingGetter<IModGetter>> IGameEnvironmentState.LoadOrder => LoadOrder;
}

/// <summary>
/// A class housing commonly used utilities when interacting with a game environment
/// </summary>
public class GameEnvironmentState<TModSetter, TModGetter> : 
    IDataDirectoryProvider, 
    IPluginListingsPathProvider,
    ICreationClubListingsPathProvider,
    IGameEnvironmentState<TModSetter, TModGetter> 
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
    public ILoadOrder<IModListing<TModGetter>> LoadOrder { get; }

    /// <summary>
    /// Convenience Link Cache to use created from the provided Load Order object
    /// </summary>
    public ILinkCache<TModSetter, TModGetter> LinkCache { get; }

    public GameEnvironmentState(
        GameRelease gameRelease,
        DirectoryPath dataFolderPath,
        FilePath loadOrderFilePath,
        FilePath? creationClubListingsFilePath,
        ILoadOrder<IModListing<TModGetter>> loadOrder,
        ILinkCache<TModSetter, TModGetter> linkCache,
        bool dispose = true)
    {
        GameRelease = gameRelease;
        LoadOrderFilePath = loadOrderFilePath;
        DataFolderPath = dataFolderPath;
        CreationClubListingsFilePath = creationClubListingsFilePath;
        LoadOrder = loadOrder;
        LinkCache = linkCache;
        _dispose = dispose;
    }

    public void Dispose()
    {
        if (!_dispose) return;
        LoadOrder.Dispose();
        LinkCache.Dispose();
    }

    public static IGameEnvironmentState<TModSetter, TModGetter> Construct(
        GameRelease release,
        DirectoryPath gameFolder,
        LinkCachePreferences? linkCachePrefs = null)
    {
        Warmup.Init();
        var dataDirectory = new DataDirectoryInjection(Path.Combine(gameFolder, "Data"));
        var gameReleaseInjection = new GameReleaseInjection(release);
        var pluginRawListingsReader = new PluginRawListingsReader(
            IFileSystemExt.DefaultFilesystem,
            new PluginListingsParser(
                new ModListingParser(
                    new HasEnabledMarkersProvider(
                        gameReleaseInjection))));
        var category = new GameCategoryContext(gameReleaseInjection);
        var pluginListingsPathProvider = new PluginListingsPathProvider(gameReleaseInjection);
        var creationClubListingsPathProvider = new CreationClubListingsPathProvider(
            category,
            new CreationClubEnabledProvider(
                category),
            new GameDirectoryInjection(gameFolder));
        return new GameEnvironmentProvider<TModSetter, TModGetter>(
                gameReleaseInjection,
                new LoadOrderImporter<TModGetter>(
                    IFileSystemExt.DefaultFilesystem,
                    dataDirectory,
                    new LoadOrderListingsProvider(
                        new OrderListings(),
                        new ImplicitListingsProvider(
                            IFileSystemExt.DefaultFilesystem,
                            dataDirectory,
                            new ImplicitListingModKeyProvider(
                                gameReleaseInjection)),
                        new PluginListingsProvider(
                            gameReleaseInjection,
                            new TimestampedPluginListingsProvider(
                                new TimestampAligner(IFileSystemExt.DefaultFilesystem),
                                new TimestampedPluginListingsPreferences() {ThrowOnMissingMods = false},
                                pluginRawListingsReader,
                                dataDirectory,
                                pluginListingsPathProvider),
                            new EnabledPluginListingsProvider(
                                pluginRawListingsReader,
                                pluginListingsPathProvider)),
                        new CreationClubListingsProvider(
                            IFileSystemExt.DefaultFilesystem,
                            dataDirectory,
                            creationClubListingsPathProvider,
                            new CreationClubRawListingsReader())),
                    new ModImporter<TModGetter>(
                        IFileSystemExt.DefaultFilesystem,
                        gameReleaseInjection)),
                dataDirectory,
                pluginListingsPathProvider,
                creationClubListingsPathProvider)
            .Construct();
    }

    DirectoryPath IDataDirectoryProvider.Path => DataFolderPath;

    FilePath IPluginListingsPathProvider.Path => LoadOrderFilePath;

    FilePath? ICreationClubListingsPathProvider.Path => CreationClubListingsFilePath;

    ILinkCache IGameEnvironmentState.LinkCache => LinkCache;

    ILoadOrderGetter<IModListingGetter<IModGetter>> IGameEnvironmentState.LoadOrder => LoadOrder;
}