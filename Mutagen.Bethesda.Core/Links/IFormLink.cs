using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface for a FormLink
    /// </summary>
    public interface IFormLink : IFormLinkNullable
    {
        /// <summary>
        /// FormKey to link against
        /// </summary>
        new FormKey FormKey { get; }
    }

    /// <summary>
    /// An interface for a FormLink, with a Major Record type constraint
    /// </summary>
    /// <typeparam name="TMajor">The type of Major Record the Link is allowed to connect with</typeparam>
    public interface IFormLink<out TMajor> : ILink<TMajor>, IFormLink, IFormLinkNullable<TMajor>
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

        /// <summary>
        /// True if FormKey points to a null ID
        /// </summary>
        bool IsNull { get; }
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
        /// <param name="link">Link to resolve</param>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <param name="majorRecord">Major Record if located</param>
        /// <returns>True if successful in linking to record</returns>
        /// <typeparam name="TMajor">Major Record type to resolve to</typeparam>
        public static bool TryResolve<TMajor>(this IFormLink<TMajor> link, ILinkCache cache, [MaybeNullWhen(false)] out TMajor majorRecord)
            where TMajor : class, IMajorRecordCommonGetter
        {
            return cache.TryResolve<TMajor>(link.FormKey, out majorRecord);
        }

        /// <summary>
        /// Locates link target in given Link Cache.
        /// </summary>
        /// <param name="link">Link to resolve</param>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>Located Major Record</returns>
        /// <exception cref="NullReferenceException">If link was not succesful</exception>
        /// <typeparam name="TMajor">Major Record type to resolve to</typeparam>
        public static TMajor? Resolve<TMajor>(this IFormLink<TMajor> link, ILinkCache cache)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (link.TryResolve<TMajor>(cache, out var majorRecord))
            {
                return majorRecord;
            }
            return null;
        }

        /// <summary>
        /// Attempts to locate link target in given Link Cache.
        /// </summary>
        /// <param name="link">Link to resolve</param>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <param name="majorRecord">Major Record if located</param>
        /// <returns>True if successful in linking to record</returns>
        /// <typeparam name="TMajor">Major Record type to resolve to</typeparam>
        public static bool TryResolve<TMajor>(this IFormLinkNullable<TMajor> link, ILinkCache cache, [MaybeNullWhen(false)] out TMajor majorRecord)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (link.FormKey == null)
            {
                majorRecord = default;
                return false;
            }
            return cache.TryResolve<TMajor>(link.FormKey.Value, out majorRecord);
        }

        /// <summary>
        /// Locates link target in given Link Cache.
        /// </summary>
        /// <param name="link">Link to resolve</param>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>Located Major Record</returns>
        /// <exception cref="NullReferenceException">If link was not succesful</exception>
        /// <typeparam name="TMajor">Major Record type to resolve to</typeparam>
        public static TMajor? Resolve<TMajor>(this IFormLinkNullable<TMajor> link, ILinkCache cache)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (link.FormKey == null)
            {
                return null;
            }
            if (link.TryResolve<TMajor>(cache, out var majorRecord))
            {
                return majorRecord;
            }
            return null;
        }

        /// <summary>
        /// Attempts to locate link target in given Link Cache.
        /// </summary>
        /// <param name="formLink">FormLink to resolve</param>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <param name="major">Located record if successful</param>
        /// <returns>True if link was resolved and a record was retrieved</returns>
        /// <typeparam name="TSource">Major Record type that the FormLink specifies explicitly</typeparam>
        /// <typeparam name="TTarget">Inheriting Major Record type to scope to</typeparam>
        public static bool TryResolve<TSource, TTarget>(this IFormLink<TSource> formLink, ILinkCache cache, [MaybeNullWhen(false)] out TTarget major)
            where TSource : class, IMajorRecordCommonGetter
            where TTarget : class, TSource
        {
            if (formLink.FormKey.Equals(FormKey.Null))
            {
                major = default;
                return false;
            }
            return cache.TryResolve(formLink.FormKey, out major);
        }

        /// <summary>
        /// Attempts to locate link target in given Link Cache.
        /// </summary>
        /// <param name="formLink">FormLink to resolve</param>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <param name="major">Located record if successful</param>
        /// <returns>True if link was resolved and a record was retrieved</returns>
        /// <typeparam name="TSource">Major Record type that the FormLink specifies explicitly</typeparam>
        /// <typeparam name="TTarget">Inheriting Major Record type to scope to</typeparam>
        public static bool TryResolve<TSource, TTarget>(this IFormLinkNullable<TSource> formLink, ILinkCache cache, [MaybeNullWhen(false)] out TTarget major)
            where TSource : class, IMajorRecordCommonGetter
            where TTarget : class, TSource
        {
            if (formLink.FormKey == null
                || formLink.FormKey.Equals(Mutagen.Bethesda.FormKey.Null))
            {
                major = default!;
                return false;
            }
            return cache.TryResolve(formLink.FormKey.Value, out major);
        }
    }
}
