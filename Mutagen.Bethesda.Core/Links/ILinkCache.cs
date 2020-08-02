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

        public IReadOnlyList<IModGetter> ListedOrder { get; }

        public IReadOnlyList<IModGetter> PriorityOrder { get; }
    }

    public static class ILinkCacheExt
    {
        /// <summary>
        /// Creates a new linking package relative to a mod.
        /// </summary>
        /// <typeparam name="TMod">Mod type</typeparam>
        /// <param name="mod">Mod to construct the package relative to</param>
        /// <returns>LinkPackage attached to given mod</returns>
        public static DirectModLinkCache<TMod> ToLinkCache<TMod>(this TMod mod)
            where TMod : class, IModGetter
        {
            return new DirectModLinkCache<TMod>(mod);
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.
        /// </summary>
        /// <typeparam name="TMod">Mod type</typeparam>
        /// <param name="loadOrder">LoadOrder to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static LoadOrderLinkCache<TMod> ToLinkCache<TMod>(
            this LoadOrder<TMod> loadOrder)
            where TMod : class, IModGetter
        {
            return new LoadOrderLinkCache<TMod>(
                loadOrder
                    .Select(listing => listing.Mod)
                    .NotNull());
        }

        /// <summary>
        /// Creates a new linking package relative to a load order.
        /// </summary>
        /// <typeparam name="TMod">Mod type</typeparam>
        /// <param name="loadOrder">Enumerable of mods to construct the package relative to</param>
        /// <returns>LinkPackage attached to given LoadOrder</returns>
        public static LoadOrderLinkCache<TMod> ToLinkCache<TMod>(
            this IEnumerable<TMod> loadOrder)
            where TMod : class, IModGetter
        {
            return new LoadOrderLinkCache<TMod>(loadOrder);
        }
    }
}
