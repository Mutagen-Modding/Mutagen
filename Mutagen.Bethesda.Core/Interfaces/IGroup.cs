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
    public interface IGroupCommonGetter<out TMajor>
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
}
