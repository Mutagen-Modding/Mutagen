using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda
{
    public static class LoadOrderExt
    {
        public static IEnumerable<ModKey> OnlyEnabled(this IEnumerable<LoadOrderListing> loadOrder)
        {
            return loadOrder.Where(x => x.Enabled)
                .Select(x => x.ModKey);
        }

        public static IEnumerable<LoadOrderListing> AsListings(this IEnumerable<ModKey> loadOrder)
        {
            return loadOrder.Select(x => new LoadOrderListing(x, true));
        }
    }
}
