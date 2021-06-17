using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order
{
    public interface ILoadOrderImporter
    {
        /// <summary>
        /// Constructs a load order filled with mods constructed
        /// </summary>
        /// <param name="dataFolder">Path data folder containing mods</param>
        /// <param name="loadOrder">Unique list of listings to import</param>
        /// <param name="gameRelease">GameRelease associated with the mods to create<br/>
        /// This may be unapplicable to some games with only one release, but should still be passed in.
        /// </param>
        ILoadOrder<IModListing<TMod>> Import<TMod>(
            DirectoryPath dataFolder,
            IEnumerable<IModListingGetter> loadOrder,
            GameRelease gameRelease)
            where TMod : class, IModGetter;

        /// <summary>
        /// Constructs a load order filled with mods constructed
        /// </summary>
        /// <param name="dataFolder">Path data folder containing mods</param>
        /// <param name="loadOrder">Unique list of mod keys to import</param>
        /// <param name="gameRelease">GameRelease associated with the mods to create<br/>
        /// This may be unapplicable to some games with only one release, but should still be passed in.
        /// </param>
        LoadOrder<IModListing<TMod>> Import<TMod>(
            DirectoryPath dataFolder,
            IEnumerable<ModKey> loadOrder,
            GameRelease gameRelease)
            where TMod : class, IModGetter;

        /// <summary>
        /// Constructs a load order filled with mods constructed by given importer func
        /// </summary>
        /// <param name="dataFolder">Path data folder containing mods</param>
        /// <param name="loadOrder">Unique list of mod keys to import</param>
        /// <param name="factory">Func to use to create a new mod from a path</param>
        LoadOrder<IModListing<TMod>> Import<TMod>(
            DirectoryPath dataFolder,
            IEnumerable<ModKey> loadOrder,
            Func<ModPath, TMod> factory)
            where TMod : class, IModGetter;

        /// <summary>
        /// Constructs a load order filled with mods constructed by given importer func
        /// </summary>
        /// <param name="dataFolder">Path data folder containing mods</param>
        /// <param name="loadOrder">Unique list of listings to import</param>
        /// <param name="factory">Func to use to create a new mod from a path</param>
        LoadOrder<IModListing<TMod>> Import<TMod>(
            DirectoryPath dataFolder,
            IEnumerable<IModListingGetter> loadOrder,
            Func<ModPath, TMod> factory)
            where TMod : class, IModGetter;
    }

    public class LoadOrderImporter : ILoadOrderImporter
    {
        /// <inheritdoc />
        public ILoadOrder<IModListing<TMod>> Import<TMod>(
            DirectoryPath dataFolder,
            IEnumerable<IModListingGetter> loadOrder,
            GameRelease gameRelease)
            where TMod : class, IModGetter
        {
            return Import(
                dataFolder,
                loadOrder,
                (modPath) => ModInstantiator<TMod>.Importer(modPath, gameRelease));
        }

        /// <inheritdoc />
        public LoadOrder<IModListing<TMod>> Import<TMod>(
            DirectoryPath dataFolder,
            IEnumerable<ModKey> loadOrder,
            GameRelease gameRelease)
            where TMod : class, IModGetter
        {
            return Import(
                dataFolder,
                loadOrder.Select(m => new ModListing(m, enabled: true)),
                (modPath) => ModInstantiator<TMod>.Importer(modPath, gameRelease));
        }

        /// <inheritdoc />
        public LoadOrder<IModListing<TMod>> Import<TMod>(
            DirectoryPath dataFolder,
            IEnumerable<ModKey> loadOrder,
            Func<ModPath, TMod> factory)
            where TMod : class, IModGetter
        {
            return Import(
                dataFolder,
                loadOrder.Select(m => new ModListing(m, enabled: true)),
                factory);
        }

        /// <inheritdoc />
        public LoadOrder<IModListing<TMod>> Import<TMod>(
            DirectoryPath dataFolder,
            IEnumerable<IModListingGetter> loadOrder,
            Func<ModPath, TMod> factory)
            where TMod : class, IModGetter
        {
            var loList = loadOrder.ToList();
            var results = new (ModKey ModKey, int ModIndex, TryGet<TMod> Mod, bool Enabled)[loList.Count];
            try
            {
                Parallel.ForEach(loList, (listing, state, modIndex) =>
                {
                    try
                    {
                        var modPath = new ModPath(listing.ModKey, dataFolder.GetFile(listing.ModKey.FileName).Path);
                        if (!modPath.Path.Exists)
                        {
                            results[modIndex] = (listing.ModKey, (int)modIndex, TryGet<TMod>.Failure, listing.Enabled);
                            return;
                        }
                        var mod = factory(modPath);
                        results[modIndex] = (listing.ModKey, (int)modIndex, TryGet<TMod>.Succeed(mod), listing.Enabled);
                    }
                    catch (Exception ex)
                    {
                        throw RecordException.Enrich(ex, listing.ModKey);
                    }
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
                            return ModListing<TMod>.CreateUnloaded(item.ModKey, item.Enabled);
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
}