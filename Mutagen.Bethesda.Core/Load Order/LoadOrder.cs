using DynamicData;
using Loqui;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// A static class with LoadOrder related utility functions
    /// </summary>
    public static class LoadOrder
    {
        private static string GetRelativePluginsPath(GameRelease game)
        {
            return game switch
            {
                GameRelease.Oblivion => "Oblivion/Plugins.txt",
                GameRelease.SkyrimLE => "Skyrim/Plugins.txt",
                GameRelease.SkyrimSE => "Skyrim Special Edition/Plugins.txt",
                GameRelease.SkyrimVR => "Skyrim VR/Plugins.txt",
                _ => throw new NotImplementedException()
            };
        }

        public static string GetPluginsPath(GameRelease game)
        {
            string pluginPath = GetRelativePluginsPath(game);
            return Path.Combine(
                Environment.GetEnvironmentVariable("LocalAppData"),
                pluginPath);
        }

        /// <summary>
        /// Attempts to locate the path to a game's load order file
        /// </summary>
        /// <param name="game">Game to locate for</param>
        /// <param name="path">Path to load order file if it was located</param>
        /// <returns>True if file located</returns>
        public static bool TryGetPluginsFile(GameRelease game, out FilePath path)
        {
            path = new FilePath(GetPluginsPath(game));
            return path.Exists;
        }

        /// <summary>
        /// Returns whether given game needs timestamp alignment for its load order
        /// </summary>
        /// <param name="game">Game to check</param>
        /// <returns>True if file located</returns>
        public static bool NeedsTimestampAlignment(GameCategory game)
        {
            switch (game)
            {
                case GameCategory.Oblivion:
                    return true;
                case GameCategory.Skyrim:
                    return false;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Constructs a load order from a list of mods and a data folder.
        /// Load Order is sorted to the order the game will load the mod files: by file's date modified timestamp.
        /// </summary>
        /// <param name="incomingLoadOrder">Mods to include</param>
        /// <param name="dataPath">Path to data folder</param>
        /// <param name="throwOnMissingMods">Whether to throw and exception if mods are missing</param>
        /// <returns>Enumerable of modkeys in load order, excluding missing mods</returns>
        /// <exception cref="MissingModException">If throwOnMissingMods true and file is missing</exception>
        public static IEnumerable<LoadOrderListing> AlignToTimestamps(
            IEnumerable<LoadOrderListing> incomingLoadOrder,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true)
        {
            var list = new List<(bool Enabled, ModKey ModKey, DateTime Write)>();
            foreach (var key in incomingLoadOrder)
            {
                ModPath modPath = new ModPath(key.ModKey, Path.Combine(dataPath.Path, key.ModKey.FileName));
                if (!File.Exists(modPath.Path))
                {
                    if (throwOnMissingMods) throw new MissingModException(modPath);
                    continue;
                }
                list.Add((key.Enabled, key.ModKey, File.GetLastWriteTime(modPath.Path)));
            }
            var comp = new LoadOrderTimestampComparer(incomingLoadOrder.Select(i => i.ModKey).ToList());
            return list
                .OrderBy(i => (i.ModKey, i.Write), comp)
                .Select(i => new LoadOrderListing(i.ModKey, i.Enabled));
        }

        /// <summary>
        /// Constructs a load order from a list of mods and a data folder.
        /// Load Order is sorted to the order the game will load the mod files: by file's date modified timestamp.
        /// </summary>
        /// <param name="incomingLoadOrder">Mods and their write timestamps</param>
        /// <returns>Enumerable of modkeys in load order, excluding missing mods</returns>
        /// <exception cref="MissingModException">If throwOnMissingMods true and file is missing</exception>
        public static IEnumerable<ModKey> AlignToTimestamps(IEnumerable<(ModKey ModKey, DateTime Write)> incomingLoadOrder)
        {
            return incomingLoadOrder
                .OrderBy(i => i, new LoadOrderTimestampComparer(incomingLoadOrder.Select(i => i.ModKey).ToList()))
                .Select(i => i.ModKey);
        }

        /// <summary>
        /// Modifies time stamps of files to match the given ordering
        /// <param name="loadOrder">Order to conform files to</param>
        /// <param name="dataPath">Path to data folder</param>
        /// <param name="throwOnMissingMods">Whether to throw and exception if mods are missing</param>
        /// <param name="startDate">Date to give the first file</param>
        /// <param name="interval">Time interval to space between each file's date</param>
        /// <exception cref="MissingModException">If throwOnMissingMods true and file is missing</exception>
        /// </summary>
        public static void AlignTimestamps(
            IEnumerable<ModKey> loadOrder,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true,
            DateTime? startDate = null,
            TimeSpan? interval = null)
        {
            startDate ??= DateTime.Today.AddDays(-1);
            interval ??= TimeSpan.FromMinutes(1);
            foreach (var mod in loadOrder)
            {
                ModPath modPath = new ModPath(mod, Path.Combine(dataPath.Path, mod.FileName));
                if (!File.Exists(modPath.Path))
                {
                    if (throwOnMissingMods) throw new MissingModException(modPath);
                    continue;
                }
                File.SetLastWriteTime(modPath.Path, startDate.Value);
                startDate = startDate.Value.Add(interval.Value);
            }
        }

        internal static LoadOrderListing FromString(ReadOnlySpan<char> str, bool enabledMarkerProcessing)
        {
            str = str.Trim();
            bool enabled = true;
            if (enabledMarkerProcessing)
            {
                if (str[0] == '*')
                {
                    str = str[1..];
                }
                else
                {
                    enabled = false;
                }
            }
            if (!ModKey.TryFactory(str, out var key))
            {
                throw new ArgumentException($"Load order file had malformed line: {str.ToString()}");
            }
            return new LoadOrderListing(key, enabled);
        }

        /// <summary>
        /// Parses a stream to retrieve all ModKeys in expected plugin file format
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <param name="game">Game type</param>
        /// <returns>List of modkeys representing a load order</returns>
        /// <exception cref="ArgumentException">Line in plugin stream is unexpected</exception>
        public static IEnumerable<LoadOrderListing> FromStream(Stream stream, GameRelease game)
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
                yield return FromString(str, enabledMarkerProcessing);
            }
        }

        /// <summary>
        /// Parses a file to retrieve all ModKeys in expected plugin file format,
        /// Will order mods by timestamps if applicable
        /// Will add implicit base mods if applicable
        /// </summary>
        /// <param name="game">Game type</param>
        /// <param name="path">Path of plugin list</param>
        /// <param name="dataPath">Path to game's data folder</param>
        /// <param name="throwOnMissingMods">Whether to throw and exception if mods are missing</param>
        /// <returns>Enumerable of modkeys representing a load order</returns>
        /// <exception cref="ArgumentException">Line in plugin file is unexpected</exception>
        public static IEnumerable<LoadOrderListing> FromPath(
            FilePath path,
            GameRelease game,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true)
        {
            List<LoadOrderListing> mods;
            if (path.Exists)
            {
                using var stream = new FileStream(path.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
                mods = FromStream(stream, game).ToList();
            }
            else
            {
                mods = new List<LoadOrderListing>();
            }
            AddImplicitMods(game, dataPath, mods);
            if (mods.Count == 0)
            {
                throw new FileNotFoundException("Could not locate plugins file");
            }
            if (NeedsTimestampAlignment(game.ToCategory()))
            {
                return AlignToTimestamps(mods, dataPath, throwOnMissingMods: throwOnMissingMods);
            }
            else
            {
                return mods;
            }
        }

        public static IObservable<IChangeSet<LoadOrderListing>> GetLiveLoadOrder(
            GameRelease game,
            FilePath loadOrderFilePath,
            DirectoryPath dataFolderPath,
            out IObservable<ErrorResponse> state,
            bool throwOnMissingMods = true)
        {
            var results = ObservableExt.WatchFile(loadOrderFilePath.Path)
                .StartWith(Unit.Default)
                .Select(_ =>
                {
                    try
                    {
                        return GetResponse<IObservable<IChangeSet<LoadOrderListing>>>.Succeed(
                            FromPath(loadOrderFilePath, game, dataFolderPath, throwOnMissingMods: throwOnMissingMods).AsObservableChangeSet());
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

        private readonly static ModKey[] _sseImplicitMods = new ModKey[]
        {
            "Skyrim.esm",
            "Update.esm",
            "Dawnguard.esm",
            "HearthFires.esm",
            "Dragonborn.esm",
        };

        internal static IEnumerable<ModKey> GetImplicitMods(GameRelease release)
        {
            return release switch
            {
                GameRelease.SkyrimSE => _sseImplicitMods,
                GameRelease.SkyrimVR => _sseImplicitMods,
                _ => Enumerable.Empty<ModKey>(),
            };
        }

        internal static void AddImplicitMods(
            GameRelease release,
            DirectoryPath dataPath,
            IList<LoadOrderListing> loadOrder)
        {
            foreach (var implicitMod in GetImplicitMods(release).Reverse())
            {
                if (loadOrder.Any(x => x.ModKey == implicitMod)) continue;
                if (!File.Exists(Path.Combine(dataPath.Path, implicitMod.FileName))) continue;
                loadOrder.Insert(0, new LoadOrderListing(implicitMod, true));
            }
        }

        public static bool HasEnabledMarkers(GameRelease game)
        {
            return game switch
            {
                GameRelease.SkyrimSE => true,
                GameRelease.SkyrimVR => true,
                GameRelease.SkyrimLE => false,
                GameRelease.Oblivion => false,
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// Returns a load order listing from the usual sources
        /// </summary>
        /// <param name="game">Game type</param>
        /// <param name="dataPath">Path to game's data folder</param>
        /// <param name="throwOnMissingMods">Whether to throw and exception if mods are missing</param>
        /// <returns>Enumerable of modkeys representing a load order</returns>
        /// <exception cref="ArgumentException">Line in plugin file is unexpected</exception>
        /// <exception cref="FileNotFoundException">If plugin file not located</exception>
        /// <exception cref="MissingModException">If throwOnMissingMods true and file is missing</exception>
        public static IEnumerable<LoadOrderListing> GetLoadOrder(
            GameRelease game,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true)
        {
            if (!TryGetPluginsFile(game, out var path))
            {
                throw new FileNotFoundException("Could not locate plugins file");
            }

            return FromPath(path, game, dataPath, throwOnMissingMods);
        }

        public static void Write(string path, GameRelease release, IEnumerable<LoadOrderListing> loadOrder)
        {
            bool markers = HasEnabledMarkers(release);
            var loadOrderList = loadOrder.ToList();
            foreach (var implicitMod in GetImplicitMods(release))
            {
                if (loadOrderList.Count > 0
                    && loadOrderList[0].ModKey == implicitMod
                    && loadOrderList[0].Enabled)
                {
                    loadOrderList.RemoveAt(0);
                }
            }
            File.WriteAllLines(path,
                loadOrderList.Where(x =>
                {
                    return (markers || x.Enabled);
                })
                .Select(x =>
                {
                    if (x.Enabled && markers)
                    {
                        return $"*{x.ModKey.FileName}";
                    }
                    else
                    {
                        return x.ModKey.FileName;
                    }
                }));
        }

        /// <summary>
        /// Constructs a load order filled with mods constructed
        /// </summary>
        /// <param name="dataFolder">Path data folder containing mods</param>
        /// <param name="loadOrder">Unique list of listings to import</param>
        /// <param name="gameRelease">GameRelease associated with the mods to create<br/>
        /// This may be unapplicable to some games with only one release, but should still be passed in.
        /// </param>
        public static LoadOrder<IModListing<TMod>> Import<TMod>(
            DirectoryPath dataFolder,
            IEnumerable<LoadOrderListing> loadOrder,
            GameRelease gameRelease)
            where TMod : class, IModGetter
        {
            return Import(
                dataFolder,
                loadOrder,
                (modPath) => ModInstantiator<TMod>.Importer(modPath, gameRelease));
        }

        /// <summary>
        /// Constructs a load order filled with mods constructed
        /// </summary>
        /// <param name="dataFolder">Path data folder containing mods</param>
        /// <param name="loadOrder">Unique list of mod keys to import</param>
        /// <param name="gameRelease">GameRelease associated with the mods to create<br/>
        /// This may be unapplicable to some games with only one release, but should still be passed in.
        /// </param>
        public static LoadOrder<IModListing<TMod>> Import<TMod>(
            DirectoryPath dataFolder,
            IEnumerable<ModKey> loadOrder,
            GameRelease gameRelease)
            where TMod : class, IModGetter
        {
            return Import(
                dataFolder,
                loadOrder.Select(m => new LoadOrderListing(m, enabled: true)),
                (modPath) => ModInstantiator<TMod>.Importer(modPath, gameRelease));
        }

        /// <summary>
        /// Constructs a load order filled with mods constructed by given importer func
        /// </summary>
        /// <param name="dataFolder">Path data folder containing mods</param>
        /// <param name="loadOrder">Unique list of mod keys to import</param>
        /// <param name="factory">Func to use to create a new mod from a path</param>
        public static LoadOrder<IModListing<TMod>> Import<TMod>(
            DirectoryPath dataFolder,
            IEnumerable<ModKey> loadOrder,
            Func<ModPath, TMod> factory)
            where TMod : class, IModGetter
        {
            return Import(
                dataFolder,
                loadOrder.Select(m => new LoadOrderListing(m, enabled: true)),
                factory);
        }

        /// <summary>
        /// Constructs a load order filled with mods constructed by given importer func
        /// </summary>
        /// <param name="dataFolder">Path data folder containing mods</param>
        /// <param name="loadOrder">Unique list of listings to import</param>
        /// <param name="factory">Func to use to create a new mod from a path</param>
        public static LoadOrder<IModListing<TMod>> Import<TMod>(
            DirectoryPath dataFolder,
            IEnumerable<LoadOrderListing> loadOrder,
            Func<ModPath, TMod> factory)
            where TMod : class, IModGetter
        {
            var loList = loadOrder.ToList();
            var results = new (ModKey ModKey, int ModIndex, TryGet<TMod> Mod, bool Enabled)[loList.Count];
            try
            {
                Parallel.ForEach(loList, (listing, state, modIndex) =>
                {
                    var modPath = new ModPath(listing.ModKey, dataFolder.GetFile(listing.ModKey.FileName).Path);
                    if (!File.Exists(modPath.Path))
                    {
                        results[modIndex] = (listing.ModKey, (int)modIndex, TryGet<TMod>.Failure, listing.Enabled);
                        return;
                    }
                    var mod = factory(modPath);
                    results[modIndex] = (listing.ModKey, (int)modIndex, TryGet<TMod>.Succeed(mod), listing.Enabled);
                });
                return new LoadOrder<IModListing<TMod>>(results
                    .OrderBy(i => i.ModIndex)
                    .Select(item =>
                    {
                        if (item.Mod.Succeeded)
                        {
                            return new ModListing<TMod>(item.Mod.Value, item.Enabled);
                        }
                        else
                        {
                            return ModListing<TMod>.UnloadedModListing(item.ModKey, item.Enabled);
                        }
                    }));
            }
            catch (Exception)
            {
                // We're aborting, but we still want to dispose any that were successful
                foreach (var result in results)
                {
                    if (result.Mod.Value is IDisposable disp)
                    {
                        disp.Dispose();
                    }
                }
                throw;
            }
        }
    }

    /// <summary>
    /// A container for objects with in a specific load order, that are associated with ModKeys.
    /// LoadOrder does not need to be disposed for proper use, but rather can optionally be disposed of which will dispose any contained items that implement IDisposable
    /// </summary>
    public class LoadOrder<TItem> : IReadOnlyList<KeyValuePair<ModKey, TItem>>, IReadOnlyDictionary<ModKey, TItem>, IDisposable
        where TItem : IModKeyed
    {
        private readonly List<ItemContainer> _byLoadOrder = new List<ItemContainer>();
        private readonly Dictionary<ModKey, ItemContainer> _byModKey = new Dictionary<ModKey, ItemContainer>();

        /// <inheritdoc />
        public int Count => _byLoadOrder.Count;

        /// <inheritdoc />
        public TItem this[int index] => _byLoadOrder[index].Item;

        /// <inheritdoc />
        public IEnumerable<ModKey> Keys => _byModKey.Keys;

        IEnumerable<TItem> IReadOnlyDictionary<ModKey, TItem>.Values => _byLoadOrder.Select(i => i.Item);

        public IEnumerable<TItem> ListedOrder => _byLoadOrder.Select(i => i.Item);

        public IEnumerable<TItem> PriorityOrder => ((IEnumerable<ItemContainer>)_byLoadOrder).Reverse().Select(i => i.Item);

        KeyValuePair<ModKey, TItem> IReadOnlyList<KeyValuePair<ModKey, TItem>>.this[int index]
        {
            get
            {
                var cont = _byLoadOrder[index];
                return new KeyValuePair<ModKey, TItem>(cont.Item.ModKey, cont.Item);
            }
        }

        /// <inheritdoc />
        public TItem this[ModKey key] => _byModKey[key].Item;

        public LoadOrder()
        {
        }

        public LoadOrder(IEnumerable<TItem> items)
        {
            int index = 0;
            _byLoadOrder.AddRange(items.Select(i => new ItemContainer(i, index++)));
            foreach (var item in _byLoadOrder)
            {
                try
                {
                    _byModKey.Add(item.Item.ModKey, item);
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException($"ModKey was already present: {item.Item.ModKey}");
                }
            }
        }

        /// <summary>
        /// Attempts to retrive an item given a ModKey
        /// </summary>
        /// <param name="key">ModKey to query for</param>
        /// <param name="value">Result containing located index, and a reference to the item</param>
        /// <returns>True if matching key located</returns>
        public bool TryGetValue(ModKey key, [MaybeNullWhen(false)] out (int Index, TItem Item) value)
        {
            if (_byModKey.TryGetValue(key, out var container))
            {
                value = (container.Index, container.Item);
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// Attempts to retrive an item given a ModKey
        /// </summary>
        /// <param name="key">ModKey to query for</param>
        /// <param name="value">Result reference to the item</param>
        /// <returns>True if matching key located</returns>
        public bool TryGetValue(ModKey key, [MaybeNullWhen(false)] out TItem value)
        {
            if (_byModKey.TryGetValue(key, out var container))
            {
                value = container.Item;
                return true;
            }
            value = default;
            return false;
        }

        bool IReadOnlyDictionary<ModKey, TItem>.TryGetValue(ModKey key, out TItem value)
        {
            return this.TryGetValue(key, out value!);
        }

        /// <summary>
        /// Attempts to retrive an item given an index
        /// </summary>
        /// <param name="index">Index to retrieve</param>
        /// <param name="result">Reference to the item</param>
        /// <returns>True if index in range</returns>
        public bool TryGetIndex(int index, [MaybeNullWhen(false)] out TItem result)
        {
            if (!_byLoadOrder.InRange(index))
            {
                result = default!;
                return false;
            }
            result = _byLoadOrder[index].Item;
            return true;
        }

        /// <summary>
        /// Adds an item to the end of the load order
        /// </summary>
        /// <param name="item">Item to put at end of load order</param>
        /// <exception cref="ArgumentException">If an item with same ModKey exists already</exception>
        public void Add(TItem item)
        {
            var index = _byLoadOrder.Count;
            var container = new ItemContainer(item, index);
            try
            {
                _byModKey.Add(item.ModKey, container);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException($"ModKey was already present: {item.ModKey}");
            }
            _byLoadOrder.Add(container);
        }

        /// <summary>
        /// Adds an item at the given index in load order, with the given ModKey
        /// </summary>
        /// <param name="item">Item to put at end of load order</param>
        /// <param name="index">Index to insert at</param>
        /// <exception cref="ArgumentException">If an item with same ModKey exists already</exception>
        public void Add(TItem item, int index)
        {
            if (!_byLoadOrder.InRange(index))
            {
                throw new ArgumentException("Tried to insert at an out of range index.");
            }
            var container = new ItemContainer(item, index);
            try
            {
                _byModKey.Add(item.ModKey, container);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException($"ModKey was already present: {item.ModKey}");
            }
            _byLoadOrder.Add(container);
            for (int i = index + 1; i < _byLoadOrder.Count; i++)
            {
                _byLoadOrder[i].Index += 1;
            }
        }

        /// <inheritdoc />
        public bool ContainsKey(ModKey key)
        {
            return IndexOf(key) != -1;
        }

        /// <summary>
        /// Locates index of an item with given key
        /// </summary>
        /// <param name="key">Key to query</param>
        /// <returns>Index of item on list with key. -1 if not located</returns>
        public int IndexOf(ModKey key)
        {
            if (!_byModKey.TryGetValue(key, out var container))
            {
                return -1;
            }
            return container.Index;
        }

        /// <summary>
        /// Clears load order of all items
        /// </summary>
        public void Clear()
        {
            this._byLoadOrder.Clear();
            this._byModKey.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Disposes all contained items that implement IDisposable
        /// </summary>
        public void Dispose()
        {
            foreach (var item in _byLoadOrder)
            {
                if (item.Item is IDisposable disp)
                {
                    disp.Dispose();
                }
            }
        }

        public IEnumerator<KeyValuePair<ModKey, TItem>> GetEnumerator()
        {
            foreach (var item in _byLoadOrder)
            {
                yield return new KeyValuePair<ModKey, TItem>(item.Item.ModKey, item.Item);
            }
        }

        private class ItemContainer
        {
            public readonly TItem Item;
            public int Index;

            public ItemContainer(TItem item, int index)
            {
                Item = item;
                Index = index;
            }
        }
    }
}
