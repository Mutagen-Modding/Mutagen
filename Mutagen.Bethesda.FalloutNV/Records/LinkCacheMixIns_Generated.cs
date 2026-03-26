using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;

namespace Mutagen.Bethesda.FalloutNV
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
        public static ImmutableModLinkCache<IFalloutNVMod, IFalloutNVModGetter> ToImmutableLinkCache(this IFalloutNVModGetter mod)
        {
            return mod.ToImmutableLinkCache<IFalloutNVMod, IFalloutNVModGetter>();
        }

        /// <summary>
        /// Creates a Link Cache using a single mod as its link target.  Mod is allowed to be modified afterwards, but
        /// this comes at a performance cost of not allowing much caching to be done.  If the mod is not expected to
        /// be modified afterwards, use ImmutableModLinkCache instead.<br/>
        /// </summary>
        /// <param name="mod">Mod to construct the package relative to</param>
        /// <returns>LinkPackage attached to given mod</returns>
        public static MutableModLinkCache<IFalloutNVMod, IFalloutNVModGetter> ToMutableLinkCache(this IFalloutNVModGetter mod)
        {
            return mod.ToMutableLinkCache<IFalloutNVMod, IFalloutNVModGetter>();
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<IFalloutNVMod, IFalloutNVModGetter> ToImmutableLinkCache(this ILoadOrderGetter<IFalloutNVModGetter> loadOrder)
        {
            return loadOrder.ToImmutableLinkCache<IFalloutNVMod, IFalloutNVModGetter>();
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<IFalloutNVMod, IFalloutNVModGetter> ToImmutableLinkCache(this ILoadOrderGetter<IModListingGetter<IFalloutNVModGetter>> loadOrder)
        {
            return loadOrder.ToImmutableLinkCache<IFalloutNVMod, IFalloutNVModGetter>();
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<IFalloutNVMod, IFalloutNVModGetter> ToImmutableLinkCache(this IEnumerable<IModListingGetter<IFalloutNVModGetter>> loadOrder)
        {
            return loadOrder.ToImmutableLinkCache<IFalloutNVMod, IFalloutNVModGetter>();
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<IFalloutNVMod, IFalloutNVModGetter> ToImmutableLinkCache(this IEnumerable<IFalloutNVModGetter> loadOrder)
        {
            return loadOrder.ToImmutableLinkCache<IFalloutNVMod, IFalloutNVModGetter>();
        }

        /// <summary>
        /// Creates a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ILinkCache<IFalloutNVMod, IFalloutNVModGetter> ToMutableLinkCache(
            this ILoadOrderGetter<IFalloutNVModGetter> immutableBaseCache,
            params IFalloutNVMod[] mutableMods)
        {
            return immutableBaseCache.ToMutableLinkCache<IFalloutNVMod, IFalloutNVModGetter>(mutableMods);
        }

        /// <summary>
        /// Creates a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ILinkCache<IFalloutNVMod, IFalloutNVModGetter> ToMutableLinkCache(
            this ILoadOrderGetter<IModListingGetter<IFalloutNVModGetter>> immutableBaseCache,
            params IFalloutNVMod[] mutableMods)
        {
            return immutableBaseCache.ToMutableLinkCache<IFalloutNVMod, IFalloutNVModGetter>(mutableMods);
        }

        /// <summary>
        /// Creates a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ILinkCache<IFalloutNVMod, IFalloutNVModGetter> ToMutableLinkCache(
            this IEnumerable<IFalloutNVModGetter> immutableBaseCache,
            params IFalloutNVMod[] mutableMods)
        {
            return immutableBaseCache.ToMutableLinkCache<IFalloutNVMod, IFalloutNVModGetter>(mutableMods);
        }

    }
}
