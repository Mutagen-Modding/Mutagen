using System.Collections.Generic;
using Mutagen.Bethesda.LoadOrders;

namespace Mutagen.Bethesda.Oblivion
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
        public static ImmutableModLinkCache<IOblivionMod, IOblivionModGetter> ToImmutableLinkCache(this IOblivionModGetter mod)
        {
            return mod.ToImmutableLinkCache<IOblivionMod, IOblivionModGetter>();
        }

        /// <summary>
        /// Creates a Link Cache using a single mod as its link target.  Mod is allowed to be modified afterwards, but
        /// this comes at a performance cost of not allowing much caching to be done.  If the mod is not expected to
        /// be modified afterwards, use ImmutableModLinkCache instead.<br/>
        /// </summary>
        /// <param name="mod">Mod to construct the package relative to</param>
        /// <returns>LinkPackage attached to given mod</returns>
        public static MutableModLinkCache<IOblivionMod, IOblivionModGetter> ToMutableLinkCache(this IOblivionModGetter mod)
        {
            return mod.ToMutableLinkCache<IOblivionMod, IOblivionModGetter>();
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<IOblivionMod, IOblivionModGetter> ToImmutableLinkCache(this LoadOrder<IOblivionModGetter> loadOrder)
        {
            return loadOrder.ToImmutableLinkCache<IOblivionMod, IOblivionModGetter>();
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<IOblivionMod, IOblivionModGetter> ToImmutableLinkCache(this LoadOrder<IModListing<IOblivionModGetter>> loadOrder)
        {
            return loadOrder.ToImmutableLinkCache<IOblivionMod, IOblivionModGetter>();
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<IOblivionMod, IOblivionModGetter> ToImmutableLinkCache(this IEnumerable<IModListing<IOblivionModGetter>> loadOrder)
        {
            return loadOrder.ToImmutableLinkCache<IOblivionMod, IOblivionModGetter>();
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<IOblivionMod, IOblivionModGetter> ToImmutableLinkCache(this IEnumerable<IOblivionModGetter> loadOrder)
        {
            return loadOrder.ToImmutableLinkCache<IOblivionMod, IOblivionModGetter>();
        }

        /// <summary>
        /// Creates a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static MutableLoadOrderLinkCache<IOblivionMod, IOblivionModGetter> ToMutableLinkCache(
            this LoadOrder<IOblivionModGetter> immutableBaseCache,
            params IOblivionMod[] mutableMods)
        {
            return immutableBaseCache.ToMutableLinkCache<IOblivionMod, IOblivionModGetter>(mutableMods);
        }

        /// <summary>
        /// Creates a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static MutableLoadOrderLinkCache<IOblivionMod, IOblivionModGetter> ToMutableLinkCache(
            this LoadOrder<IModListing<IOblivionModGetter>> immutableBaseCache,
            params IOblivionMod[] mutableMods)
        {
            return immutableBaseCache.ToMutableLinkCache<IOblivionMod, IOblivionModGetter>(mutableMods);
        }

        /// <summary>
        /// Creates a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static MutableLoadOrderLinkCache<IOblivionMod, IOblivionModGetter> ToMutableLinkCache(
            this IEnumerable<IOblivionModGetter> immutableBaseCache,
            params IOblivionMod[] mutableMods)
        {
            return immutableBaseCache.ToMutableLinkCache<IOblivionMod, IOblivionModGetter>(mutableMods);
        }

    }
}
