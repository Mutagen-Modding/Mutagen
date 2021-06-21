using DynamicData;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;

namespace Mutagen.Bethesda.Plugins.Order
{
    public static class PluginListings
    {
        private static readonly PluginPathProvider PathProvider = new();
        private static readonly ModListingParserFactory ModListingParserFactory = new();
        private static readonly PluginListingsParserFactory ParserFactory = new(ModListingParserFactory);
        private static readonly PluginListingsProvider Retriever = new(
            IFileSystemExt.DefaultFilesystem,
            ParserFactory,
            PathProvider,
            new TimestampAligner(IFileSystemExt.DefaultFilesystem));
        private static readonly PluginLiveLoadOrderProvider LiveLoadOrder = new(
            IFileSystemExt.DefaultFilesystem,
            Retriever,
            PathProvider);
        
        /// <summary>
        /// Returns expected location of the plugin load order file
        /// </summary>
        /// <param name="game">Release to query</param>
        /// <returns>Expected path to load order file</returns>
        public static string GetListingsPath(GameRelease game)
        {
            return PathProvider.Get(game);
        }

        /// <summary>
        /// Attempts to locate the path to a game's load order file, and ensure existence
        /// </summary>
        /// <param name="game">Release to query</param>
        /// <param name="path">Path to load order file if it was located</param>
        /// <returns>True if file located</returns>
        public static bool TryGetListingsFile(GameRelease game, out FilePath path)
        {
            path = new FilePath(PathProvider.Get(game));
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
            throw new FileNotFoundException($"Could not locate load order automatically.  Expected a file at: {path.Path}");
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
            return ParserFactory.Create(game).Parse(stream);
        }

        /// <inheritdoc cref="IPluginListingsProvider"/>
        public static IEnumerable<IModListingGetter> ListingsFromPath(
            GameRelease game,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true)
        {
            return Retriever.ListingsFromPath(game, dataPath, throwOnMissingMods: throwOnMissingMods);
        }

        /// <inheritdoc cref="IPluginListingsProvider"/>
        public static IEnumerable<IModListingGetter> ListingsFromPath(
            FilePath pluginTextPath,
            GameRelease game,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true)
        {
            return Retriever.ListingsFromPath(pluginTextPath, game, dataPath, throwOnMissingMods: throwOnMissingMods);
        }

        /// <inheritdoc cref="IPluginListingsProvider"/>
        public static IEnumerable<IModListingGetter> RawListingsFromPath(
            FilePath pluginTextPath,
            GameRelease game)
        {
            return Retriever.RawListingsFromPath(pluginTextPath, game);
        }

        /// <inheritdoc cref="IPluginLiveLoadOrderProvider"/>
        public static IObservable<IChangeSet<IModListingGetter>> GetLiveLoadOrder(
            GameRelease game,
            FilePath loadOrderFilePath,
            DirectoryPath dataFolderPath,
            out IObservable<ErrorResponse> state,
            bool throwOnMissingMods = true,
            bool orderListings = true)
        {
            return LiveLoadOrder.GetLiveLoadOrder(
                game: game,
                loadOrderFilePath: loadOrderFilePath,
                dataFolderPath: dataFolderPath,
                state: out state,
                throwOnMissingMods: throwOnMissingMods,
                orderListings: orderListings);
        }

        /// <inheritdoc cref="IPluginLiveLoadOrderProvider"/>
        public static IObservable<Unit> GetLoadOrderChanged(FilePath loadOrderFilePath)
        {
            return LiveLoadOrder.GetLoadOrderChanged(loadOrderFilePath);
        }

        /// <inheritdoc cref="IPluginLiveLoadOrderProvider"/>
        public static IObservable<Unit> GetLoadOrderChanged(GameRelease game) =>
            LiveLoadOrder.GetLoadOrderChanged(game);

        public static bool HasEnabledMarkers(GameRelease game)
        {
            return game switch
            {
                GameRelease.Fallout4 => true,
                GameRelease.SkyrimSE => true,
                GameRelease.EnderalSE => true,
                GameRelease.SkyrimVR => true,
                GameRelease.SkyrimLE => false,
                GameRelease.EnderalLE => false,
                GameRelease.Oblivion => false,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
