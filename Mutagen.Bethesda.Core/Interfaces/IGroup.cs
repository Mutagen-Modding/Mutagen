using Loqui;
using Loqui.Xml;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public interface IGroupCommon<out TMajor>
        where TMajor : IMajorRecordCommonGetter, IBinaryItem
    {
        IMod SourceMod { get; }

        /// <summary>
        /// A convenience accessor to iterate over all records in a group
        /// </summary>
        IEnumerable<TMajor> Records { get; }

        int Count { get; }
    }

    public static class IGroupCommonExt
    {
        public static TMajor AddNew<TMajor>(this AGroup<TMajor> group)
            where TMajor : IMajorRecordInternal, IBinaryItem, IEquatable<TMajor>
        {
            var ret = MajorRecordInstantiator<TMajor>.Activator(group.SourceMod.GetNextFormKey());
            group.InternalCache.Set(ret);
            return ret;
        }
    }
}
