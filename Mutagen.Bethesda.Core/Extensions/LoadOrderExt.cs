using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda
{
    public static class LoadOrderExt
    {
        /// <summary>
        /// Filters listings to only include ones that are enabled
        /// </summary>
        /// <param name="loadOrder">Listings to filter</param>
        /// <returns>Listings that are enabled</returns>
        public static IEnumerable<TListing> OnlyEnabled<TListing>(this IEnumerable<TListing> loadOrder)
            where TListing : IModListingGetter
        {
            return loadOrder.Where(x => x.Enabled);
        }

        /// <summary>
        /// Filters listings to only include ones where the mod objects exist
        /// </summary>
        /// <param name="loadOrder">Listings to filter</param>
        /// <returns>Listings that have mods that exist</returns>
        public static IEnumerable<TListing> OnlyExisting<TListing, TMod>(this IEnumerable<TListing> loadOrder)
            where TListing : IModListingGetter<TMod>
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
        public static IEnumerable<TListing> OnlyEnabledAndExisting<TListing>(this IEnumerable<TListing> loadOrder)
            where TListing : IModListingGetter<IModGetter>
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
        public static IEnumerable<TMod> Resolve<TMod>(this IEnumerable<IModListingGetter<TMod>> loadOrder)
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
        public static IEnumerable<IModListingGetter> AsListings(this IEnumerable<ModKey> loadOrder, bool markEnabled = true)
        {
            return loadOrder.Select(x => new ModListing(x, markEnabled));
        }

        public static bool TryGetIndex<TListing>(this ILoadOrderGetter<TListing> loadOrder, int index, [MaybeNullWhen(false)] out TListing listing)
            where TListing : IModKeyed
        {
            var result = loadOrder.TryGetAtIndex(index);
            if (result == null)
            {
                listing = default;
                return false;
            }

            listing = result;
            return true;
        }
    }
}
