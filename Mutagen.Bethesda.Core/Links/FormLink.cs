using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// A FormKey with an associated Major Record Type that it is allowed to link to.
    /// This provides type safety concepts on top of a basic FormKey.
    /// </summary>
    /// <typeparam name="TMajorGetter">The type of Major Record the Link is allowed to connect with</typeparam>
    public class FormLink<TMajorGetter> : 
        IFormLink<TMajorGetter>,
        IEquatable<FormLink<TMajorGetter>>,
        IEquatable<FormLinkNullable<TMajorGetter>>,
        IEquatable<IFormLink<TMajorGetter>>,
        IEquatable<IFormLinkNullable<TMajorGetter>>
        where TMajorGetter : class, IMajorRecordCommonGetter
    {
        /// <summary>
        /// A readonly singleton representing an unlinked FormLink
        /// </summary>
        public static readonly FormLink<TMajorGetter> Null = new FormLink<TMajorGetter>();

        /// <summary>
        /// FormKey of the target record
        /// </summary>
        public FormKey FormKey { get; set; }
        
        Type ILink.TargetType => typeof(TMajorGetter);

        /// <summary>
        /// True if unlinked and ID points to Null
        /// </summary>
        public bool IsNull => this.FormKey.IsNull;

        FormKey? IFormLink.FormKeyNullable => this.FormKey;

        public FormLink()
        {
            this.FormKey = FormKey.Null;
        }

        /// <summary>
        /// Default constructor that creates a link to the target FormKey
        /// </summary>
        public FormLink(FormKey formKey)
        {
            this.FormKey = formKey;
        }

        /// <summary>
        /// Default constructor that creates a link to the target FormKey
        /// </summary>
        public FormLink(TMajorGetter record)
        {
            this.FormKey = record.FormKey;
        }

        /// <summary>
        /// Sets the link to the target FormKey
        /// </summary>
        /// <param name="formKey">Target FormKey to link to</param>
        public void Set(FormKey formKey)
        {
            FormKey = formKey;
        }

        /// <summary>
        /// Sets the link to the target Record
        /// </summary>
        /// <param name="record">Target record to link to</param>
        public void Set(TMajorGetter record)
        {
            FormKey = record.FormKey;
        }

        public void Clear()
        {
            this.FormKey = FormKey.Null;
        }

        public static bool operator ==(FormLink<TMajorGetter> lhs, FormLink<TMajorGetter> rhs)
        {
            return lhs.FormKey.Equals(rhs.FormKey);
        }

        public static bool operator !=(FormLink<TMajorGetter> lhs, FormLink<TMajorGetter> rhs)
        {
            return !lhs.FormKey.Equals(rhs.FormKey);
        }

        public static bool operator ==(FormLink<TMajorGetter> lhs, FormLinkNullable<TMajorGetter> rhs)
        {
            return EqualityComparer<FormKey?>.Default.Equals(lhs.FormKey, rhs.FormKeyNullable);
        }

        public static bool operator !=(FormLink<TMajorGetter> lhs, FormLinkNullable<TMajorGetter> rhs)
        {
            return !EqualityComparer<FormKey?>.Default.Equals(lhs.FormKey, rhs.FormKeyNullable);
        }

        /// <summary>
        /// Default Equality
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>True if object is ILinkGetter and FormKeys match</returns>
        public override bool Equals(object? obj)
        {
            return IFormLinkExt.EqualsWithInheritanceConsideration(this, obj);
        }

        /// <summary>
        /// Compares equality of two links.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(FormLink<TMajorGetter>? other) => this.FormKey.Equals(other?.FormKey ?? FormKey.Null);

        /// <summary>
        /// Compares equality of two links, where rhs is a nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(FormLinkNullable<TMajorGetter>? other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKey, other?.FormKeyNullable);

        /// <summary>
        /// Compares equality of two links.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(IFormLink<TMajorGetter>? other) => this.FormKey.Equals(other?.FormKey);

        /// <summary>
        /// Compares equality of two links, where rhs is a nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(IFormLinkNullable<TMajorGetter>? other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKey, other?.FormKeyNullable);

        /// <summary>
        /// Returns hash code
        /// </summary>
        /// <returns>Hash code evaluated from FormKey member</returns>
        public override int GetHashCode() => this.FormKey.GetHashCode();

        /// <summary>
        /// Returns string representation of link
        /// </summary>
        /// <returns>Returns FormKey string</returns>
        public override string ToString() => $"<{MajorRecordTypePrinter<TMajorGetter>.TypeString}>{this.FormKey}";

        /// <summary>
        /// Attempts to locate link target in given Link Cache.
        /// </summary>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <param name="major">Located record if successful</param>
        /// <returns>True if link was resolved and a record was retrieved</returns>
        public bool TryResolve(ILinkCache cache, [MaybeNullWhen(false)] out TMajorGetter major)
        {
            return TryResolve<TMajorGetter>(cache, out major);
        }

        /// <summary>
        /// Attempts to locate link target in given Link Cache.
        /// </summary>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <param name="major">Located record if successful</param>
        /// <returns>True if link was resolved and a record was retrieved</returns>
        /// <typeparam name="TScopedMajor">Major Record type to resolve to</typeparam>
        public bool TryResolve<TScopedMajor>(ILinkCache cache, [MaybeNullWhen(false)] out TScopedMajor major)
            where TScopedMajor : class, TMajorGetter
        {
            if (this.FormKey.Equals(FormKey.Null))
            {
                major = default!;
                return false;
            }
            if (cache.TryResolve<TScopedMajor>(this.FormKey, out var majorRec))
            {
                major = majorRec;
                return true;
            }
            major = default!;
            return false;
        }

        /// <summary>
        /// Attempts to locate link target in given Link Cache.
        /// </summary>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>TLocated record if successful, or null</returns>
        public TMajorGetter? Resolve(ILinkCache cache)
        {
            if (TryResolve(cache, out var major))
            {
                return major;
            }
            return default;
        }

        /// <summary>
        /// Attempts to locate link target in given Link Cache.
        /// </summary>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>TLocated record if successful, or null</returns>
        /// <typeparam name="TScopedMajor">Major Record type to resolve to</typeparam>
        public TMajorGetter? Resolve<TScopedMajor>(ILinkCache cache)
            where TScopedMajor : class, TMajorGetter
        {
            if (TryResolve<TScopedMajor>(cache, out var major))
            {
                return major;
            }
            return default;
        }

        bool ILink.TryResolveFormKey(ILinkCache cache, [MaybeNullWhen(false)] out FormKey formKey)
        {
            formKey = this.FormKey;
            return true;
        }

        bool ILink.TryResolveCommon(ILinkCache cache, [MaybeNullWhen(false)] out IMajorRecordCommonGetter formKey)
        {
            if (TryResolve(cache, out var rec))
            {
                formKey = rec;
                return true;
            }
            formKey = default!;
            return false;
        }

        /// <summary>
        /// Attempts to locate link target in given Link Cache.
        /// </summary>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>TryGet object with located record if successful</returns>
        public ITryGetter<TMajorGetter> TryResolve(ILinkCache cache)
        {
            if (TryResolve(cache, out var rec))
            {
                return TryGet<TMajorGetter>.Succeed(rec);
            }
            return TryGet<TMajorGetter>.Failure;
        }

        /// <summary>
        /// Attempts to locate link's target record in given Link Cache. 
        /// </summary>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <param name="majorRecord">Major Record if located</param>
        /// <returns>True if successful in linking to record</returns>
        /// <typeparam name="TMod">Mod setter type that can be overridden into</typeparam>
        /// <typeparam name="TModGetter">Mod getter type that can be overridden into</typeparam>
        /// <typeparam name="TMajor">Major Record setter type to resolve to</typeparam>
        public bool TryResolveContext<TMod, TModGetter, TMajor>(
            ILinkCache<TMod, TModGetter> cache,
            [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRecord)
            where TModGetter : class, IModGetter
            where TMod : class, TModGetter, IContextMod<TMod, TModGetter>
            where TMajor : class, IMajorRecordCommon, TMajorGetter
        {
            return cache.TryResolveContext<TMajor, TMajorGetter>(this.FormKey, out majorRecord);
        }

        /// <summary>
        /// Locates link target record in given Link Cache.
        /// </summary>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>Located Major Record</returns>
        /// <exception cref="NullReferenceException">If link was not succesful</exception>
        /// <typeparam name="TMod">Mod setter type that can be overridden into</typeparam>
        /// <typeparam name="TModGetter">Mod getter type that can be overridden into</typeparam>
        /// <typeparam name="TMajor">Major Record setter type to resolve to</typeparam>
        public IModContext<TMod, TModGetter, TMajor, TMajorGetter>? ResolveContext<TMod, TModGetter, TMajor>(ILinkCache<TMod, TModGetter> cache)
            where TModGetter : class, IModGetter
            where TMod : class, TModGetter, IContextMod<TMod, TModGetter>
            where TMajor : class, IMajorRecordCommon, TMajorGetter
        {
            if (this.TryResolveContext<TMod, TModGetter, TMajor, TMajorGetter>(cache, out var majorRecord))
            {
                return majorRecord;
            }
            return null;
        }

        /// <summary>
        /// Attempts to locate link target record in given Link Cache.
        /// </summary>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <param name="majorRecord">Located record if successful</param>
        /// <returns>True if link was resolved and a record was retrieved</returns>
        /// <typeparam name="TMod">Mod setter type that can be overridden into</typeparam>
        /// <typeparam name="TModGetter">Mod getter type that can be overridden into</typeparam>
        /// <typeparam name="TScopedSetter">Inheriting Major Record setter type to scope to</typeparam>
        /// <typeparam name="TScopedGetter">Inheriting Major Record getter type to scope to</typeparam>
        public bool TryResolveContext<TMod, TModGetter, TScopedSetter, TScopedGetter>(
            ILinkCache<TMod, TModGetter> cache,
            [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TScopedSetter, TScopedGetter> majorRecord)
            where TModGetter : class, IModGetter
            where TMod : class, TModGetter, IContextMod<TMod, TModGetter>
            where TScopedSetter : class, TScopedGetter, IMajorRecordCommon
            where TScopedGetter : class, TMajorGetter
        {
            return cache.TryResolveContext<TScopedSetter, TScopedGetter>(this.FormKey, out majorRecord);
        }

        /// <summary>
        /// Locates link target record in given Link Cache.
        /// </summary>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>Located Major Record</returns>
        /// <exception cref="NullReferenceException">If link was not succesful</exception>
        /// <typeparam name="TMod">Mod setter type that can be overridden into</typeparam>
        /// <typeparam name="TModGetter">Mod getter type that can be overridden into</typeparam>
        /// <typeparam name="TScopedSetter">Inheriting Major Record setter type to scope to</typeparam>
        /// <typeparam name="TScopedGetter">Inheriting Major Record getter type to scope to</typeparam>
        public IModContext<TMod, TModGetter, TScopedSetter, TScopedGetter>? ResolveContext<TMod, TModGetter, TScopedSetter, TScopedGetter>(ILinkCache<TMod, TModGetter> cache)
            where TModGetter : class, IModGetter
            where TMod : class, TModGetter, IContextMod<TMod, TModGetter>
            where TScopedSetter : class, TScopedGetter, IMajorRecordCommon
            where TScopedGetter : class, TMajorGetter
        {
            if (this.TryResolveContext<TMod, TModGetter, TMajorGetter, TScopedSetter, TScopedGetter>(cache, out var majorRecord))
            {
                return majorRecord;
            }
            return null;
        }

        /// <summary>
        /// Locate all of a link's target records in given Link Cache.<br /> 
        /// The winning override will be returned first, and finished by the original defining definition.
        /// </summary>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>Enumerable of the linked records</returns>
        public IEnumerable<TMajorGetter> ResolveAll(ILinkCache cache)
        {
            return cache.ResolveAll<TMajorGetter>(this.FormKey);
        }

        /// <summary>
        /// Locate all of a link's target records in given Link Cache.<br /> 
        /// The winning override will be returned first, and finished by the original defining definition.
        /// </summary>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>Enumerable of the linked records</returns>
        /// <typeparam name="TScopedMajor">Inheriting Major Record type to scope to</typeparam>
        public IEnumerable<TScopedMajor> ResolveAll<TScopedMajor>(ILinkCache cache)
            where TScopedMajor : class, TMajorGetter
        {
            return cache.ResolveAll<TScopedMajor>(this.FormKey);
        }

        /// <summary>
        /// Locate all of a link's target record contexts in given Link Cache.<br /> 
        /// The winning override will be returned first, and finished by the original defining definition.
        /// </summary>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>Enumerable of the linked records</returns>
        public IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>> ResolveAllContexts<TMod, TModGetter, TMajor>(ILinkCache<TMod, TModGetter> cache)
            where TModGetter : class, IModGetter
            where TMod : class, TModGetter, IContextMod<TMod, TModGetter>
            where TMajor : class, IMajorRecordCommon, TMajorGetter
        {
            return cache.ResolveAllContexts<TMajor, TMajorGetter>(this.FormKey);
        }

        /// <summary>
        /// Locate all of a link's target record contexts in given Link Cache.<br /> 
        /// The winning override will be returned first, and finished by the original defining definition.
        /// </summary>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>Enumerable of the linked records</returns>
        /// <typeparam name="TMod">Mod setter type that can be overridden into</typeparam>
        /// <typeparam name="TModGetter">Mod getter type that can be overridden into</typeparam>
        /// <typeparam name="TScopedSetter">Inheriting Major Record setter type to scope to</typeparam>
        /// <typeparam name="TScopedGetter">Inheriting Major Record getter type to scope to</typeparam>
        public IEnumerable<IModContext<TMod, TModGetter, TScopedSetter, TScopedGetter>> ResolveAllContexts<TMod, TModGetter, TScopedSetter, TScopedGetter>(ILinkCache<TMod, TModGetter> cache)
            where TModGetter : class, IModGetter
            where TMod : class, TModGetter, IContextMod<TMod, TModGetter>
            where TScopedSetter : class, TScopedGetter, IMajorRecordCommon
            where TScopedGetter : class, TMajorGetter
        {
            return cache.ResolveAllContexts<TScopedSetter, TScopedGetter>(this.FormKey);
        }

        bool ILink.TryGetModKey([MaybeNullWhen(false)] out ModKey modKey)
        {
            modKey = this.FormKey.ModKey;
            return true;
        }

        public static implicit operator FormLink<TMajorGetter>(TMajorGetter major)
        {
            return new FormLink<TMajorGetter>(major.FormKey);
        }

        public static implicit operator FormLink<TMajorGetter>(FormKey formKey)
        {
            return new FormLink<TMajorGetter>(formKey);
        }
    }

    public struct FormLink<TMajor, TMajorGetter>
        where TMajor : class, IMajorRecordCommon, TMajorGetter
        where TMajorGetter : class, IMajorRecordCommonGetter
    {
        /// <summary>
        /// FormKey of the target record
        /// </summary>
        public FormKey FormKey { get; private set; }

        /// <summary>
        /// Default constructor that creates a link to the target FormKey
        /// </summary>
        public FormLink(FormKey formKey)
        {
            this.FormKey = formKey;
        }

        public static implicit operator FormLink<TMajor, TMajorGetter>(FormLink<TMajor> link)
        {
            return new FormLink<TMajor, TMajorGetter>(link.FormKey);
        }

        public static implicit operator FormLink<TMajor, TMajorGetter>(FormLink<TMajorGetter> link)
        {
            return new FormLink<TMajor, TMajorGetter>(link.FormKey);
        }

        public static implicit operator FormLink<TMajorGetter>(FormLink<TMajor, TMajorGetter> link)
        {
            return new FormLink<TMajorGetter>(link.FormKey);
        }

        public static FormLink<TMajor, TMajorGetter> Factory(FormLink<TMajorGetter> link)
        {
            return new FormLink<TMajor, TMajorGetter>(link.FormKey);
        }
    }
}
