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
        private static readonly PluginPathProvider PathProvider = new(IFileSystemExt.DefaultFilesystem);
        private static readonly PluginListingsProvider Retriever = new(
            IFileSystemExt.DefaultFilesystem,
            PathProvider,
            new TimestampAligner(IFileSystemExt.DefaultFilesystem));
        private static readonly PluginLiveLoadOrderProvider LiveLoadOrder = new(
            IFileSystemExt.DefaultFilesystem,
            Retriever,
            PathProvider);
        
        /// <inheritdoc cref="IPluginListingsProvider"/>
        public static string GetListingsPath(GameRelease game)
        {
            return PathProvider.GetListingsPath(game);
        }

        /// <inheritdoc cref="IPluginListingsProvider"/>
        public static bool TryGetListingsFile(GameRelease game, out FilePath path)
        {
            return PathProvider.TryLocateListingsPath(game, out path);
        }

        /// <inheritdoc cref="IPluginListingsProvider"/>
        public static FilePath GetListingsFile(GameRelease game)
        {
            return PathProvider.LocateListingsPath(game);
        }

        /// <inheritdoc cref="IPluginListingsProvider"/>
        public static IEnumerable<IModListingGetter> ListingsFromStream(Stream stream, GameRelease game)
        {
            return Retriever.ListingsFromStream(stream, game);
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
