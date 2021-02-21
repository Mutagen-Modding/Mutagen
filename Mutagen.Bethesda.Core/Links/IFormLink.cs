using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Noggog;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface for a FormLink.
    /// FormKey is allowed to be null to communicate absence of a record.
    /// </summary>
    public interface IFormLinkGetter : ILink
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
    /// </summary>
    /// <typeparam name="TMajorGetter">The type of Major Record the Link is allowed to connect with</typeparam>
    public interface IFormLinkGetter<out TMajorGetter> : ILink<TMajorGetter>, IFormLinkGetter
       where TMajorGetter : IMajorRecordCommonGetter
    {
    }

    public interface IFormLink<out TMajorGetter> : IFormLinkGetter<TMajorGetter>, IClearable
       where TMajorGetter : IMajorRecordCommonGetter
    {
        /// <summary>
        /// FormKey to link against
        /// </summary>
        new FormKey? FormKeyNullable { get; set; }

        /// <summary>
        /// FormKey to link against
        /// </summary>
        new FormKey FormKey { get; set; }

        void SetTo(FormKey? formKey);
    }

    /// <summary>
    /// An interface for a FormLink, with a Major Record type constraint 
    /// FormKey is allowed to be null to communicate absence of a record.
    /// </summary>
    /// <typeparam name="TMajorGetter">The type of Major Record the Link is allowed to connect with</typeparam>
    public interface IFormLinkNullableGetter<out TMajorGetter> : ILink<TMajorGetter>, IFormLinkGetter, IFormLinkGetter<TMajorGetter>
       where TMajorGetter : IMajorRecordCommonGetter
    {
    }

    public interface IFormLinkNullable<out TMajorGetter> : IFormLink<TMajorGetter>, IFormLinkNullableGetter<TMajorGetter>
       where TMajorGetter : IMajorRecordCommonGetter
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
        public static bool TryResolve<TMajor>(this IFormLinkGetter<TMajor> link, ILinkCache cache, [MaybeNullWhen(false)] out TMajor majorRecord)
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
        /// Attempts to locate link winning target record in given Link Cache.
        /// </summary>
        /// <param name="link">FormLink to resolve</param>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <param name="majorRecord">Located record if successful</param>
        /// <returns>True if link was resolved and a record was retrieved</returns>
        /// <typeparam name="TSource">Major Record type that the FormLink specifies explicitly</typeparam>
        /// <typeparam name="TScopedMajor">Inheriting Major Record type to scope to</typeparam>
        public static bool TryResolve<TSource, TScopedMajor>(this IFormLinkGetter<TSource> link, ILinkCache cache, [MaybeNullWhen(false)] out TScopedMajor majorRecord)
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
        /// Attempts to locate link winning target record in given Link Cache.
        /// </summary>
        /// <param name="link">FormLink to resolve</param>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <param name="majorRecord">Located record if successful</param>
        /// <returns>True if link was resolved and a record was retrieved</returns>
        /// <typeparam name="TMajor">Major Record type to resolve to</typeparam>
        public static bool TryResolve<TMajor>(this IFormLinkGetter link, ILinkCache cache, [MaybeNullWhen(false)] out TMajor majorRecord)
            where TMajor : class, IMajorRecordCommonGetter
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
        public static TMajor? Resolve<TMajor>(this IFormLinkGetter<TMajor> link, ILinkCache cache)
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

        /// <summary> 
        /// Locates link winning target record in given Link Cache. 
        /// </summary> 
        /// <param name="link">Link to resolve</param> 
        /// <param name="cache">Link Cache to resolve against</param> 
        /// <returns>Located Major Record, or null if not located</returns> 
        /// <typeparam name="TMajor">Major Record type of the FormLink</typeparam> 
        /// <typeparam name="TScopedMajor">Major Record type to resolve to</typeparam> 
        public static TScopedMajor? Resolve<TMajor, TScopedMajor>(this IFormLinkGetter<TMajor> link, ILinkCache cache)
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
        /// Locates link winning target record in given Link Cache. 
        /// </summary> 
        /// <param name="link">Link to resolve</param> 
        /// <param name="cache">Link Cache to resolve against</param> 
        /// <returns>Located Major Record, or null if not located</returns> 
        /// <typeparam name="TMajor">Major Record type of the FormLink</typeparam> 
        public static TMajor? Resolve<TMajor>(this IFormLinkGetter link, ILinkCache cache)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (link.TryResolve<TMajor>(cache, out var majorRecord))
            {
                return majorRecord;
            }
            return null;
        }

        /// <summary> 
        /// Locates link winning target record in given Link Cache. 
        /// </summary> 
        /// <param name="link">Link to resolve</param> 
        /// <param name="cache">Link Cache to resolve against</param> 
        /// <returns>Located Major Record</returns> 
        /// <exception cref="RecordException">If link was not succesful</exception> 
        /// <typeparam name="TMajor">Major Record type of the FormLink</typeparam> 
        /// <typeparam name="TScopedMajor">Major Record type to resolve to</typeparam> 
        public static TScopedMajor ResolveOrThrow<TMajor, TScopedMajor>(this IFormLinkGetter<TMajor> link, ILinkCache cache)
            where TMajor : class, IMajorRecordCommonGetter
            where TScopedMajor : class, TMajor
        {
            if (link.TryResolve<TMajor, TScopedMajor>(cache, out var majorRecord))
            {
                return majorRecord;
            }
            throw new RecordException(link.FormKeyNullable, link.FormKeyNullable?.ModKey, edid: null, "Could not resolve record");
        }

        /// <summary>
        /// Locates link winning target record in given Link Cache.
        /// </summary>
        /// <param name="link">Link to resolve</param>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>Located Major Record</returns>
        /// <exception cref="RecordException">If link was not succesful</exception> 
        /// <typeparam name="TMajor">Major Record type to resolve to</typeparam>
        public static TMajor ResolveOrThrow<TMajor>(this IFormLinkGetter<TMajor> link, ILinkCache cache)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (link.FormKeyNullable == null)
            {
                throw new RecordException(null, null, null, "Could not resolve record");
            }
            if (link.TryResolve<TMajor>(cache, out var majorRecord))
            {
                return majorRecord;
            }
            throw new RecordException(link.FormKeyNullable, link.FormKeyNullable?.ModKey, edid: null, "Could not resolve record");
        }

        /// <summary> 
        /// Locates link winning target record in given Link Cache. 
        /// </summary> 
        /// <param name="link">Link to resolve</param> 
        /// <param name="cache">Link Cache to resolve against</param> 
        /// <returns>Located Major Record</returns> 
        /// <exception cref="RecordException">If link was not succesful</exception> 
        /// <typeparam name="TMajor">Major Record type of the FormLink</typeparam> 
        public static TMajor ResolveOrThrow<TMajor>(this IFormLinkGetter link, ILinkCache cache)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (link.TryResolve<TMajor>(cache, out var majorRecord))
            {
                return majorRecord;
            }
            throw new RecordException(link.FormKeyNullable, link.FormKeyNullable?.ModKey, edid: null, "Could not resolve record");
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
        public static IEnumerable<TMajor> ResolveAll<TMajor>(this IFormLinkGetter<TMajor> link, ILinkCache cache)
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
        public static IEnumerable<TScopedMajor> ResolveAll<TSource, TScopedMajor>(this IFormLinkGetter<TSource> link, ILinkCache cache)
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
        /// <typeparam name="TModGetter">Mod getter type that can be overridden into</typeparam>
        /// <typeparam name="TMajor">Major Record setter type to resolve to</typeparam>
        /// <typeparam name="TMajorGetter">Major Record getter type to resolve to</typeparam>
        public static bool TryResolveContext<TMod, TModGetter, TMajor, TMajorGetter>(
            this IFormLinkGetter<TMajorGetter> link,
            ILinkCache<TMod, TModGetter> cache,
            [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRecord)
            where TModGetter : class, IModGetter
            where TMod : class, TModGetter, IContextMod<TMod, TModGetter>
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (!link.FormKeyNullable.TryGet(out var formKey))
            {
                majorRecord = default;
                return false;
            }
            return cache.TryResolveContext<TMajor, TMajorGetter>(formKey, out majorRecord);
        }

        /// <summary>
        /// Locates link winning target record in given Link Cache.
        /// </summary>
        /// <param name="link">Link to resolve</param>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>Located Major Record</returns>
        /// <exception cref="NullReferenceException">If link was not succesful</exception>
        /// <typeparam name="TMod">Mod setter type that can be overridden into</typeparam>
        /// <typeparam name="TModGetter">Mod getter type that can be overridden into</typeparam>
        /// <typeparam name="TMajor">Major Record setter type to resolve to</typeparam>
        /// <typeparam name="TMajorGetter">Major Record getter type to resolve to</typeparam>
        public static IModContext<TMod, TModGetter, TMajor, TMajorGetter>? ResolveContext<TMod, TModGetter, TMajor, TMajorGetter>(
            this IFormLinkGetter<TMajorGetter> link,
            ILinkCache<TMod, TModGetter> cache)
            where TModGetter : class, IModGetter
            where TMod : class, TModGetter, IContextMod<TMod, TModGetter>
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (link.TryResolveContext<TMod, TModGetter, TMajor, TMajorGetter>(cache, out var majorRecord))
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
        /// <typeparam name="TModGetter">Mod getter type that can be overridden into</typeparam>
        /// <typeparam name="TMajorGetter">Original links Major Record type</typeparam>
        /// <typeparam name="TScopedSetter">Inheriting Major Record setter type to scope to</typeparam>
        /// <typeparam name="TScopedGetter">Inheriting Major Record getter type to scope to</typeparam>
        public static bool TryResolveContext<TMod, TModGetter, TMajorGetter, TScopedSetter, TScopedGetter>(
            this IFormLinkGetter<TMajorGetter> link,
            ILinkCache<TMod, TModGetter> cache,
            [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TScopedSetter, TScopedGetter> majorRecord)
            where TModGetter : class, IModGetter
            where TMod : class, TModGetter, IContextMod<TMod, TModGetter>
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
        /// <typeparam name="TModGetter">Mod getter type that can be overridden into</typeparam>
        /// <typeparam name="TMajorGetter">Original links Major Record type</typeparam>
        /// <typeparam name="TScopedSetter">Inheriting Major Record setter type to scope to</typeparam>
        /// <typeparam name="TScopedGetter">Inheriting Major Record getter type to scope to</typeparam>
        public static IModContext<TMod, TModGetter, TScopedSetter, TScopedGetter>? ResolveContext<TMod, TModGetter, TMajorGetter, TScopedSetter, TScopedGetter>(
            this IFormLinkGetter<TMajorGetter> link,
            ILinkCache<TMod, TModGetter> cache)
            where TModGetter : class, IModGetter
            where TMod : class, TModGetter, IContextMod<TMod, TModGetter>
            where TMajorGetter : class, IMajorRecordCommonGetter
            where TScopedSetter : class, TScopedGetter, IMajorRecordCommon
            where TScopedGetter : class, TMajorGetter
        {
            if (link.TryResolveContext<TMod, TModGetter, TMajorGetter, TScopedSetter, TScopedGetter>(cache, out var majorRecord))
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
        /// <typeparam name="TModGetter">Mod getter type that can be overridden into</typeparam>
        /// <typeparam name="TMajor">Major Record setter type to resolve to</typeparam>
        /// <typeparam name="TMajorGetter">Major Record getter type to resolve to</typeparam>
        public static IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>> ResolveAllContexts<TMod, TModGetter, TMajor, TMajorGetter>(
            this IFormLinkGetter<TMajorGetter> link,
            ILinkCache<TMod, TModGetter> cache)
            where TModGetter : class, IModGetter
            where TMod : class, TModGetter, IContextMod<TMod, TModGetter>
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (!link.FormKeyNullable.TryGet(out var formKey))
            {
                return Enumerable.Empty<IModContext<TMod, TModGetter, TMajor, TMajorGetter>>();
            }
            return cache.ResolveAllContexts<TMajor, TMajorGetter>(formKey);
        }

        /// <summary>
        /// Locate all of a link's target record contexts in given Link Cache.<br /> 
        /// The winning override will be returned first, and finished by the original defining definition.
        /// </summary>
        /// <param name="link">FormLink to resolve</param>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>Enumerable of the linked records</returns>
        /// <typeparam name="TMod">Mod setter type that can be overridden into</typeparam>
        /// <typeparam name="TModGetter">Mod getter type that can be overridden into</typeparam>
        /// <typeparam name="TMajorGetter">Original links Major Record type</typeparam>
        /// <typeparam name="TScopedSetter">Inheriting Major Record setter type to scope to</typeparam>
        /// <typeparam name="TScopedGetter">Inheriting Major Record getter type to scope to</typeparam>
        public static IEnumerable<IModContext<TMod, TModGetter, TScopedSetter, TScopedGetter>> ResolveAllContexts<TMod, TModGetter, TMajorGetter, TScopedSetter, TScopedGetter>(
            this IFormLinkGetter<TMajorGetter> link, 
            ILinkCache<TMod, TModGetter> cache)
            where TModGetter : class, IModGetter
            where TMod : class, TModGetter, IContextMod<TMod, TModGetter>
            where TMajorGetter : class, IMajorRecordCommonGetter
            where TScopedSetter : class, TScopedGetter, IMajorRecordCommon
            where TScopedGetter : class, TMajorGetter
        {
            if (!link.FormKeyNullable.TryGet(out var formKey))
            {
                return Enumerable.Empty<IModContext<TMod, TModGetter, TScopedSetter, TScopedGetter>>();
            }
            return cache.ResolveAllContexts<TScopedSetter, TScopedGetter>(formKey);
        }
        #endregion

        internal static bool EqualsWithInheritanceConsideration<TMajorGetter>(IFormLinkGetter<TMajorGetter> link, object? obj)
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (obj is IFormLinkGetter<TMajorGetter> rhs)
            {
                return link.FormKey == rhs.FormKey;
            }
            else if (obj is IFormLinkGetter rhsLink
                && rhsLink.TargetType.IsAssignableFrom(typeof(TMajorGetter)))
            {
                return link.FormKey == rhsLink.FormKey;
            }
            else
            {
                return false;
            }
        }

        public static void SetTo<TMajorLhs, TMajorRhs>(this IFormLink<TMajorLhs> link, TMajorRhs? record)
            where TMajorLhs : IMajorRecordCommonGetter
            where TMajorRhs : TMajorLhs
        {
            link.SetTo(record?.FormKey);
        }

        public static void SetTo<TMajor, TMajorGetter>(this IFormLink<TMajor> link, IFormLinkGetter<TMajorGetter> rhs)
            where TMajor : IMajorRecordCommonGetter
            where TMajorGetter : TMajor
        {
            link.SetTo(rhs.FormKeyNullable);
        }

        public static void SetToGetter<TMajor, TMajorGetter>(this IFormLink<TMajor> link, IFormLinkGetter<TMajorGetter> rhs)
            where TMajor : IMapsToGetter<TMajorGetter>
            where TMajorGetter : IMajorRecordCommonGetter
        {
            link.SetTo(rhs.FormKeyNullable);
        }
    }
}
