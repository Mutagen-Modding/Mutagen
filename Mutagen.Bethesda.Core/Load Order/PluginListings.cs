using DynamicData;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Mutagen.Bethesda
{
    public static class PluginListings
    {
        private static string GetRelativePluginsPath(GameRelease game)
        {
            return game switch
            {
                GameRelease.Oblivion => "Oblivion/Plugins.txt",
                GameRelease.SkyrimLE => "Skyrim/Plugins.txt",
                GameRelease.SkyrimSE => "Skyrim Special Edition/Plugins.txt",
                GameRelease.SkyrimVR => "Skyrim VR/Plugins.txt",
                GameRelease.Fallout4 => "Fallout4/Plugins.txt",
                _ => throw new NotImplementedException()
            };
        }

        public static string GetListingsPath(GameRelease game)
        {
            string pluginPath = GetRelativePluginsPath(game);
            return Path.Combine(
                Environment.GetEnvironmentVariable("LocalAppData")!,
                pluginPath);
        }

        /// <summary>
        /// Attempts to locate the path to a game's load order file, and ensure existance
        /// </summary>
        /// <param name="game">Game to locate for</param>
        /// <param name="path">Path to load order file if it was located</param>
        /// <returns>True if file located</returns>
        public static bool TryGetListingsFile(GameRelease game, out FilePath path)
        {
            path = new FilePath(GetListingsPath(game));
            return path.Exists;
        }

        /// <summary>
        /// Attempts to locate the path to a game's load order file, and ensure existance
        /// </summary>
        /// <param name="game">Game to locate for</param>
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
        /// <returns>List of modkeys representing a load order</returns>
        /// <exception cref="ArgumentException">Line in plugin stream is unexpected</exception>
        public static IEnumerable<LoadOrderListing> ListingsFromStream(Stream stream, GameRelease game)
        {
            using var streamReader = new StreamReader(stream);
            var enabledMarkerProcessing = HasEnabledMarkers(game);
            while (!streamReader.EndOfStream)
            {
                var str = streamReader.ReadLine().AsSpan();
                var commentIndex = str.IndexOf('#');
                if (commentIndex != -1)
                {
                    str = str.Slice(0, commentIndex);
                }
                if (MemoryExtensions.IsWhiteSpace(str) || str.Length == 0) continue;
                yield return LoadOrderListing.FromString(str, enabledMarkerProcessing);
            }
        }

        /// <summary>
        /// Parses the typical plugins file to retrieve all ModKeys in expected plugin file format,
        /// Will order mods by timestamps if applicable
        /// Will add implicit base mods if applicable
        /// </summary>
        /// <param name="game">Game type</param>
        /// <param name="dataPath">Path to game's data folder</param>
        /// <param name="throwOnMissingMods">Whether to throw and exception if mods are missing</param>
        /// <returns>Enumerable of modkeys representing a load order</returns>
        /// <exception cref="ArgumentException">Line in plugin file is unexpected</exception>
        public static IEnumerable<LoadOrderListing> ListingsFromPath(
            GameRelease game,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true)
        {
            return ListingsFromPath(
                pluginTextPath: GetListingsPath(game),
                game: game,
                dataPath: dataPath,
                throwOnMissingMods: throwOnMissingMods);
        }

        /// <summary>
        /// Parses a file to retrieve all ModKeys in expected plugin file format,
        /// Will order mods by timestamps if applicable
        /// Will add implicit base mods if applicable
        /// </summary>
        /// <param name="game">Game type</param>
        /// <param name="pluginTextPath">Path of plugin list</param>
        /// <param name="dataPath">Path to game's data folder</param>
        /// <param name="throwOnMissingMods">Whether to throw and exception if mods are missing</param>
        /// <returns>Enumerable of modkeys representing a load order</returns>
        /// <exception cref="ArgumentException">Line in plugin file is unexpected</exception>
        public static IEnumerable<LoadOrderListing> ListingsFromPath(
            FilePath pluginTextPath,
            GameRelease game,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true)
        {
            var mods = RawListingsFromPath(pluginTextPath, game);
            if (LoadOrder.NeedsTimestampAlignment(game.ToCategory()))
            {
                return LoadOrder.AlignToTimestamps(mods, dataPath, throwOnMissingMods: throwOnMissingMods);
            }
            else
            {
                return mods;
            }
        }

        public static IEnumerable<LoadOrderListing> RawListingsFromPath(
            FilePath pluginTextPath,
            GameRelease game)
        {
            if (!pluginTextPath.Exists)
            {
                throw new FileNotFoundException("Could not locate plugins file");
            }
            using var stream = new FileStream(pluginTextPath.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return ListingsFromStream(stream, game).ToList();
        }

        public static IObservable<IChangeSet<LoadOrderListing>> GetLiveLoadOrder(
            GameRelease game,
            FilePath loadOrderFilePath,
            DirectoryPath dataFolderPath,
            out IObservable<ErrorResponse> state,
            bool throwOnMissingMods = true,
            bool orderListings = true)
        {
            var results = ObservableExt.WatchFile(loadOrderFilePath.Path)
                .StartWith(Unit.Default)
                .Select(_ =>
                {
                    try
                    {
                        var lo = ListingsFromPath(loadOrderFilePath, game, dataFolderPath, throwOnMissingMods: throwOnMissingMods);
                        if (orderListings)
                        {
                            lo = lo.OrderListings();
                        }
                        return GetResponse<IObservable<IChangeSet<LoadOrderListing>>>.Succeed(lo.AsObservableChangeSet());
                    }
                    catch (Exception ex)
                    {
                        return GetResponse<IObservable<IChangeSet<LoadOrderListing>>>.Fail(ex);
                    }
                })
                .Replay(1)
                .RefCount();
            state = results
                .Select(r => (ErrorResponse)r);
            return results
                .Select(r =>
                {
                    return r.Value ?? Observable.Empty<IChangeSet<LoadOrderListing>>();
                })
                .Switch();
        }

        public static IObservable<Unit> GetLoadOrderChanged(FilePath loadOrderFilePath)
        {
            return ObservableExt.WatchFile(loadOrderFilePath.Path);
        }

        public static IObservable<Unit> GetLoadOrderChanged(GameRelease game) => GetLoadOrderChanged(GetListingsPath(game));

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
