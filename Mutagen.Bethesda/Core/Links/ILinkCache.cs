using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface for retriving records given a FormKey.
    /// </summary>
    /// <typeparam name="TMod">Modtype records are being retrieved from</typeparam>
    public interface ILinkCache<TMod> : IEnumerable<TMod>
        where TMod : IModGetter
    {
        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the package was attached to.
        /// 
        /// NOTE:  This call is much slower than the alternative that uses generics, as all records in the entire mod must be
        /// processed, rather than being able to scope the search to a specific area.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <param name="majorRec">Out parameter containing the record if successful</param>
        /// <returns>True if a matching record was found</returns>
        bool TryGetMajorRecord(FormKey formKey, out IMajorRecordCommonGetter majorRec);

        /// <summary>
        /// Retrieves the record that matches the FormKey relative to the source the package was attached to.
        /// 
        /// If a record exists that matches the FormKey, but does not inherit from the given generic, it will not be returned, and 
        /// the function will return false.
        /// </summary>
        /// <param name="formKey">FormKey to look for</param>
        /// <param name="majorRec">Out parameter containing the record if successful</param>
        /// <returns>True if a matching record was found</returns>
        bool TryGetMajorRecord<TMajor>(FormKey formKey, out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter;
    }

    public static class ILinkCacheExt
    {
        /// <summary>
        /// Creates a new linking package relative to a mod.
        /// </summary>
        /// <typeparam name="TMod">Mod type</typeparam>
        /// <param name="mod">Mod to construct the package relative to</param>
        /// <returns>LinkPackage attached to given mod</returns>
        public static DirectModLinkCache<TMod> CreateLinkCache<TMod>(this TMod mod)
            where TMod : IModGetter
        {
            return new DirectModLinkCache<TMod>(mod);
        }

        /// <summary>
        /// Creates a new linking package relative to a modlist.
        /// </summary>
        /// <typeparam name="TMod">Mod type</typeparam>
        /// <param name="modList">ModList to construct the package relative to</param>
        /// <returns>LinkPackage attached to given ModList</returns>
        public static ILinkCache<TMod> CreateLinkCache<TMod>(this ModList<TMod> modList)
            where TMod : IModGetter
        {
            return new ModListLinkCache<TMod>(modList);
        }
    }
}
