using Loqui;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// A static class with LoadOrder related utility functions
    /// </summary>
    public static class LoadOrder
    {
        /// <summary>
        /// Attempts to locate the path to a game's load order file
        /// </summary>
        /// <param name="game">Game to locate for</param>
        /// <param name="path">Path to load order file if it was located</param>
        /// <returns>True if file located</returns>
        public static bool TryGetPluginsFile(GameRelease game, out FilePath path)
        {
            string pluginPath = game switch
            {
                GameRelease.Oblivion => "Oblivion/Plugins.txt",
                GameRelease.SkyrimLE => "Skyrim/Plugins.txt",
                GameRelease.SkyrimSE => "Skyrim Special Edition/Plugins.txt",
                _ => throw new NotImplementedException()
            };
            path = new FilePath(
                Path.Combine(
                    Environment.GetEnvironmentVariable("LocalAppData"),
                    pluginPath));
            return path.Exists;
        }

        /// <summary>
        /// Constructs a load order from a list of mods and a data folder.
        /// Load Order is sorted to the order the game will load the mod files: by file's date modified timestamp.
        /// </summary>
        /// <param name="modsToInclude">Mods to include</param>
        /// <param name="dataPath">Path to data folder</param>
        /// <param name="throwOnMissingMods">Whether to throw and exception if mods are missing</param>
        /// <returns>List of modkeys in load order, excluding missing mods</returns>
        /// <exception cref="FileNotFoundException">If throwOnMissingMods true and file is missing</exception>
        public static IExtendedList<ModKey> Align(
            IEnumerable<ModKey> modsToInclude,
            DirectoryPath dataPath,
            bool throwOnMissingMods = false)
        {
            List<(ModKey ModKey, DateTime Write)> list = new List<(ModKey ModKey, DateTime Write)>();
            var loadOrder = new ExtendedList<ModKey>();
            foreach (var key in modsToInclude)
            {
                FilePath file = new FilePath(
                    Path.Combine(dataPath.Path, key.ToString()));
                if (!file.Exists)
                {
                    if (throwOnMissingMods) throw new FileNotFoundException($"Expected mod was missing: {file}");
                    continue;
                }
                list.Add((key, file.Info.LastWriteTime));
            }
            loadOrder.AddRange(list
                .OrderBy(i => i.Write)
                .Select(i => i.ModKey));
            return loadOrder;
        }

        /// <summary>
        /// Parses a stream to retrieve all ModKeys in expected plugin file format
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <returns>List of modkeys representing a load order</returns>
        /// <exception cref="ArgumentException">Line in plugin stream is unexpected</exception>
        public static IExtendedList<ModKey> FromStream(Stream stream)
        {
            var ret = new ExtendedList<ModKey>();
            using var streamReader = new StreamReader(stream);
            while (!streamReader.EndOfStream)
            {
                var str = streamReader.ReadLine();
                var commentIndex = str.IndexOf('#');
                if (commentIndex != -1)
                {
                    str = str.Substring(0, commentIndex);
                }
                if (string.IsNullOrWhiteSpace(str)) continue;
                str = str.Trim();
                if (!ModKey.TryFactory(str, out var key))
                {
                    throw new ArgumentException($"Load order file had malformed line: {str}");   
                }
                ret.Add(key);
            }
            return ret;
        }
        
        /// <summary>
        /// Parses a file to retrieve all ModKeys in expected plugin file format
        /// </summary>
        /// <param name="path">Path of plugin list</param>
        /// <returns>List of modkeys representing a load order</returns>
        /// <exception cref="ArgumentException">Line in plugin file is unexpected</exception>
        public static IExtendedList<ModKey> FromPath(FilePath path)
        {
            var stream = new FileStream(path.Path, FileMode.Open, FileAccess.Read);
            return FromStream(stream);
        }

        /// <summary>
        /// Returns a load order listing from the usual sources
        /// </summary>
        /// <param name="game">Game type</param>
        /// <param name="dataPath">Path to game's data folder</param>
        /// <param name="allowMissingMods">Whether to skip missing mods</param>
        /// <returns>List of modkeys representing a load order</returns>
        /// <exception cref="ArgumentException">Line in plugin file is unexpected</exception>
        /// <exception cref="FileNotFoundException">If plugin file not located, or if allowMissingMods false and file is missing</exception>
        public static IExtendedList<ModKey> GetUsualLoadOrder(GameRelease game, DirectoryPath dataPath, bool allowMissingMods)
        {
            if (!TryGetPluginsFile(game, out var path))
            {
                throw new FileNotFoundException("Could not locate plugins file");
            }
            
            IExtendedList<ModKey> mods = FromPath(path);
            return Align(mods, dataPath, throwOnMissingMods: !allowMissingMods);
        }

        /// <summary>
        /// Constructs a load order filled with mods constructed by given importer
        /// </summary>
        /// <param name="dataFolder">Path data folder containing mods</param>
        /// <param name="loadOrder">Unique list of mod keys to import</param>
        public static LoadOrder<ModListing<TMod>> Import<TMod>(
            DirectoryPath dataFolder,
            IReadOnlyList<ModKey> loadOrder)
            where TMod : class, IModGetter
        {
            var results = new (ModKey ModKey, int ModIndex, TryGet<TMod> Mod)[loadOrder.Count];
            Parallel.ForEach(loadOrder, (modKey, state, modIndex) =>
            {
                var modPath = new ModPath(modKey, dataFolder.GetFile(modKey.FileName).Path);
                if (!File.Exists(modPath.Path))
                {
                    results[modIndex] = (modKey, (int)modIndex, TryGet<TMod>.Failure);
                    return;
                }
                var mod = ModInstantiator<TMod>.Importer(modPath);
                results[modIndex] = (modKey, (int)modIndex, TryGet<TMod>.Succeed(mod));
            });
            return new LoadOrder<ModListing<TMod>>(results
                .OrderBy(i => i.ModIndex)
                .Select(item =>
                {
                    if (item.Mod.Succeeded)
                    {
                        return new ModListing<TMod>(item.Mod.Value);
                    }
                    else
                    {
                        return ModListing<TMod>.UnloadedModListing(item.ModKey);
                    }
                }));
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
