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
    public interface IEDIDLinkGetter : ILink
    {
        /// <summary>
        /// Record type representing the target EditorID to link against
        /// </summary>
        RecordType EDID { get; }
    }

    public interface IEDIDLink : IEDIDLinkGetter
    {
        /// <summary>
        /// Record type representing the target EditorID to link against
        /// </summary>
        new RecordType EDID { get; set; }
    }

    /// <summary>
    /// An interface for a EDID Link, with a Major Record type constraint
    /// </summary>
    /// <typeparam name="TMajor">The type of Major Record the Link is allowed to connect with</typeparam>
    public interface IEDIDLinkGetter<out TMajor> : ILink<TMajor>, IEDIDLinkGetter
       where TMajor : IMajorRecordCommonGetter
    {
    }

    public interface IEDIDLink<TMajor> : IEDIDLinkGetter<TMajor>, IEDIDLink
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
