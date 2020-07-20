using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface for a FormLink
    /// </summary>
    public interface IFormLink : ILink
    {
        /// <summary>
        /// FormKey to link against
        /// </summary>
        FormKey FormKey { get; }

        /// <summary>
        /// True if FormKey points to a null ID
        /// </summary>
        bool IsNull { get; }
    }

    /// <summary>
    /// An interface for a FormLink, with a Major Record type constraint
    /// </summary>
    /// <typeparam name="TMajor">The type of Major Record the Link is allowed to connect with</typeparam>
    public interface IFormLink<out TMajor> : ILink<TMajor>, IFormLink
       where TMajor : IMajorRecordCommonGetter
    {
    }

    /// <summary>
    /// An interface for a FormLink.
    /// FormKey is allowed to be null to communicate absence of a record.
    /// </summary>
    public interface IFormLinkNullable : ILink
    {
        /// <summary>
        /// FormKey to link against
        /// </summary>
        FormKey? FormKey { get; }
    }

    /// <summary>
    /// An interface for a FormLink, with a Major Record type constraint 
    /// FormKey is allowed to be null to communicate absence of a record.
    /// </summary>
    /// <typeparam name="TMajor">The type of Major Record the Link is allowed to connect with</typeparam>
    public interface IFormLinkNullable<out TMajor> : ILink<TMajor>, IFormLinkNullable
       where TMajor : IMajorRecordCommonGetter
    {
    }

    /// <summary>
    /// A static class that contains extension functions for FormLinks
    /// </summary>
    public static class IFormLinkExt
    {
        /// <summary>
        /// Attempts to locate link target in given Link Cache.
        /// </summary>
        /// <param name="formLink">FormLink to resolve</param>
        /// <param name="package">Link Cache to resolve against</param>
        /// <param name="major">Located record if successful</param>
        /// <returns>True if link was resolved and a record was retrieved</returns>
        /// <typeparam name="TMajor">Major Record type to resolve to</typeparam>
        public static bool TryResolve<TMajor>(this IFormLink<TMajor> formLink, ILinkCache package, out TMajor major)
            where TMajor : IMajorRecordCommonGetter
        {
            major = formLink.Resolve(package);
            return major != null;
        }

        /// <summary>
        /// Attempts to locate link target in given Link Cache.
        /// </summary>
        /// <param name="formLink">FormLink to resolve</param>
        /// <param name="package">Link Cache to resolve against</param>
        /// <param name="major">Located record if successful</param>
        /// <returns>True if link was resolved and a record was retrieved</returns>
        /// <typeparam name="TMajor">Major Record type to resolve to</typeparam>
        public static bool TryResolve<TMajor>(this IFormLinkNullable<TMajor> formLink, ILinkCache package, out TMajor major)
            where TMajor : IMajorRecordCommonGetter
        {
            major = formLink.Resolve(package);
            return major != null;
        }
    }
}
