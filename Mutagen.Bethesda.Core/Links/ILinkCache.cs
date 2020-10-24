using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface for retriving records given a FormKey.
    /// </summary>
    public interface ILinkCache
    {
        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the package was attached to.<br/>
        /// <br/>
        /// NOTE:  This call is much slower than the alternative that uses generics, as all records in the entire mod must be
        /// processed, rather than being able to scope the search to a specific area.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <param name="majorRec">Out parameter containing the record if successful</param>
        /// <returns>True if a matching record was found</returns>
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        bool TryLookup(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec);

        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from the given generic, it will not be returned, and 
        /// the function will return false.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <param name="majorRec">Out parameter containing the record if successful</param>
        /// <returns>True if a matching record was found</returns>
        /// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        bool TryLookup<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter;

        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from the given generic, it will not be returned, and 
        /// the function will return false.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <param name="type">The type of Major Record to look up</param>
        /// <param name="majorRec">Out parameter containing the record if successful</param>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>True if a matching record was found</returns>
        bool TryLookup(FormKey formKey, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec);

        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the cache was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from the given generic, it will not be returned, and 
        /// the function will throw a KeyNotFoundException.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// When the FormKey cannot be found under the attached cache.<br/>
        /// </exception>
        /// <returns>True if a matching record was found</returns>
        IMajorRecordCommonGetter Lookup(FormKey formKey);

        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the package was attached to.<br/>
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from the given type, it will be seen as not a match.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <param name="type">The type of Major Record to look up</param>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <returns>record if successful</returns>
        /// <exception cref="KeyNotFoundException">
        /// When the FormKey having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        IMajorRecordCommonGetter Lookup(FormKey formKey, Type type);

        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the package was attached to.
        /// <br/>
        /// If a record exists that matches the FormKey, but does not inherit from the given generic, it will be seen as not a match.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
        /// <returns>record if successful</returns>
        /// <exception cref="ArgumentException">
        /// An unexpected TMajor type will throw an exception.<br/>
        /// Unexpected types include:<br/>
        ///   - Major Record Types that are not part of this game type.  (Querying for Oblivion records on a Skyrim mod)<br/>
        ///   - A setter type is requested from a getter only object.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// When the FormKey having the specified Major Record type cannot be found under the attached cache.<br/>
        /// </exception>
        TMajor Lookup<TMajor>(FormKey formKey)
            where TMajor : class, IMajorRecordCommonGetter;

        /// <summary>
        /// Iterates through the contained mods in the order they were listed, with the least prioritized mod first.
        /// </summary>
        public IReadOnlyList<IModGetter> ListedOrder { get; }

        /// <summary>
        /// Iterates through the contained mods in priority order (reversed), with the most prioritized mod first.
        /// </summary>
        public IReadOnlyList<IModGetter> PriorityOrder { get; }
    }

    public static class ILinkCacheExt
    {
        /// <summary>
        /// Creates a Link Cache using a single mod as its link target. <br/>
        /// Modification of the target Mod is not safe.  Internal caches can become incorrect if 
        /// modifications occur on content already cached.
        /// </summary>
        /// <param name="mod">Mod to construct the package relative to</param>
        /// <returns>LinkPackage attached to given mod</returns>
        public static ImmutableModLinkCache<TMod> ToImmutableLinkCache<TMod>(this TMod mod)
            where TMod : class, IModGetter
        {
            return new ImmutableModLinkCache<TMod>(mod);
        }

        /// <summary>
        /// Creates a Link Cache using a single mod as its link target.  Mod is allowed to be modified afterwards, but
        /// this comes at a performance cost of not allowing much caching to be done.  If the mod is not expected to
        /// be modified afterwards, use ImmutableModLinkCache instead.<br/>
        /// </summary>
        /// <param name="mod">Mod to construct the package relative to</param>
        /// <returns>LinkPackage attached to given mod</returns>
        public static MutableModLinkCache<TMod> ToMutableLinkCache<TMod>(this TMod mod)
            where TMod : class, IModGetter
        {
            return new MutableModLinkCache<TMod>(mod);
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<TModGetter> ToImmutableLinkCache<TModGetter>(
            this LoadOrder<TModGetter> loadOrder)
            where TModGetter : class, IModGetter
        {
            return new ImmutableLoadOrderLinkCache<TModGetter>(loadOrder.ListedOrder);
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<TModGetter> ToImmutableLinkCache<TModGetter>(
            this LoadOrder<IModListing<TModGetter>> loadOrder)
            where TModGetter : class, IModGetter
        {
            return new ImmutableLoadOrderLinkCache<TModGetter>(
                loadOrder
                    .Select(listing => listing.Value.Mod)
                    .NotNull());
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.<br/>
        /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
        /// Modification of mods on the load order is not safe.  Internal caches can become
        /// incorrect if modifications occur on content already cached.
        /// </summary>
        /// <param name="loadOrder">Enumerable of mods to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static ImmutableLoadOrderLinkCache<TModGetter> ToImmutableLinkCache<TModGetter>(
            this IEnumerable<TModGetter> loadOrder)
            where TModGetter : class, IModGetter
        {
            return new ImmutableLoadOrderLinkCache<TModGetter>(loadOrder);
        }

        /// <summary>
        /// Creates a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static MutableLoadOrderLinkCache<TMod, TModGetter> ToMutableLinkCache<TMod, TModGetter>(
            this LoadOrder<TModGetter> immutableBaseCache,
            params TMod[] mutableMods)
            where TMod : class, IMod, TModGetter
            where TModGetter : class, IModGetter
        {
            return new MutableLoadOrderLinkCache<TMod, TModGetter>(
                immutableBaseCache.ToImmutableLinkCache(),
                mutableMods);
        }

        /// <summary>
        /// Creates a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static MutableLoadOrderLinkCache<TMod, TModGetter> ToMutableLinkCache<TMod, TModGetter>(
            this LoadOrder<IModListing<TModGetter>> immutableBaseCache,
            params TMod[] mutableMods)
            where TMod : class, IMod, TModGetter
            where TModGetter : class, IModGetter
        {
            return new MutableLoadOrderLinkCache<TMod, TModGetter>(
                immutableBaseCache.ToImmutableLinkCache(),
                mutableMods);
        }

        /// <summary>
        /// Creates a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static MutableLoadOrderLinkCache<TMod, TModGetter> ToMutableLinkCache<TMod, TModGetter>(
            this IEnumerable<TModGetter> immutableBaseCache,
            params TMod[] mutableMods)
            where TMod : class, IMod, TModGetter
            where TModGetter : class, IModGetter
        {
            return new MutableLoadOrderLinkCache<TMod, TModGetter>(
                immutableBaseCache.ToImmutableLinkCache(),
                mutableMods);
        }
    }
}
