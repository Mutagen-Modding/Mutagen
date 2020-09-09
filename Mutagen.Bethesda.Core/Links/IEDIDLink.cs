using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface for a EDID Link
    /// </summary>
    public interface IEDIDLink : ILink
    {
        /// <summary>
        /// Record type representing the target EditorID to link against
        /// </summary>
        RecordType EDID { get; }
    }

    /// <summary>
    /// An interface for a EDID Link, with a Major Record type constraint
    /// </summary>
    /// <typeparam name="TMajor">The type of Major Record the Link is allowed to connect with</typeparam>
    public interface IEDIDLink<out TMajor> : ILink<TMajor>, IEDIDLink
       where TMajor : IMajorRecordCommonGetter
    {
    }

    /// <summary>
    /// A static class that contains extension functions for EDIDLinks
    /// </summary>
    public static class IEDIDLinkExt
    {
    }
}
