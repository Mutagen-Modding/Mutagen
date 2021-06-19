using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Order;
using Noggog;

namespace Mutagen.Bethesda.Core.Plugins.Order
{
    public interface IFindImplicitlyIncludedMods
    {
        IEnumerable<ModKey> Find(
            GameRelease release, 
            DirectoryPath dataFolder, 
            IEnumerable<IModListingGetter> loadOrderListing);
    }

    public class FindImplicitlyIncludedMods : IFindImplicitlyIncludedMods
    {
        private readonly IMasterReferenceReaderFactory _readerFactory;

        public FindImplicitlyIncludedMods(IMasterReferenceReaderFactory readerFactory)
        {
            _readerFactory = readerFactory;
        }
        
        public IEnumerable<ModKey> Find(
            GameRelease release, 
            DirectoryPath dataFolder, 
            IEnumerable<IModListingGetter> loadOrderListing)
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
                var reader = _readerFactory.FromPath(Path.Combine(dataFolder, listing.ModKey.FileName), release);
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