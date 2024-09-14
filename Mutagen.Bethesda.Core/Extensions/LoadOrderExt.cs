using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda;

public static class LoadOrderExt
{
    /// <summary>
    /// Filters listings to only include ones that are enabled
    /// </summary>
    /// <param name="loadOrder">Listings to filter</param>
    /// <returns>Listings that are enabled</returns>
    public static IEnumerable<TListing> OnlyEnabled<TListing>(this IEnumerable<TListing> loadOrder)
        where TListing : ILoadOrderListingGetter
    {
        return loadOrder.Where(x => x.Enabled);
    }

    /// <summary>
    /// Filters listings to only include ones where the mod objects exist
    /// </summary>
    /// <param name="loadOrder">Listings to filter</param>
    /// <returns>Listings that have mods that exist</returns>
    public static IEnumerable<TListing> OnlyExisting<TListing, TMod>(this IEnumerable<TListing> loadOrder)
        where TListing : IModListingGetter
        where TMod : class, IModGetter
    {
        return loadOrder
            .Where(x => x.ExistsOnDisk);
    }

    /// <summary>
    /// Filters listings to only include ones that are enabled and mods exist
    /// </summary>
    /// <param name="loadOrder">Listings to filter</param>
    /// <returns>Listings that are enabled and have mods that exist</returns>
    public static IEnumerable<TListing> OnlyEnabledAndExisting<TListing>(this IEnumerable<TListing> loadOrder)
        where TListing : IModListingGetter
    {
        return loadOrder
            .Where(x => x.Enabled && x.ExistsOnDisk);
    }

    /// <summary>
    /// Converts listings to Mods.  Will throw if any mods do not exist
    /// </summary>
    /// <param name="loadOrder">Listings to convert</param>
    /// <exception cref="MissingModException">Thrown if a listing is missing its mod</exception>
    /// <returns>Mods contained in the listings</returns>
    [Obsolete("Use ResolveAllModsExist instead")]
    public static IEnumerable<TMod> Resolve<TMod>(this IEnumerable<IModListingGetter<TMod>> loadOrder)
        where TMod : class, IModGetter
    {
        return ResolveAllModsExist(loadOrder);
    }

    /// <summary>
    /// Converts listings to Mods.  Will throw if any mods do not exist
    /// </summary>
    /// <param name="loadOrder">Listings to convert</param>
    /// <exception cref="MissingModException">Thrown if a listing is missing its mod</exception>
    /// <returns>Mods contained in the listings</returns>
    public static IEnumerable<TModItem> ResolveAllModsExist<TModItem>(this IEnumerable<IModListingGetter<TModItem>> loadOrder)
        where TModItem : class, IModKeyed
    {
        loadOrder = loadOrder.ToArray();
        var missingMods = loadOrder.Where(x => x.Mod == null)
            .ToArray();

        if (missingMods.Length > 0)
        {
            throw new MissingModException(missingMods.Select(x => x.ModKey));
        }

        return loadOrder.Select(x => x.Mod!);
    }

    /// <summary>
    /// Converts any listings that have mods into Mods.  Will not throw
    /// </summary>
    /// <param name="loadOrder">Listings to convert</param>
    /// <returns>Mods contained in the listings that exist</returns>
    public static IEnumerable<TModItem> ResolveExistingMods<TModItem>(this IEnumerable<IModListingGetter<TModItem>> loadOrder)
        where TModItem : class, IModKeyed
    {
        loadOrder = loadOrder.ToArray();
        var missingMods = loadOrder.Where(x => x.Mod == null)
            .ToArray();

        if (missingMods.Length > 0)
        {
            throw new MissingModException(missingMods.Select(x => x.ModKey));
        }

        return loadOrder
            .Select(x => x.Mod)
            .NotNull();
    }

    /// <summary>
    /// Converts listings to Mods.  Will throw if any mods do not exist
    /// </summary>
    /// <param name="loadOrder">Listings to convert</param>
    /// <exception cref="MissingModException">Thrown if a listing is missing its mod</exception>
    /// <returns>Mods contained in the listings</returns>
    public static LoadOrder<TModItem> ResolveAllModsExist<TModItem>(
        this ILoadOrderGetter<IModListingGetter<TModItem>> loadOrder,
        bool? disposeItems = null)
        where TModItem : class, IModKeyed
    {
        return new LoadOrder<TModItem>(ResolveAllModsExist<TModItem>(loadOrder.ListedOrder), disposeItems: disposeItems ?? loadOrder.DisposingItems);
    }

    /// <summary>
    /// Converts ModKeys to LoadOrderListing objects
    /// </summary>
    /// <param name="loadOrder">ModKeys to convert</param>
    /// <param name="markEnabled">Whether to mark the listings as enabled</param>
    /// <returns>ModKeys as LoadOrderListing objects</returns>
    public static IEnumerable<ILoadOrderListingGetter> ToLoadOrderListings(this IEnumerable<ModKey> loadOrder, bool markEnabled = true)
    {
        return loadOrder.Select(x => new LoadOrderListing(x, markEnabled));
    }

    /// <summary>
    /// Converts ModKeys to LoadOrderListing objects
    /// </summary>
    /// <param name="loadOrder">ModKeys to convert</param>
    /// <param name="existsOnDisk">Whether to mark the ModListings as existing on disk</param>
    /// <param name="markEnabled">Whether to mark the listings as enabled</param>
    /// <returns>ModKeys as LoadOrderListing objects</returns>
    public static IEnumerable<IModListingGetter> ToModListings(this IEnumerable<ModKey> loadOrder, bool existsOnDisk, bool markEnabled = true)
    {
        return loadOrder.Select(x => new ModListing(x, enabled: markEnabled, existsOnDisk: existsOnDisk));
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

    public static LoadOrder<TListing> TrimAt<TListing>(this ILoadOrderGetter<TListing> loadOrder, ModKey modKey)
        where TListing : IModKeyed
    {
        return new LoadOrder<TListing>(loadOrder.ListedOrder.TrimAt(modKey));
    }

    public static IEnumerable<TListing> TrimAt<TListing>(this IEnumerable<TListing> loadOrder, ModKey modKey)
        where TListing : IModKeyed
    {
        return loadOrder.TakeWhile(x => x.ModKey != modKey);
    }

    public static LoadOrder<TRetListing> Transform<TListing, TRetListing>(this ILoadOrderGetter<TListing> loadOrder, Func<TListing, TRetListing> transformer)
        where TListing : IModKeyed
        where TRetListing : IModKeyed
    {
        return new LoadOrder<TRetListing>(
            loadOrder.ListedOrder
                .Select(transformer));
    }

    public static LoadOrder<TListing> Where<TListing>(this ILoadOrderGetter<TListing> loadOrder, Func<TListing, bool> filter)
        where TListing : IModKeyed
    {
        return new LoadOrder<TListing>(
            loadOrder.ListedOrder
                .Where(filter));
    }
}