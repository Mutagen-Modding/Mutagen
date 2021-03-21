using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda
{
    public static class LoadOrderExt
    {
        /// <summary>
        /// Filters listings to only include ones that are enabled
        /// </summary>
        /// <param name="loadOrder">Listings to filter</param>
        /// <returns>Listings that are enabled</returns>
        public static IEnumerable<LoadOrderListing> OnlyEnabled(this IEnumerable<LoadOrderListing> loadOrder)
        {
            return loadOrder.Where(x => x.Enabled);
        }

        /// <summary>
        /// Filters listings to only include ones that are enabled
        /// </summary>
        /// <param name="loadOrder">Listings to filter</param>
        /// <returns>Listings that are enabled</returns>
        public static IEnumerable<IModListing<TMod>> OnlyEnabled<TMod>(this IEnumerable<IModListing<TMod>> loadOrder)
            where TMod : class, IModGetter
        {
            return loadOrder.Where(x => x.Enabled);
        }

        /// <summary>
        /// Filters listings to only include ones whos mods exist
        /// </summary>
        /// <param name="loadOrder">Listings to filter</param>
        /// <returns>Listings that have mods that exist</returns>
        public static IEnumerable<IModListing<TMod>> OnlyExisting<TMod>(this IEnumerable<IModListing<TMod>> loadOrder)
            where TMod : class, IModGetter
        {
            return loadOrder
                .Where(x => x.Mod != null);
        }

        /// <summary>
        /// Filters listings to only include ones that are enabled and mods exist
        /// </summary>
        /// <param name="loadOrder">Listings to filter</param>
        /// <returns>Listings that are enabled and have mods that exist</returns>
        public static IEnumerable<IModListing<TMod>> OnlyEnabledAndExisting<TMod>(this IEnumerable<IModListing<TMod>> loadOrder)
            where TMod : class, IModGetter
        {
            return loadOrder
                .Where(x => x.Enabled && x.Mod != null);
        }

        /// <summary>
        /// Converts listings to Mods.  Will throw if any mods do not exist
        /// </summary>
        /// <param name="loadOrder">Listings to convert</param>
        /// <exception cref="MissingModException">Thrown if a listing is missing its mod</exception>
        /// <returns>Mods contained in the listings</returns>
        public static IEnumerable<TMod> Resolve<TMod>(this IEnumerable<IModListing<TMod>> loadOrder)
            where TMod : class, IModGetter
        {
            return loadOrder
                .Select(l =>
                {
                    if (l.Mod == null)
                    {
                        throw new MissingModException(l.ModKey);
                    }
                    return l.Mod;
                });
        }

        /// <summary>
        /// Converts ModKeys to LoadOrderListing objects
        /// </summary>
        /// <param name="loadOrder">ModKeys to convert</param>
        /// <param name="markEnabled">Whether to mark the listings as enabled</param>
        /// <returns>ModKeys as LoadOrderListing objects</returns>
        public static IEnumerable<LoadOrderListing> AsListings(this IEnumerable<ModKey> loadOrder, bool markEnabled = true)
        {
            return loadOrder.Select(x => new LoadOrderListing(x, markEnabled));
        }
    }
}
