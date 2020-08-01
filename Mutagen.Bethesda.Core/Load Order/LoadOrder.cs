using Loqui;
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
        public static IExtendedList<ModKey> AlignLoadOrder(
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
        public static IExtendedList<ModKey> ProcessLoadOrder(Stream stream)
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
        public static IExtendedList<ModKey> ProcessLoadOrder(FilePath path)
        {
            var stream = new FileStream(path.Path, FileMode.Open, FileAccess.Read);
            return ProcessLoadOrder(stream);
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
            
            IExtendedList<ModKey> mods = ProcessLoadOrder(path);
            return AlignLoadOrder(mods, dataPath, throwOnMissingMods: !allowMissingMods);
        }
    }

    /// <summary>
    /// A container for Mod objects in an order that can optionally exist.
    /// For a load order of just ModKeys in an order with no optionality, it is usually preferable to just use a normal List of ModKeys.
    /// LoadOrder does not need to be disposed for proper use, but rather can optionally be disposed of which will dispose any contained mods that implement IDisposable
    /// </summary>
    public class LoadOrder<TMod> : IReadOnlyList<ModListing<TMod>>, IDisposable
        where TMod : class, IModGetter
    {
        private readonly List<ModListing<TMod>> _modsByLoadOrder = new List<ModListing<TMod>>();

        /// <summary>
        /// Number of mods
        /// </summary>
        public int Count => _modsByLoadOrder.Count;

        /// <summary>
        /// Access a mod at a given index
        /// </summary>
        public ModListing<TMod> this[int index] => _modsByLoadOrder[index];

        /// <summary>
        /// Attempts to retrive a mod listing given a ModKey
        /// </summary>
        /// <param name="key">ModKey to query for</param>
        /// <param name="result">Result containing located index, and a reference to the listing</param>
        /// <returns>True if matching mod located</returns>
        public bool TryGetListing(ModKey key, out (int Index, ModListing<TMod> Listing) result)
        {
            for (int i = 0; i < _modsByLoadOrder.Count; i++)
            {
                var item = _modsByLoadOrder[i];
                if (item.Key.Equals(key))
                {
                    result = (i, _modsByLoadOrder[i]);
                    return true;
                }
            }
            result = default(ValueTuple<int, ModListing<TMod>>);
            return false;
        }

        /// <summary>
        /// Attempts to retrive a mod object given a ModKey
        /// </summary>
        /// <param name="key">ModKey to query for</param>
        /// <param name="result">Result containing located index, and a reference to the mod object</param>
        /// <returns>True if matching mod located</returns>
        public bool TryGetMod(ModKey key, out (int Index, TMod Mod) result)
        {
            if (!this.TryGetListing(key, out var listing)
                || listing.Listing.Mod == null)
            {
                result = default((int, TMod));
                return false;
            }
            result = (listing.Index, listing.Listing.Mod);
            return true;
        }

        /// <summary>
        /// Attempts to retrive a mod listing given an index
        /// </summary>
        /// <param name="index">Index to retrieve</param>
        /// <param name="result">Reference to the mod listing</param>
        /// <returns>True if index in range</returns>
        public bool TryGetListing(int index, [MaybeNullWhen(false)] out ModListing<TMod> result)
        {
            if (!_modsByLoadOrder.InRange(index))
            {
                result = default!;
                return false;
            }
            result = _modsByLoadOrder[index];
            return result != null;
        }

        /// <summary>
        /// Adds a mod to the end of the load order
        /// </summary>
        /// <param name="mod">Mod to add</param>
        /// <exception cref="ArgumentException">If mod with same key exists already</exception>
        public void Add(TMod mod)
        {
            if (this.Contains(mod.ModKey))
            {
                throw new ArgumentException("Mod was already present on the mod list.");
            }
            _modsByLoadOrder.Add(new ModListing<TMod>(mod));
        }

        /// <summary>
        /// Adds a mod at the given index
        /// </summary>
        /// <param name="mod">Mod to add</param>
        /// <param name="index">Index to insert at</param>
        /// <exception cref="ArgumentException">If mod with same key exists already</exception>
        public void Add(TMod mod, int index)
        {
            if (this.Contains(mod.ModKey))
            {
                throw new ArgumentException("Mod was already present on the mod list.");
            }
            _modsByLoadOrder.Insert(index, new ModListing<TMod>(mod));
        }

        /// <summary>
        /// Checks if a mod exists with given key
        /// </summary>
        /// <param name="key">Key to query</param>
        /// <returns>True if mod on list with key</returns>
        public bool Contains(ModKey key)
        {
            return IndexOf(key) != -1;
        }

        /// <summary>
        /// Locates index of a mod with given key
        /// </summary>
        /// <param name="key">Key to query</param>
        /// <returns>Index of mod on list with key. -1 if not located</returns>
        public int IndexOf(ModKey key)
        {
            for (int i = 0; i < _modsByLoadOrder.Count; i++)
            {
                if (_modsByLoadOrder[i].Key.Equals(key))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Clears load order of all mods
        /// </summary>
        public void Clear()
        {
            this._modsByLoadOrder.Clear();
        }

        /// <summary>
        /// Delegate used for importing mods
        /// </summary>
        /// <param name="path">Path to mod file</param>
        /// <param name="modKey">ModKey associated with listing</param>
        /// <param name="mod">Out parameter containing mod object if successful</param>
        /// <returns>True if import successful</returns>
        public delegate bool Importer(ModPath path, [MaybeNullWhen(false)] out TMod mod);

        /// <summary>
        /// Clears load order and fills it with mods constructed by given importer
        /// </summary>
        /// <param name="dataFolder">Path data folder containing mods</param>
        /// <param name="loadOrder">Unique list of mod keys to import</param>
        /// <param name="importer">Function used to construct a mod</param>
        public void Import(
            DirectoryPath dataFolder,
            IReadOnlyList<ModKey> loadOrder,
            Importer importer)
        {
            this.Clear();
            var results = new (ModKey ModKey, int ModIndex, TryGet<TMod> Mod)[loadOrder.Count];
            Parallel.ForEach(loadOrder, (modKey, state, modIndex) =>
            {
                var modPath = new ModPath(modKey, dataFolder.GetFile(modKey.FileName).Path);
                if (!File.Exists(modPath.Path))
                {
                    results[modIndex] = (modKey, (int)modIndex, TryGet<TMod>.Failure);
                    return;
                }
                if (!importer(modPath, out var mod))
                {
                    results[modIndex] = (modKey, (int)modIndex, TryGet<TMod>.Failure);
                    return;
                }
                results[modIndex] = (modKey, (int)modIndex, TryGet<TMod>.Succeed(mod));
            });
            foreach (var item in results
                .OrderBy(i => i.ModIndex))
            {
                if (item.Mod.Succeeded)
                {
                    this._modsByLoadOrder.Add(
                        new ModListing<TMod>(
                            item.Mod.Value));
                }
                else
                {
                    this._modsByLoadOrder.Add(
                        ModListing<TMod>.UnloadedModListing(item.ModKey));
                }
            }
        }

        /// <summary>
        /// Creates a load order and fills it with mods constructed by given importer
        /// </summary>
        /// <param name="dataFolder">Path data folder containing mods</param>
        /// <param name="loadOrder">Unique list of mod keys to import</param>
        /// <param name="importer">Function used to construct a mod</param>
        public static LoadOrder<TMod> ImportFactory(
            DirectoryPath dataFolder,
            IReadOnlyList<ModKey> loadOrder,
            Importer importer)
        {
            var ret = new LoadOrder<TMod>();
            ret.Import(
                dataFolder,
                loadOrder,
                importer);
            return ret;
        }

        /// <summary>
        /// Iterates through all mod listings in load order
        /// </summary>
        public IEnumerator<ModListing<TMod>> GetEnumerator()
        {
            return _modsByLoadOrder.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Disposes all contained mods that implement IDisposable
        /// </summary>
        public void Dispose()
        {
            foreach (var mod in _modsByLoadOrder)
            {
                if (mod.Mod is IDisposable disp)
                {
                    disp.Dispose();
                }
            }
        }
    }
}
