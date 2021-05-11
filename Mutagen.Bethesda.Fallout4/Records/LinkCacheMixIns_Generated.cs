using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Cache.Implementations;

namespace Mutagen.Bethesda.Fallout4
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
        public static ImmutableModLinkCache<IFallout4Mod, IFallout4ModGetter> ToImmutableLinkCache(this IFallout4ModGetter mod)
        {
            return mod.ToImmutableLinkCache<IFallout4Mod, IFallout4ModGetter>();
        }

        /// <summary>
        /// Creates a Link Cache using a single mod as its link target.  Mod is allowed to be modified afterwards, but
        /// this comes at a performance cost of not allowing much caching to be done.  If the mod is not expected to
        /// be modified afterwards, use ImmutableModLinkCache instead.<br/>
        /// </summary>
        /// <param name="mod">Mod to construct the package relative to</param>
        /// <returns>LinkPackage attached to given mod</returns>
        public static MutableModLinkCache<IFallout4Mod, IFallout4ModGetter> ToMutableLinkCache(this IFallout4ModGetter mod)
        {
            return mod.ToMutableLinkCache<IFallout4Mod, IFallout4ModGetter>();
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<IFallout4Mod, IFallout4ModGetter> ToImmutableLinkCache(this LoadOrder<IFallout4ModGetter> loadOrder)
        {
            return loadOrder.ToImmutableLinkCache<IFallout4Mod, IFallout4ModGetter>();
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<IFallout4Mod, IFallout4ModGetter> ToImmutableLinkCache(this LoadOrder<IModListingGetter<IFallout4ModGetter>> loadOrder)
        {
            return loadOrder.ToImmutableLinkCache<IFallout4Mod, IFallout4ModGetter>();
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<IFallout4Mod, IFallout4ModGetter> ToImmutableLinkCache(this IEnumerable<IModListingGetter<IFallout4ModGetter>> loadOrder)
        {
            return loadOrder.ToImmutableLinkCache<IFallout4Mod, IFallout4ModGetter>();
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<IFallout4Mod, IFallout4ModGetter> ToImmutableLinkCache(this IEnumerable<IFallout4ModGetter> loadOrder)
        {
            return loadOrder.ToImmutableLinkCache<IFallout4Mod, IFallout4ModGetter>();
        }

        /// <summary>
        /// Creates a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static MutableLoadOrderLinkCache<IFallout4Mod, IFallout4ModGetter> ToMutableLinkCache(
            this LoadOrder<IFallout4ModGetter> immutableBaseCache,
            params IFallout4Mod[] mutableMods)
        {
            return immutableBaseCache.ToMutableLinkCache<IFallout4Mod, IFallout4ModGetter>(mutableMods);
        }

        /// <summary>
        /// Creates a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static MutableLoadOrderLinkCache<IFallout4Mod, IFallout4ModGetter> ToMutableLinkCache(
            this LoadOrder<IModListingGetter<IFallout4ModGetter>> immutableBaseCache,
            params IFallout4Mod[] mutableMods)
        {
            return immutableBaseCache.ToMutableLinkCache<IFallout4Mod, IFallout4ModGetter>(mutableMods);
        }

        /// <summary>
        /// Creates a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static MutableLoadOrderLinkCache<IFallout4Mod, IFallout4ModGetter> ToMutableLinkCache(
            this IEnumerable<IFallout4ModGetter> immutableBaseCache,
            params IFallout4Mod[] mutableMods)
        {
            return immutableBaseCache.ToMutableLinkCache<IFallout4Mod, IFallout4ModGetter>(mutableMods);
        }

    }
}
