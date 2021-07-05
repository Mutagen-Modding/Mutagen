using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Masters.DI;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI
{
    public interface IFindImplicitlyIncludedMods
    {
        IEnumerable<ModKey> Find(IEnumerable<IModListingGetter> loadOrderListing);
    }

    public class FindImplicitlyIncludedMods : IFindImplicitlyIncludedMods
    {
        private readonly IDataDirectoryProvider _dataDirectoryProvider;
        private readonly IMasterReferenceReaderFactory _readerFactory;

        public FindImplicitlyIncludedMods(
            IDataDirectoryProvider dataDirectoryProvider,
            IMasterReferenceReaderFactory readerFactory)
        {
            _dataDirectoryProvider = dataDirectoryProvider;
            _readerFactory = readerFactory;
        }
        
        public IEnumerable<ModKey> Find(IEnumerable<IModListingGetter> loadOrderListing)
        {            
            var listingToIndices = loadOrderListing
                .ToDictionary(x => x.ModKey);
            HashSet<ModKey> referencedMasters = new();
            Queue<ModKey> toCheck = new(listingToIndices
                .Where(x => x.Value.Enabled)
                .Select(x => x.Value.ModKey));
            while (toCheck.Count > 0)
            {
                var key = toCheck.Dequeue();
                if (!referencedMasters.Add(key)) continue;
                if (!listingToIndices.TryGetValue(key, out var listing)) continue;
                if (!listing.Enabled)
                {
                    yield return listing.ModKey;
                }
                var reader = _readerFactory.FromPath(Path.Combine(_dataDirectoryProvider.Path, listing.ModKey.FileName));
                foreach (var master in reader.Masters)
                {
                    if (!referencedMasters.Contains(master.Master))
                    {
                        toCheck.Enqueue(master.Master);
                    }
                }
            }
        }
    }
}