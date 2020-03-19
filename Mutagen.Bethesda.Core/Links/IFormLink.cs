using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface for a read-only FormLink
    /// </summary>
    public interface IFormLinkGetter : ILinkGetter
    {
        /// <summary>
        /// FormKey to link against
        /// </summary>
        FormKey FormKey { get; }
    }

    /// <summary>
    /// An interface for a read-only FormLink, with a Major Record type constraint
    /// </summary>
    /// <typeparam name="TMajor">The type of Major Record the Link is allowed to connect with</typeparam>
    public interface IFormLinkGetter<out TMajor> : ILinkGetter<TMajor>, IFormLinkGetter
       where TMajor : IMajorRecordCommonGetter
    {
    }

    /// <summary>
    /// An interface for a FormLink
    /// </summary>
    public interface IFormLink : IFormLinkGetter
    {
        /// <summary>
        /// FormKey to link against
        /// </summary>
        new FormKey FormKey { get; set; }
    }

    /// <summary>
    /// An interface for a FormLink, with a Major Record type constraint
    /// </summary>
    /// <typeparam name="TMajor">The type of Major Record the Link is allowed to connect with</typeparam>
    public interface IFormLink<out TMajor> : IFormLinkGetter<TMajor>, IFormLink
       where TMajor : IMajorRecordCommonGetter
    {
    }

    /// <summary>
    /// An interface for a read-only FormLink.
    /// FormKey is allowed to be null to communicate absence of a record.
    /// </summary>
    public interface IFormLinkNullableGetter : ILinkGetter
    {
        /// <summary>
        /// FormKey to link against
        /// </summary>
        FormKey? FormKey { get; }
    }

    /// <summary>
    /// An interface for a read-only FormLink, with a Major Record type constraint 
    /// FormKey is allowed to be null to communicate absence of a record.
    /// </summary>
    /// <typeparam name="TMajor">The type of Major Record the Link is allowed to connect with</typeparam>
    public interface IFormLinkNullableGetter<out TMajor> : ILinkGetter<TMajor>, IFormLinkNullableGetter
       where TMajor : IMajorRecordCommonGetter
    {
    }

    /// <summary>
    /// An interface for a FormLink.
    /// FormKey is allowed to be null to communicate absence of a record.
    /// </summary>
    public interface IFormLinkNullable : IFormLinkNullableGetter
    {
        new FormKey? FormKey { get; set; }
    }

    /// <summary>
    /// An interface for a FormLink, with a Major Record type constraint 
    /// FormKey is allowed to be null to communicate absence of a record.
    /// </summary>
    /// <typeparam name="TMajor">The type of Major Record the Link is allowed to connect with</typeparam>
    public interface IFormLinkNullable<TMajor> : IFormLinkNullableGetter<TMajor>, IFormLinkNullable
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
        /// <typeparam name="TMod">Mod type</typeparam>
        /// <typeparam name="TMajor">Major Record type to resolve to</typeparam>
        public static bool TryResolve<TMajor, TMod>(this IFormLinkGetter<TMajor> formLink, ILinkCache<TMod> package, out TMajor item)
            where TMajor : IMajorRecordCommonGetter
            where TMod : IMod
        {
            item = formLink.Resolve(package);
            return item != null;
        }
        
        /// <summary>
        /// Attempts to locate link target in given Link Cache.
        /// </summary>
        /// <param name="formLink">FormLink to resolve</param>
        /// <param name="package">Link Cache to resolve against</param>
        /// <param name="major">Located record if successful</param>
        /// <returns>True if link was resolved and a record was retrieved</returns>
        /// <typeparam name="TMod">Mod type</typeparam>
        /// <typeparam name="TMajor">Major Record type to resolve to</typeparam>
        public static bool TryResolve<TMajor, TMod>(this IFormLinkNullableGetter<TMajor> formLink, ILinkCache<TMod> package, out TMajor item)
            where TMajor : IMajorRecordCommonGetter
            where TMod : IMod
        {
            item = formLink.Resolve(package);
            return item != null;
        }
    }
}
