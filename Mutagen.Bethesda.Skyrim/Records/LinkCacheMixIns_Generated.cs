using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Cache.Implementations;

namespace Mutagen.Bethesda.Skyrim
{
    public static class LinkCacheMixIns
    {
        /// <summary>
        /// Creates a Link Cache using a single mod as its link target. <br/>
        /// Modification of the target Mod is not safe.  Internal caches can become incorrect if 
        /// modifications occur on content already cached.
        /// </summary>
        /// <param name="mod">Mod to construct the package relative to</param>
        /// <returns>LinkPackage attached to given mod</returns>
        public static ImmutableModLinkCache<ISkyrimMod, ISkyrimModGetter> ToImmutableLinkCache(this ISkyrimModGetter mod)
        {
            return mod.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>();
        }

        /// <summary>
        /// Creates a Link Cache using a single mod as its link target.  Mod is allowed to be modified afterwards, but
        /// this comes at a performance cost of not allowing much caching to be done.  If the mod is not expected to
        /// be modified afterwards, use ImmutableModLinkCache instead.<br/>
        /// </summary>
        /// <param name="mod">Mod to construct the package relative to</param>
        /// <returns>LinkPackage attached to given mod</returns>
        public static MutableModLinkCache<ISkyrimMod, ISkyrimModGetter> ToMutableLinkCache(this ISkyrimModGetter mod)
        {
            return mod.ToMutableLinkCache<ISkyrimMod, ISkyrimModGetter>();
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter> ToImmutableLinkCache(this ILoadOrderGetter<ISkyrimModGetter> loadOrder)
        {
            return loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>();
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter> ToImmutableLinkCache(this ILoadOrderGetter<IModListingGetter<ISkyrimModGetter>> loadOrder)
        {
            return loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>();
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter> ToImmutableLinkCache(this IEnumerable<IModListingGetter<ISkyrimModGetter>> loadOrder)
        {
            return loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>();
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter> ToImmutableLinkCache(this IEnumerable<ISkyrimModGetter> loadOrder)
        {
            return loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>();
        }

        /// <summary>
        /// Creates a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static MutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter> ToMutableLinkCache(
            this ILoadOrderGetter<ISkyrimModGetter> immutableBaseCache,
            params ISkyrimMod[] mutableMods)
        {
            return immutableBaseCache.ToMutableLinkCache<ISkyrimMod, ISkyrimModGetter>(mutableMods);
        }

        /// <summary>
        /// Creates a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static MutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter> ToMutableLinkCache(
            this ILoadOrderGetter<IModListingGetter<ISkyrimModGetter>> immutableBaseCache,
            params ISkyrimMod[] mutableMods)
        {
            return immutableBaseCache.ToMutableLinkCache<ISkyrimMod, ISkyrimModGetter>(mutableMods);
        }

        /// <summary>
        /// Creates a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static MutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter> ToMutableLinkCache(
            this IEnumerable<ISkyrimModGetter> immutableBaseCache,
            params ISkyrimMod[] mutableMods)
        {
            return immutableBaseCache.ToMutableLinkCache<ISkyrimMod, ISkyrimModGetter>(mutableMods);
        }

    }
}
