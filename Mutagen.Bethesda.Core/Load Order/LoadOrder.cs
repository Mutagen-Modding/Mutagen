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
    public class LoadOrder
    {
        public static bool TryGetPluginsFile(GameMode game, out FilePath path)
        {
            string pluginPath;
            switch (game)
            {
                case GameMode.Oblivion:
                    pluginPath = "Oblivion/Plugins.txt";
                    break;
                default:
                    throw new NotImplementedException();
            }
            path = new FilePath(
                Path.Combine(
                    Environment.GetEnvironmentVariable("LocalAppData"),
                    pluginPath));
            return path.Exists;
        }

        public static bool TryCreateLoadOrder(
            FilePath pluginListPath,
            DirectoryPath dataPath,
            out List<ModKey> loadOrder)
        {
            List<(ModKey ModKey, DateTime Write)> list = new List<(ModKey ModKey, DateTime Write)>();
            loadOrder = new List<ModKey>();
            foreach (var item in File.ReadAllLines(pluginListPath.Path))
            {
                var str = item;
                var commentIndex = str.IndexOf('#');
                if (commentIndex != -1)
                {
                    str = str.Substring(0, commentIndex);
                }
                if (string.IsNullOrWhiteSpace(str)) continue;
                str = str.Trim();
                if (!ModKey.TryFactory(str, out var key)) return false;
                FilePath file = new FilePath(
                    Path.Combine(dataPath.Path, str));
                if (!file.Exists) return false;
                list.Add((key, file.Info.LastWriteTime));
            }
            loadOrder.AddRange(list
                .OrderBy(i => i.Write)
                .Select(i => i.ModKey));
            return true;
        }

        public static bool TryGetUsualLoadOrder(GameMode game, DirectoryPath dataPath, [MaybeNullWhen(false)]out List<ModKey> loadOrder)
        {
            if (!TryGetPluginsFile(game, out var path))
            {
                loadOrder = default!;
                return false;
            }
            return TryCreateLoadOrder(path, dataPath, out loadOrder);
        }
    }

    public class LoadOrder<TMod> : IEnumerable<ModListing<TMod>>
        where TMod : class, IModGetter
    {
        private readonly List<ModListing<TMod>> _modsByLoadOrder = new List<ModListing<TMod>>();

        public int Count => _modsByLoadOrder.Count;

        public ModListing<TMod> this[LoadOrderIndex index] => _modsByLoadOrder[index.ID];

        public bool TryGetListing(ModKey key, out (LoadOrderIndex Index, ModListing<TMod> Listing) result)
        {
            for (int i = 0; i < _modsByLoadOrder.Count; i++)
            {
                var item = _modsByLoadOrder[i];
                if (item.Key.Equals(key))
                {
                    result = (new LoadOrderIndex(i), _modsByLoadOrder[i]);
                    return true;
                }
            }
            result = default(ValueTuple<LoadOrderIndex, ModListing<TMod>>);
            return false;
        }

        public bool TryGetMod(ModKey key, out (LoadOrderIndex Index, TMod Mod) result)
        {
            if (!this.TryGetListing(key, out var listing)
                || listing.Listing.Mod == null)
            {
                result = default((LoadOrderIndex, TMod));
                return false;
            }
            result = (listing.Index, listing.Listing.Mod);
            return true;
        }

        public bool TryGetIndex(LoadOrderIndex index, [MaybeNullWhen(false)] out ModListing<TMod> result)
        {
            if (!_modsByLoadOrder.InRange(index.ID))
            {
                result = default!;
                return false;
            }
            result = _modsByLoadOrder[index.ID];
            return result != null;
        }

        public void Add(TMod mod)
        {
            if (this.Contains(mod.ModKey))
            {
                throw new ArgumentException("Mod was already present on the mod list.");
            }
            _modsByLoadOrder.Add(new ModListing<TMod>(mod));
        }

        public void Add(TMod mod, byte index)
        {
            if (this.Contains(mod.ModKey))
            {
                throw new ArgumentException("Mod was already present on the mod list.");
            }
            _modsByLoadOrder.Insert(index, new ModListing<TMod>(mod));
        }

        public bool Contains(ModKey mod)
        {
            return IndexOf(mod) != -1;
        }

        public int IndexOf(ModKey mod)
        {
            for (int i = 0; i < _modsByLoadOrder.Count; i++)
            {
                if (_modsByLoadOrder[i].Key.Equals(mod))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Clear()
        {
            this._modsByLoadOrder.Clear();
        }

        public delegate bool Importer(FilePath path, ModKey modKey, [MaybeNullWhen(false)] out TMod mod);

        public void Import(
            DirectoryPath dataFolder,
            List<ModKey> loadOrder,
            Importer importer)
        {
            this.Clear();
            var results = new (ModKey ModKey, int ModIndex, TryGet<TMod> Mod)[loadOrder.Count];
            Parallel.ForEach(loadOrder, (modKey, state, modIndex) =>
            {
                FilePath modPath = dataFolder.GetFile(modKey.FileName);
                if (!modPath.Exists)
                {
                    results[modIndex] = (modKey, (int)modIndex, TryGet<TMod>.Failure);
                    return;
                }
                if (!importer(modPath, modKey, out var mod))
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

        public IEnumerator<ModListing<TMod>> GetEnumerator()
        {
            return _modsByLoadOrder.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
