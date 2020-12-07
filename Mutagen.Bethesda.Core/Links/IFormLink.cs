using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface for a FormLink, with a Major Record type constraint
    /// </summary>
    /// <typeparam name="TMajorGetter">The type of Major Record the Link is allowed to connect with</typeparam>
    public interface IFormLink<out TMajorGetter> : ILink<TMajorGetter>, IFormLink, IFormLinkNullable<TMajorGetter>
       where TMajorGetter : IMajorRecordCommonGetter
    {
    }

    /// <summary>
    /// An interface for a FormLink.
    /// FormKey is allowed to be null to communicate absence of a record.
    /// </summary>
    public interface IFormLink : ILink
    {
        /// <summary>
        /// FormKey to link against
        /// </summary>
        FormKey? FormKeyNullable { get; }

        /// <summary>
        /// True if FormKey points to a null ID
        /// </summary>
        bool IsNull { get; }

        /// <summary>
        /// FormKey to link against
        /// </summary>
        FormKey FormKey { get; }
    }

    /// <summary>
    /// An interface for a FormLink, with a Major Record type constraint 
    /// FormKey is allowed to be null to communicate absence of a record.
    /// </summary>
    /// <typeparam name="TMajor">The type of Major Record the Link is allowed to connect with</typeparam>
    public interface IFormLinkNullable<out TMajor> : ILink<TMajor>, IFormLink
       where TMajor : IMajorRecordCommonGetter
    {
    }

    /// <summary>
    /// A static class that contains extension functions for FormLinks
    /// </summary>
    public static class IFormLinkExt
    {
        #region Resolve
        /// <summary>
        /// Attempts to locate link's winning target record in given Link Cache. 
        /// </summary>
        /// <param name="link">Link to resolve</param>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <param name="majorRecord">Major Record if located</param>
        /// <returns>True if successful in linking to record</returns>
        /// <typeparam name="TMajor">Major Record type to resolve to</typeparam>
        public static bool TryResolve<TMajor>(this IFormLink<TMajor> link, ILinkCache cache, [MaybeNullWhen(false)] out TMajor majorRecord)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (!link.FormKeyNullable.TryGet(out var formKey))
            {
                majorRecord = default;
                return false;
            }
            return cache.TryResolve<TMajor>(formKey, out majorRecord);
        }

        /// <summary> 
        /// Locates link winning target record in given Link Cache. 
        /// </summary> 
        /// <param name="link">Link to resolve</param> 
        /// <param name="cache">Link Cache to resolve against</param> 
        /// <returns>Located Major Record</returns> 
        /// <exception cref="NullReferenceException">If link was not succesful</exception> 
        /// <typeparam name="TMajor">Major Record type of the FormLink</typeparam> 
        /// <typeparam name="TScopedMajor">Major Record type to resolve to</typeparam> 
        public static TScopedMajor? Resolve<TMajor, TScopedMajor>(this IFormLink<TMajor> link, ILinkCache cache)
            where TMajor : class, IMajorRecordCommonGetter
            where TScopedMajor : class, TMajor
        {
            if (link.TryResolve<TMajor, TScopedMajor>(cache, out var majorRecord))
            {
                return majorRecord;
            }
            return null;
        }

        /// <summary>
        /// Attempts to locate link winning target record in given Link Cache.
        /// </summary>
        /// <param name="link">FormLink to resolve</param>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <param name="majorRecord">Located record if successful</param>
        /// <returns>True if link was resolved and a record was retrieved</returns>
        /// <typeparam name="TSource">Major Record type that the FormLink specifies explicitly</typeparam>
        /// <typeparam name="TScopedMajor">Inheriting Major Record type to scope to</typeparam>
        public static bool TryResolve<TSource, TScopedMajor>(this IFormLink<TSource> link, ILinkCache cache, [MaybeNullWhen(false)] out TScopedMajor majorRecord)
            where TSource : class, IMajorRecordCommonGetter
            where TScopedMajor : class, TSource
        {
            if (!link.FormKeyNullable.TryGet(out var formKey))
            {
                majorRecord = default;
                return false;
            }
            return cache.TryResolve(formKey, out majorRecord);
        }

        /// <summary>
        /// Locates link winning target record in given Link Cache.
        /// </summary>
        /// <param name="link">Link to resolve</param>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>Located Major Record</returns>
        /// <exception cref="NullReferenceException">If link was not succesful</exception>
        /// <typeparam name="TMajor">Major Record type to resolve to</typeparam>
        public static TMajor? Resolve<TMajor>(this IFormLink<TMajor> link, ILinkCache cache)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (link.FormKeyNullable == null)
            {
                return null;
            }
            if (link.TryResolve<TMajor>(cache, out var majorRecord))
            {
                return majorRecord;
            }
            return null;
        }
        #endregion

        #region ResolveAll
        /// <summary>
        /// Locate all of a link's target records in given Link Cache.<br /> 
        /// The winning override will be returned first, and finished by the original defining definition.
        /// </summary>
        /// <param name="link">Link to resolve</param>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>Enumerable of the linked records</returns>
        /// <typeparam name="TMajor">Major Record type to resolve to</typeparam>
        public static IEnumerable<TMajor> ResolveAll<TMajor>(this IFormLink<TMajor> link, ILinkCache cache)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (!link.FormKeyNullable.TryGet(out var formKey))
            {
                return Enumerable.Empty<TMajor>();
            }
            return cache.ResolveAll<TMajor>(formKey);
        }

        /// <summary>
        /// Locate all of a link's target records in given Link Cache.<br /> 
        /// The winning override will be returned first, and finished by the original defining definition.
        /// </summary>
        /// <param name="link">FormLink to resolve</param>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>Enumerable of the linked records</returns>
        /// <typeparam name="TSource">Major Record type that the FormLink specifies explicitly</typeparam>
        /// <typeparam name="TScopedMajor">Inheriting Major Record type to scope to</typeparam>
        public static IEnumerable<TScopedMajor> ResolveAll<TSource, TScopedMajor>(this IFormLink<TSource> link, ILinkCache cache)
            where TSource : class, IMajorRecordCommonGetter
            where TScopedMajor : class, TSource
        {
            if (!link.FormKeyNullable.TryGet(out var formKey))
            {
                return Enumerable.Empty<TScopedMajor>();
            }
            return cache.ResolveAll<TScopedMajor>(formKey);
        }
        #endregion

        #region Resolve Context
        /// <summary>
        /// Attempts to locate link's winning target record in given Link Cache. 
        /// </summary>
        /// <param name="link">Link to resolve</param>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <param name="majorRecord">Major Record if located</param>
        /// <returns>True if successful in linking to record</returns>
        /// <typeparam name="TMod">Mod setter type that can be overridden into</typeparam>
        /// <typeparam name="TMajorSetter">Major Record setter type to resolve to</typeparam>
        /// <typeparam name="TMajorGetter">Major Record getter type to resolve to</typeparam>
        public static bool TryResolveContext<TMod, TMajorSetter, TMajorGetter>(
            this IFormLink<TMajorGetter> link,
            ILinkCache<TMod> cache,
            [MaybeNullWhen(false)] out IModContext<TMod, TMajorSetter, TMajorGetter> majorRecord)
            where TMod : class, IContextMod<TMod>
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (!link.FormKeyNullable.TryGet(out var formKey))
            {
                majorRecord = default;
                return false;
            }
            return cache.TryResolveContext<TMajorSetter, TMajorGetter>(formKey, out majorRecord);
        }

        /// <summary>
        /// Locates link winning target record in given Link Cache.
        /// </summary>
        /// <param name="link">Link to resolve</param>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>Located Major Record</returns>
        /// <exception cref="NullReferenceException">If link was not succesful</exception>
        /// <typeparam name="TMod">Mod setter type that can be overridden into</typeparam>
        /// <typeparam name="TMajorSetter">Major Record setter type to resolve to</typeparam>
        /// <typeparam name="TMajorGetter">Major Record getter type to resolve to</typeparam>
        public static IModContext<TMod, TMajorSetter, TMajorGetter>? ResolveContext<TMod, TMajorSetter, TMajorGetter>(
            this IFormLink<TMajorGetter> link,
            ILinkCache<TMod> cache)
            where TMod : class, IContextMod<TMod>
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (link.TryResolveContext<TMod, TMajorSetter, TMajorGetter>(cache, out var majorRecord))
            {
                return majorRecord;
            }
            return null;
        }

        /// <summary>
        /// Attempts to winning locate link target record in given Link Cache.
        /// </summary>
        /// <param name="link">FormLink to resolve</param>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <param name="majorRecord">Located record if successful</param>
        /// <returns>True if link was resolved and a record was retrieved</returns>
        /// <typeparam name="TMod">Mod setter type that can be overridden into</typeparam>
        /// <typeparam name="TMajorGetter">Original links Major Record type</typeparam>
        /// <typeparam name="TScopedSetter">Inheriting Major Record setter type to scope to</typeparam>
        /// <typeparam name="TScopedGetter">Inheriting Major Record getter type to scope to</typeparam>
        public static bool TryResolveContext<TMod, TMajorGetter, TScopedSetter, TScopedGetter>(
            this IFormLink<TMajorGetter> link,
            ILinkCache<TMod> cache,
            [MaybeNullWhen(false)] out IModContext<TMod, TScopedSetter, TScopedGetter> majorRecord)
            where TMod : class, IContextMod<TMod>
            where TMajorGetter : class, IMajorRecordCommonGetter
            where TScopedSetter : class, TScopedGetter, IMajorRecordCommon
            where TScopedGetter : class, TMajorGetter
        {
            if (!link.FormKeyNullable.TryGet(out var formKey))
            {
                majorRecord = default;
                return false;
            }
            return cache.TryResolveContext<TScopedSetter, TScopedGetter>(formKey, out majorRecord);
        }

        /// <summary> 
        /// Locates link winning target record in given Link Cache. 
        /// </summary> 
        /// <param name="link">Link to resolve</param> 
        /// <param name="cache">Link Cache to resolve against</param> 
        /// <returns>Located Major Record</returns> 
        /// <exception cref="NullReferenceException">If link was not succesful</exception>
        /// <typeparam name="TMod">Mod setter type that can be overridden into</typeparam>
        /// <typeparam name="TMajorGetter">Original links Major Record type</typeparam>
        /// <typeparam name="TScopedSetter">Inheriting Major Record setter type to scope to</typeparam>
        /// <typeparam name="TScopedGetter">Inheriting Major Record getter type to scope to</typeparam>
        public static IModContext<TMod, TScopedSetter, TScopedGetter>? ResolveContext<TMod, TMajorGetter, TScopedSetter, TScopedGetter>(
            this IFormLink<TMajorGetter> link,
            ILinkCache<TMod> cache)
            where TMod : class, IContextMod<TMod>
            where TMajorGetter : class, IMajorRecordCommonGetter
            where TScopedSetter : class, TScopedGetter, IMajorRecordCommon
            where TScopedGetter : class, TMajorGetter
        {
            if (link.TryResolveContext<TMod, TMajorGetter, TScopedSetter, TScopedGetter>(cache, out var majorRecord))
            {
                return majorRecord;
            }
            return null;
        }
        #endregion

        #region ResolveAll Context
        /// <summary>
        /// Locate all of a link's target record contexts in given Link Cache.<br /> 
        /// The winning override will be returned first, and finished by the original defining definition.
        /// </summary>
        /// <param name="link">Link to resolve</param>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>Enumerable of the linked records</returns>
        /// <typeparam name="TMod">Mod setter type that can be overridden into</typeparam>
        /// <typeparam name="TMajorSetter">Major Record setter type to resolve to</typeparam>
        /// <typeparam name="TMajorGetter">Major Record getter type to resolve to</typeparam>
        public static IEnumerable<IModContext<TMod, TMajorSetter, TMajorGetter>> ResolveAllContexts<TMod, TMajorSetter, TMajorGetter>(this IFormLink<TMajorGetter> link, ILinkCache<TMod> cache)
            where TMod : class, IContextMod<TMod>
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (!link.FormKeyNullable.TryGet(out var formKey))
            {
                return Enumerable.Empty<IModContext<TMod, TMajorSetter, TMajorGetter>>();
            }
            return cache.ResolveAllContexts<TMajorSetter, TMajorGetter>(formKey);
        }

        /// <summary>
        /// Locate all of a link's target record contexts in given Link Cache.<br /> 
        /// The winning override will be returned first, and finished by the original defining definition.
        /// </summary>
        /// <param name="link">FormLink to resolve</param>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>Enumerable of the linked records</returns>
        /// <typeparam name="TMod">Mod setter type that can be overridden into</typeparam>
        /// <typeparam name="TMajorGetter">Original links Major Record type</typeparam>
        /// <typeparam name="TScopedSetter">Inheriting Major Record setter type to scope to</typeparam>
        /// <typeparam name="TScopedGetter">Inheriting Major Record getter type to scope to</typeparam>
        public static IEnumerable<IModContext<TMod, TScopedSetter, TScopedGetter>> ResolveAllContexts<TMod, TMajorGetter, TScopedSetter, TScopedGetter>(this IFormLink<TMajorGetter> link, ILinkCache<TMod> cache)
            where TMod : class, IContextMod<TMod>
            where TMajorGetter : class, IMajorRecordCommonGetter
            where TScopedSetter : class, TScopedGetter, IMajorRecordCommon
            where TScopedGetter : class, TMajorGetter
        {
            if (!link.FormKeyNullable.TryGet(out var formKey))
            {
                return Enumerable.Empty<IModContext<TMod, TScopedSetter, TScopedGetter>>();
            }
            return cache.ResolveAllContexts<TScopedSetter, TScopedGetter>(formKey);
        }
        #endregion
    }
}
