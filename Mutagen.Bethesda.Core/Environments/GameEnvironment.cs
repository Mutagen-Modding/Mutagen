using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Assets.DI;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Inis.DI;
using Mutagen.Bethesda.Installs;
using Mutagen.Bethesda.Installs.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Implicit.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.DI;
using Noggog;

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
        var fs = IFileSystemExt.DefaultFilesystem;
        var dataDirectory = new DataDirectoryInjection(Path.Combine(gameFolder, "Data"));
        var gameReleaseInjection = new GameReleaseInjection(release);
        var gameDir = new GameDirectoryInjection(gameFolder);
        var gameDirLookup = new GameDirectoryLookupInjection(release, gameDir.Path);
        var pluginRawListingsReader = new PluginRawListingsReader(
            fs,
            new PluginListingsParser(
                new PluginListingCommentTrimmer(),
                new LoadOrderListingParser(
                    new HasEnabledMarkersProvider(
                        gameReleaseInjection))));
        var category = new GameCategoryContext(gameReleaseInjection);
        var pluginListingsPathProvider = new PluginListingsPathContext(
            new PluginListingsPathProvider(),
            gameReleaseInjection);
        var creationClubListingsPathProvider = new CreationClubListingsPathProvider(
            category,
            new CreationClubEnabledProvider(
                category),
            gameDir);
        var archiveExt = new ArchiveExtensionProvider(gameReleaseInjection);
        var loListingsProvider = new LoadOrderListingsProvider(
            new OrderListings(),
            new ImplicitListingsProvider(
                IFileSystemExt.DefaultFilesystem,
                dataDirectory,
                new ImplicitListingModKeyProvider(
                    gameReleaseInjection)),
            new PluginListingsProvider(
                gameReleaseInjection,
                new TimestampedPluginListingsProvider(
                    IFileSystemExt.DefaultFilesystem,
                    new TimestampAligner(IFileSystemExt.DefaultFilesystem),
                    new TimestampedPluginListingsPreferences() { ThrowOnMissingMods = false },
                    pluginRawListingsReader,
                    dataDirectory,
                    pluginListingsPathProvider),
                new EnabledPluginListingsProvider(
                    IFileSystemExt.DefaultFilesystem,
                    pluginRawListingsReader,
                    pluginListingsPathProvider)),
            new CreationClubListingsProvider(
                IFileSystemExt.DefaultFilesystem,
                dataDirectory,
                creationClubListingsPathProvider,
                new CreationClubRawListingsReader()));
        
        var assetProvider = new GameAssetProvider(
            new DataDirectoryAssetProvider(
                fs,
                dataDirectory),
            new ArchiveAssetProvider(
                fs,
                new GetApplicableArchivePaths(
                    fs,
                    new CheckArchiveApplicability(
                        archiveExt),
                    dataDirectory,
                    archiveExt,
                    new CachedArchiveListingDetailsProvider(
                        loListingsProvider,
                        new GetArchiveIniListings(
                            fs,
                            new IniPathProvider(
                                gameReleaseInjection,
                                new IniPathLookup(
                                    gameDirLookup))))),
                gameReleaseInjection));
        return new GameEnvironmentProvider<IModGetter>(
                gameReleaseInjection,
                new LoadOrderImporter<IModGetter>(
                    IFileSystemExt.DefaultFilesystem,
                    dataDirectory,
                    loListingsProvider,
                    new ModImporter(
                        IFileSystemExt.DefaultFilesystem,
                        gameReleaseInjection),
                    new MasterFlagsLookupProvider(
                        gameReleaseInjection,
                        IFileSystemExt.DefaultFilesystem,
                        dataDirectory)),
                dataDirectory,
                pluginListingsPathProvider,
                creationClubListingsPathProvider,
                assetProvider)
            .Construct();
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
        var fs = IFileSystemExt.DefaultFilesystem;
        var dataDirectory = new DataDirectoryInjection(Path.Combine(gameFolder, "Data"));
        var gameReleaseInjection = new GameReleaseInjection(release);
        var gameDir = new GameDirectoryInjection(gameFolder);
        var gameDirLookup = new GameDirectoryLookupInjection(release, gameDir.Path);
        var pluginRawListingsReader = new PluginRawListingsReader(
            fs,
            new PluginListingsParser(
                new PluginListingCommentTrimmer(),
                new LoadOrderListingParser(
                    new HasEnabledMarkersProvider(
                        gameReleaseInjection))));
        var category = new GameCategoryContext(gameReleaseInjection);
        var pluginListingsPathProvider = new PluginListingsPathContext(
            new PluginListingsPathProvider(),
            gameReleaseInjection);
        var creationClubListingsPathProvider = new CreationClubListingsPathProvider(
            category,
            new CreationClubEnabledProvider(
                category),
            new GameDirectoryInjection(gameFolder));
        var loListingsProvider = new LoadOrderListingsProvider(
            new OrderListings(),
            new ImplicitListingsProvider(
                IFileSystemExt.DefaultFilesystem,
                dataDirectory,
                new ImplicitListingModKeyProvider(
                    gameReleaseInjection)),
            new PluginListingsProvider(
                gameReleaseInjection,
                new TimestampedPluginListingsProvider(
                    IFileSystemExt.DefaultFilesystem,
                    new TimestampAligner(IFileSystemExt.DefaultFilesystem),
                    new TimestampedPluginListingsPreferences() { ThrowOnMissingMods = false },
                    pluginRawListingsReader,
                    dataDirectory,
                    pluginListingsPathProvider),
                new EnabledPluginListingsProvider(
                    IFileSystemExt.DefaultFilesystem,
                    pluginRawListingsReader,
                    pluginListingsPathProvider)),
            new CreationClubListingsProvider(
                IFileSystemExt.DefaultFilesystem,
                dataDirectory,
                creationClubListingsPathProvider,
                new CreationClubRawListingsReader()));
        var archiveExt = new ArchiveExtensionProvider(gameReleaseInjection);
        var assetProvider = new GameAssetProvider(
            new DataDirectoryAssetProvider(
                fs,
                dataDirectory),
            new ArchiveAssetProvider(
                fs,
                new GetApplicableArchivePaths(
                    fs,
                    new CheckArchiveApplicability(
                        archiveExt),
                    dataDirectory,
                    archiveExt,
                    new CachedArchiveListingDetailsProvider(
                        loListingsProvider,
                        new GetArchiveIniListings(
                            fs,
                            new IniPathProvider(
                                gameReleaseInjection,
                                new IniPathLookup(
                                    gameDirLookup))))),
                gameReleaseInjection));
        return new GameEnvironmentProvider<TMod>(
                gameReleaseInjection,
                new LoadOrderImporter<TMod>(
                    IFileSystemExt.DefaultFilesystem,
                    dataDirectory,
                    loListingsProvider,
                    new ModImporter<TMod>(
                        IFileSystemExt.DefaultFilesystem,
                        gameReleaseInjection),
                    new MasterFlagsLookupProvider(
                        gameReleaseInjection,
                        IFileSystemExt.DefaultFilesystem,
                        dataDirectory)),
                dataDirectory,
                pluginListingsPathProvider,
                creationClubListingsPathProvider,
                assetProvider)
            .Construct();
    }

    DirectoryPath IDataDirectoryProvider.Path => DataFolderPath;

    FilePath IPluginListingsPathContext.Path => LoadOrderFilePath;

    FilePath? ICreationClubListingsPathProvider.Path => CreationClubListingsFilePath;

    ILoadOrderGetter<IModListingGetter<IModGetter>> IGameEnvironment.LoadOrder => LoadOrder;
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
        var fs = IFileSystemExt.DefaultFilesystem;
        var dataDirectory = new DataDirectoryInjection(Path.Combine(gameFolder, "Data"));
        var gameReleaseInjection = new GameReleaseInjection(release);
        var gameDir = new GameDirectoryInjection(gameFolder);
        var gameDirLookup = new GameDirectoryLookupInjection(release, gameDir.Path);
        var pluginRawListingsReader = new PluginRawListingsReader(
            fs,
            new PluginListingsParser(
                new PluginListingCommentTrimmer(),
                new LoadOrderListingParser(
                    new HasEnabledMarkersProvider(
                        gameReleaseInjection))));
        var category = new GameCategoryContext(gameReleaseInjection);
        var pluginListingsPathProvider = new PluginListingsPathContext(
            new PluginListingsPathProvider(),
            gameReleaseInjection);
        var creationClubListingsPathProvider = new CreationClubListingsPathProvider(
            category,
            new CreationClubEnabledProvider(
                category),
            new GameDirectoryInjection(gameFolder));
        var loListingsProvider = new LoadOrderListingsProvider(
            new OrderListings(),
            new ImplicitListingsProvider(
                IFileSystemExt.DefaultFilesystem,
                dataDirectory,
                new ImplicitListingModKeyProvider(
                    gameReleaseInjection)),
            new PluginListingsProvider(
                gameReleaseInjection,
                new TimestampedPluginListingsProvider(
                    IFileSystemExt.DefaultFilesystem,
                    new TimestampAligner(IFileSystemExt.DefaultFilesystem),
                    new TimestampedPluginListingsPreferences() { ThrowOnMissingMods = false },
                    pluginRawListingsReader,
                    dataDirectory,
                    pluginListingsPathProvider),
                new EnabledPluginListingsProvider(
                    IFileSystemExt.DefaultFilesystem,
                    pluginRawListingsReader,
                    pluginListingsPathProvider)),
            new CreationClubListingsProvider(
                IFileSystemExt.DefaultFilesystem,
                dataDirectory,
                creationClubListingsPathProvider,
                new CreationClubRawListingsReader()));
        var archiveExt = new ArchiveExtensionProvider(gameReleaseInjection);
        var assetProvider = new GameAssetProvider(
            new DataDirectoryAssetProvider(
                fs,
                dataDirectory),
            new ArchiveAssetProvider(
                fs,
                new GetApplicableArchivePaths(
                    fs,
                    new CheckArchiveApplicability(
                        archiveExt),
                    dataDirectory,
                    archiveExt,
                    new CachedArchiveListingDetailsProvider(
                        loListingsProvider,
                        new GetArchiveIniListings(
                            fs,
                            new IniPathProvider(
                                gameReleaseInjection,
                                new IniPathLookup(
                                    gameDirLookup))))),
                gameReleaseInjection));
        return new GameEnvironmentProvider<TModSetter, TModGetter>(
                gameReleaseInjection,
                new LoadOrderImporter<TModGetter>(
                    IFileSystemExt.DefaultFilesystem,
                    dataDirectory,
                    loListingsProvider,
                    new ModImporter<TModGetter>(
                        IFileSystemExt.DefaultFilesystem,
                        gameReleaseInjection),
                    new MasterFlagsLookupProvider(
                        gameReleaseInjection,
                        IFileSystemExt.DefaultFilesystem,
                        dataDirectory)),
                dataDirectory,
                pluginListingsPathProvider,
                creationClubListingsPathProvider,
                assetProvider)
            .Construct();
    }

    DirectoryPath IDataDirectoryProvider.Path => DataFolderPath;

    FilePath IPluginListingsPathContext.Path => LoadOrderFilePath;

    FilePath? ICreationClubListingsPathProvider.Path => CreationClubListingsFilePath;

    ILinkCache IGameEnvironment.LinkCache => LinkCache;

    ILoadOrderGetter<IModListingGetter<IModGetter>> IGameEnvironment.LoadOrder => LoadOrder;
}