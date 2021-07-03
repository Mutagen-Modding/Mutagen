using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.DI;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI
{
    public interface ILoadOrderImporter<TMod>
        where TMod : class, IModGetter
    {
        /// <summary>
        /// Returns a load order filled with mods constructed
        /// </summary>
        ILoadOrder<IModListing<TMod>> Import();
    }

    public class LoadOrderImporter<TMod> : ILoadOrderImporter<TMod>
        where TMod : class, IModGetter
    {
        private readonly IFileSystem _fileSystem;
        private readonly IDataDirectoryContext _dataDirectoryContext;
        private readonly ILoadOrderListingsProvider _loadOrderListingsProvider;
        private readonly IModImporter<TMod> _importer;

        public LoadOrderImporter(
            IFileSystem fileSystem,
            IDataDirectoryContext dataDirectoryContext,
            ILoadOrderListingsProvider loadOrderListingsProvider,
            IModImporter<TMod> importer)
        {
            _fileSystem = fileSystem;
            _dataDirectoryContext = dataDirectoryContext;
            _loadOrderListingsProvider = loadOrderListingsProvider;
            _importer = importer;
        }
        
        public ILoadOrder<IModListing<TMod>> Import()
        {
            var loList = _loadOrderListingsProvider.Get().ToList();
            var results = new (ModKey ModKey, int ModIndex, TryGet<TMod> Mod, bool Enabled)[loList.Count];
            try
            {
                Parallel.ForEach(loList, (listing, _, modIndex) =>
                {
                    try
                    {
                        var modPath = new ModPath(listing.ModKey, _dataDirectoryContext.Path.GetFile(listing.ModKey.FileName).Path);
                        if (!_fileSystem.File.Exists(modPath.Path))
                        {
                            results[modIndex] = (listing.ModKey, (int)modIndex, TryGet<TMod>.Failure, listing.Enabled);
                            return;
                        }
                        var mod = _importer.Import(modPath);
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