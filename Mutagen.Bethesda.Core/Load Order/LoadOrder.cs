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

        public async Task Import(
            DirectoryPath dataFolder,
            List<ModKey> loadOrder,
            Func<FilePath, ModKey, Task<TryGet<TMod>>> importer)
        {
            this.Clear();
            int index = 0;
            var results = await Task.WhenAll(
                loadOrder.Select(modKey =>
                {
                    var modIndex = index++;
                    return Task.Run(async () =>
                    {
                        FilePath modPath = dataFolder.GetFile(modKey.FileName);
                        if (!modPath.Exists) return (modKey, modIndex, TryGet<TMod>.Failure);
                        return (modKey, modIndex, await importer(modPath, modKey).ConfigureAwait(false));
                    });
                })).ConfigureAwait(false);
            foreach (var item in results
                .OrderBy(i => i.modIndex))
            {
                if (item.Item3.Succeeded)
                {
                    this._modsByLoadOrder.Add(
                        new ModListing<TMod>(
                            item.Item3.Value));
                }
                else
                {
                    this._modsByLoadOrder.Add(
                        ModListing<TMod>.UnloadedModListing(item.modKey));
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
