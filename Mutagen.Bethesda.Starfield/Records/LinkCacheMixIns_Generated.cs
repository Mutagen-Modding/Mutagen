using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;

namespace Mutagen.Bethesda.Starfield
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
        public static ImmutableModLinkCache<IStarfieldMod, IStarfieldModGetter> ToImmutableLinkCache(this IStarfieldModGetter mod)
        {
            return mod.ToImmutableLinkCache<IStarfieldMod, IStarfieldModGetter>();
        }

        /// <summary>
        /// Creates a Link Cache using a single mod as its link target.  Mod is allowed to be modified afterwards, but
        /// this comes at a performance cost of not allowing much caching to be done.  If the mod is not expected to
        /// be modified afterwards, use ImmutableModLinkCache instead.<br/>
        /// </summary>
        /// <param name="mod">Mod to construct the package relative to</param>
        /// <returns>LinkPackage attached to given mod</returns>
        public static MutableModLinkCache<IStarfieldMod, IStarfieldModGetter> ToMutableLinkCache(this IStarfieldModGetter mod)
        {
            return mod.ToMutableLinkCache<IStarfieldMod, IStarfieldModGetter>();
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<IStarfieldMod, IStarfieldModGetter> ToImmutableLinkCache(this ILoadOrderGetter<IStarfieldModGetter> loadOrder)
        {
            return loadOrder.ToImmutableLinkCache<IStarfieldMod, IStarfieldModGetter>();
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<IStarfieldMod, IStarfieldModGetter> ToImmutableLinkCache(this ILoadOrderGetter<IModListingGetter<IStarfieldModGetter>> loadOrder)
        {
            return loadOrder.ToImmutableLinkCache<IStarfieldMod, IStarfieldModGetter>();
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<IStarfieldMod, IStarfieldModGetter> ToImmutableLinkCache(this IEnumerable<IModListingGetter<IStarfieldModGetter>> loadOrder)
        {
            return loadOrder.ToImmutableLinkCache<IStarfieldMod, IStarfieldModGetter>();
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<IStarfieldMod, IStarfieldModGetter> ToImmutableLinkCache(this IEnumerable<IStarfieldModGetter> loadOrder)
        {
            return loadOrder.ToImmutableLinkCache<IStarfieldMod, IStarfieldModGetter>();
        }

        /// <summary>
        /// Creates a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ILinkCache<IStarfieldMod, IStarfieldModGetter> ToMutableLinkCache(
            this ILoadOrderGetter<IStarfieldModGetter> immutableBaseCache,
            params IStarfieldMod[] mutableMods)
        {
            return immutableBaseCache.ToMutableLinkCache<IStarfieldMod, IStarfieldModGetter>(mutableMods);
        }

        /// <summary>
        /// Creates a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ILinkCache<IStarfieldMod, IStarfieldModGetter> ToMutableLinkCache(
            this ILoadOrderGetter<IModListingGetter<IStarfieldModGetter>> immutableBaseCache,
            params IStarfieldMod[] mutableMods)
        {
            return immutableBaseCache.ToMutableLinkCache<IStarfieldMod, IStarfieldModGetter>(mutableMods);
        }

        /// <summary>
        /// Creates a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ILinkCache<IStarfieldMod, IStarfieldModGetter> ToMutableLinkCache(
            this IEnumerable<IStarfieldModGetter> immutableBaseCache,
            params IStarfieldMod[] mutableMods)
        {
            return immutableBaseCache.ToMutableLinkCache<IStarfieldMod, IStarfieldModGetter>(mutableMods);
        }

    }
}
