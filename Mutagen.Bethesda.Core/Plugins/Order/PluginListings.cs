using DynamicData;
using Noggog;
using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Concurrency;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Installs;
using Mutagen.Bethesda.Installs.DI;
using Mutagen.Bethesda.Plugins.Order.DI;

namespace Mutagen.Bethesda.Plugins.Order;

public static class PluginListings
{
    /// <inheritdoc cref="IPluginListingsProvider"/>
    public static string GetListingsPath(GameRelease game)
    {
        var gameReleaseInjection = new GameReleaseInjection(game);
        var gameLocator = GameLocatorLookupCache.Instance;
        return new PluginListingsPathContext(
            new PluginListingsPathProvider(
                new DataDirectoryProvider(
                    gameReleaseInjection,
                    gameLocator)),
            gameReleaseInjection).Path;
    }

    /// <summary>
    /// Attempts to locate the path to a game's load order file, and ensure existence
    /// </summary>
    /// <param name="game">Release to query</param>
    /// <param name="path">Path to load order file if it was located</param>
    /// <returns>True if file located</returns>
    public static bool TryGetListingsFile(GameRelease game, out FilePath path)
    {
        path = GetListingsPath(game);
        return File.Exists(path);
    }

    /// <summary>
    /// Attempts to locate the path to a game's load order file, and ensure existence
    /// </summary>
    /// <param name="game">Release to query</param>
    /// <returns>Path to load order file if it was located</returns>
    /// <exception cref="FileNotFoundException">If expected plugin file did not exist</exception>
    public static FilePath GetListingsFile(GameRelease game)
    {
        if (TryGetListingsFile(game, out var path))
        {
            return path;
        }

        throw new FileNotFoundException(
            $"Could not locate load order automatically.  Expected a file at: {path.Path}");
    }

    /// <summary>
    /// Parses a stream to retrieve all ModKeys in expected plugin file format
    /// </summary>
    /// <param name="stream">Stream to read from</param>
    /// <param name="game">Game type</param>
    /// <returns>List of ModKeys representing a load order</returns>
    /// <exception cref="ArgumentException">Line in plugin stream is unexpected</exception>
    public static IEnumerable<ILoadOrderListingGetter> LoadOrderListingsFromStream(Stream stream, GameRelease game)
    {
        return new PluginListingsParser(
                new PluginListingCommentTrimmer(),
                new LoadOrderListingParser(
                    new HasEnabledMarkersProvider(
                        new GameReleaseInjection(game))))
            .Parse(stream);
    }

    /// <inheritdoc cref="IPluginListingsProvider"/>
    public static IEnumerable<ILoadOrderListingGetter> LoadOrderListings(
        GameRelease game,
        DirectoryPath dataPath,
        bool throwOnMissingMods = true)
    {
        var gameReleaseInjection = new GameReleaseInjection(game);
        var gameLocator = GameLocatorLookupCache.Instance;
        return LoadOrderListingsFromPath(
            new PluginListingsPathContext(
                new PluginListingsPathProvider(
                    new DataDirectoryProvider(
                        gameReleaseInjection,
                        gameLocator)),
                gameReleaseInjection).Path,
            game,
            dataPath,
            throwOnMissingMods);
    }

    /// <inheritdoc cref="IPluginListingsProvider"/>
    public static IEnumerable<ILoadOrderListingGetter> LoadOrderListingsFromPath(
        FilePath pluginTextPath,
        GameRelease game,
        DirectoryPath dataPath,
        bool throwOnMissingMods = true,
        IFileSystem? fileSystem = null)
    {
        return PluginListingsProvider(
            new DataDirectoryInjection(dataPath),
            new GameReleaseInjection(game),
            new PluginListingsPathInjection(pluginTextPath),
            throwOnMissingMods,
            fileSystem ?? IFileSystemExt.DefaultFilesystem).Get();
    }

    /// <inheritdoc cref="IPluginListingsProvider"/>
    public static IEnumerable<ILoadOrderListingGetter> RawLoadOrderListingsFromPath(
        FilePath pluginTextPath,
        GameRelease game)
    {
        using var fs = new FileStream(pluginTextPath.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return LoadOrderListingsFromStream(fs, game).ToList();
    }

    /// <inheritdoc cref="IPluginLiveLoadOrderProvider"/>
    public static IObservable<IChangeSet<ILoadOrderListingGetter>> GetLiveLoadOrder(
        GameRelease game,
        FilePath loadOrderFilePath,
        DirectoryPath dataFolderPath,
        out IObservable<ErrorResponse> state,
        bool throwOnMissingMods = true,
        IScheduler? scheduler = null,
        IFileSystem? fileSystem = null)
    {
        fileSystem ??= IFileSystemExt.DefaultFilesystem;
        var pluginPath = new PluginListingsPathInjection(loadOrderFilePath);
        var prov = PluginListingsProvider(
            new DataDirectoryInjection(dataFolderPath),
            new GameReleaseInjection(game),
            pluginPath,
            throwOnMissingMods,
            fileSystem);
        return new PluginLiveLoadOrderProvider(
            fileSystem,
            prov,
            pluginPath).Get(out state, scheduler);
    }

    /// <inheritdoc cref="IPluginLiveLoadOrderProvider"/>
    public static IObservable<Unit> GetLoadOrderChanged(FilePath loadOrderFilePath)
    {
        return ObservableExt.WatchFile(loadOrderFilePath.Path);
    }

    /// <inheritdoc cref="IPluginLiveLoadOrderProvider"/>
    public static IObservable<Unit> GetLoadOrderChanged(GameRelease game)
    {
        var gameReleaseInjection = new GameReleaseInjection(game);
        var gameLocator = GameLocatorLookupCache.Instance;
        return ObservableExt.WatchFile(
            new PluginListingsPathContext(
                new PluginListingsPathProvider(
                    new DataDirectoryProvider(
                        gameReleaseInjection,
                        gameLocator)),
                gameReleaseInjection).Path);
    }

    private static PluginListingsProvider PluginListingsProvider(
        IDataDirectoryProvider dataDirectory,
        IGameReleaseContext gameContext,
        IPluginListingsPathContext listingsPathContext, 
        bool throwOnMissingMods,
        IFileSystem fs)
    {
        var pluginListingParser = new PluginListingsParser(
            new PluginListingCommentTrimmer(),
            new LoadOrderListingParser(
                new HasEnabledMarkersProvider(gameContext)));
        var provider = new PluginListingsProvider(
            gameContext,
            new TimestampedPluginListingsProvider(
                fs,
                new TimestampAligner(fs),
                new TimestampedPluginListingsPreferences() {ThrowOnMissingMods = throwOnMissingMods},
                new PluginRawListingsReader(
                    fs,
                    pluginListingParser),
                dataDirectory,
                listingsPathContext),
            new EnabledPluginListingsProvider(
                fs,
                new PluginRawListingsReader(
                    fs,
                    pluginListingParser),
                listingsPathContext));
        return provider;
    }
}