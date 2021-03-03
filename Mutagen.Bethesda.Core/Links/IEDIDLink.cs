using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noggog;

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

    public interface IEDIDLink<TMajor> : IEDIDLinkGetter<TMajor>, IEDIDLink, IClearable
       where TMajor : IMajorRecordCommonGetter
    {
        void SetTo(RecordType type);
        void SetTo(IEDIDLinkGetter<TMajor> rhsLink);
    }

    /// <summary>
    /// A static class that contains extension functions for EDIDLinks
    /// </summary>
    public static class IEDIDLinkExt
    {
    }
}
