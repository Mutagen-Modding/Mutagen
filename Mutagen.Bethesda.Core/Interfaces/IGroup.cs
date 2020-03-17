using Loqui;
using Loqui.Xml;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface that Group Record objects implement to hook into the common systems
    /// </summary>
    public interface IGroupCommon<out TMajor>
        where TMajor : IMajorRecordCommonGetter, IBinaryItem
    {
        /// <summary>
        /// Mod object the Group belongs to
        /// </summary>
        IMod SourceMod { get; }

        /// <summary>
        /// A convenience accessor to iterate over all records in a group
        /// </summary>
        IEnumerable<TMajor> Records { get; }

        /// <summary>
        /// Number of contained records
        /// </summary>
        int Count { get; }
    }

    /// <summary>
    /// Class containing extension methods for groups
    /// </summary>
    public static class IGroupCommonExt
    {
        /// <summary>
        /// Convenience function to instantiate a new Major Record and add it to the Group.
        /// FormKey will be automatically assigned.
        /// </summary>
        /// <param name="group">Group to add record to</param>
        /// <returns>New record already added to the Group</returns>
        public static TMajor AddNew<TMajor>(this AGroup<TMajor> group)
            where TMajor : IMajorRecordInternal, IBinaryItem, IEquatable<TMajor>
        {
            var ret = MajorRecordInstantiator<TMajor>.Activator(group.SourceMod.GetNextFormKey());
            group.InternalCache.Set(ret);
            return ret;
        }
    }
}
