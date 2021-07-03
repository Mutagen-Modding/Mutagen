using DynamicData;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order.DI;

namespace Mutagen.Bethesda.Plugins.Order
{
    public static class PluginListings
    {
        /// <inheritdoc cref="IPluginListingsProvider"/>
        public static string GetListingsPath(GameRelease game)
        {
            return new PluginPathContext(new GameReleaseInjection(game)).Path;
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
        public static IEnumerable<IModListingGetter> ListingsFromStream(Stream stream, GameRelease game)
        {
            return new PluginListingsParser(
                    new ModListingParser(
                        new HasEnabledMarkersProvider(
                            new GameReleaseInjection(game))))
                .Parse(stream);
        }

        /// <inheritdoc cref="IPluginListingsProvider"/>
        public static IEnumerable<IModListingGetter> Listings(
            GameRelease game,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true)
        {
            return ListingsFromPath(
                new PluginPathContext(
                    new GameReleaseInjection(game)).Path,
                game,
                dataPath,
                throwOnMissingMods);
        }

        /// <inheritdoc cref="IPluginListingsProvider"/>
        public static IEnumerable<IModListingGetter> ListingsFromPath(
            FilePath pluginTextPath,
            GameRelease game,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true)
        {
            return PluginListingsProvider(
                new DataDirectoryInjection(dataPath),
                new GameReleaseInjection(game),
                new PluginPathInjection(pluginTextPath),
                throwOnMissingMods).Get();
        }

        /// <inheritdoc cref="IPluginListingsProvider"/>
        public static IEnumerable<IModListingGetter> RawListingsFromPath(
            FilePath pluginTextPath,
            GameRelease game)
        {
            using var fs = new FileStream(pluginTextPath.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return ListingsFromStream(fs, game).ToList();
        }

        /// <inheritdoc cref="IPluginLiveLoadOrderProvider"/>
        public static IObservable<IChangeSet<IModListingGetter>> GetLiveLoadOrder(
            GameRelease game,
            FilePath loadOrderFilePath,
            DirectoryPath dataFolderPath,
            out IObservable<ErrorResponse> state,
            bool throwOnMissingMods = true)
        {
            var pluginPath = new PluginPathInjection(loadOrderFilePath);
            var prov = PluginListingsProvider(
                new DataDirectoryInjection(dataFolderPath),
                new GameReleaseInjection(game),
                pluginPath,
                throwOnMissingMods);
            return new PluginLiveLoadOrderProvider(
                IFileSystemExt.DefaultFilesystem,
                prov,
                pluginPath).Get(out state);
        }

        /// <inheritdoc cref="IPluginLiveLoadOrderProvider"/>
        public static IObservable<Unit> GetLoadOrderChanged(FilePath loadOrderFilePath)
        {
            return ObservableExt.WatchFile(loadOrderFilePath.Path);
        }

        /// <inheritdoc cref="IPluginLiveLoadOrderProvider"/>
        public static IObservable<Unit> GetLoadOrderChanged(GameRelease game)
        {
            return ObservableExt.WatchFile(
                new PluginPathContext(
                    new GameReleaseInjection(game)).Path);
        }

        public static bool HasEnabledMarkers(GameRelease game)
        {
            return new HasEnabledMarkersProvider(
                new GameReleaseInjection(game)).HasEnabledMarkers;
        }

        private static PluginListingsProvider PluginListingsProvider(
            IDataDirectoryContext dataDirectory,
            IGameReleaseContext gameContext,
            IPluginPathContext pathContext, 
            bool throwOnMissingMods)
        {
            var fs = IFileSystemExt.DefaultFilesystem;
            var pluginListingParser = new PluginListingsParser(
                new ModListingParser(
                    new HasEnabledMarkersProvider(gameContext)));
            var provider = new PluginListingsProvider(
                gameContext,
                new TimestampedPluginListingsProvider(
                    new TimestampAligner(fs),
                    new TimestampedPluginListingsPreferences() {ThrowOnMissingMods = throwOnMissingMods},
                    new PluginRawListingsReader(
                        fs,
                        pluginListingParser),
                    dataDirectory,
                    pathContext),
                new EnabledPluginListingsProvider(
                    new PluginRawListingsReader(
                        fs,
                        pluginListingParser),
                    pathContext));
            return provider;
        }
    }
}