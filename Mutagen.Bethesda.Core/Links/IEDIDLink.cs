using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface for a read-only EDID Link
    /// </summary>
    public interface IEDIDLinkGetter : ILinkGetter
    {
        /// <summary>
        /// Record type representing the target EditorID to link against
        /// </summary>
        RecordType EDID { get; }
    }

    /// <summary>
    /// An interface for a read-only EDID Link, with a Major Record type constraint
    /// </summary>
    /// <typeparam name="TMajor">The type of Major Record the Link is allowed to connect with</typeparam>
    public interface IEDIDLinkGetter<out TMajor> : ILinkGetter<TMajor>, IEDIDLinkGetter
       where TMajor : IMajorRecordCommonGetter
    {
    }

    /// <summary>
    /// An interface for a EDID Link, with a Major Record type constraint
    /// </summary>
    /// <typeparam name="TMajor">The type of Major Record the Link is allowed to connect with</typeparam>
    public interface IEDIDLink<TMajor> : IEDIDLinkGetter<TMajor>, IEDIDLinkGetter
       where TMajor : IMajorRecordCommonGetter
    {
        /// <summary>
        /// Record type representing the target EditorID to link against
        /// </summary>
        new RecordType EDID { get; set; }
    }

    /// <summary>
    /// A static class that contains extension functions for EDIDLinks
    /// </summary>
    public static class IEDIDLinkExt
    {
        /// <summary>
        /// Attempts to locate link target in given Link Cache.
        /// </summary>
        /// <param name="edidLink">EDIDLink to resolve</param>
        /// <param name="package">Link Cache to resolve against</param>
        /// <param name="major">Located record if successful</param>
        /// <returns>True if link was resolved and a record was retrieved</returns>
        /// <typeparam name="TMajor">Major Record type to resolve to</typeparam>
        public static bool TryResolve<TMajor>(this IEDIDLinkGetter<TMajor> edidLink, ILinkCache package, out TMajor major)
            where TMajor : IMajorRecordCommonGetter
        {
            major = edidLink.Resolve(package);
            return major != null;
        }
    }
}
