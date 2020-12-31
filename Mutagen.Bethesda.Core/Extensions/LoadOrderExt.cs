using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda
{
    public static class LoadOrderExt
    {
        public static IEnumerable<LoadOrderListing> OnlyEnabled(this IEnumerable<LoadOrderListing> loadOrder)
        {
            return loadOrder.Where(x => x.Enabled);
        }

        public static IEnumerable<IModListing<TMod>> OnlyEnabled<TMod>(this IEnumerable<IModListing<TMod>> loadOrder)
            where TMod : class, IModGetter
        {
            return loadOrder.Where(x => x.Enabled);
        }

        public static IEnumerable<LoadOrderListing> AsListings(this IEnumerable<ModKey> loadOrder)
        {
            return loadOrder.Select(x => new LoadOrderListing(x, true));
        }
    }
}
