using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// A FormKey with an associated Major Record Type that it is allowed to link to.
    /// This provides type safety concepts on top of a basic FormKey.
    /// FormKey allowed to be null to communicate the absence of the field.
    /// </summary>
    /// <typeparam name="TMajorGetter">The type of Major Record the Link is allowed to connect with</typeparam>
    public struct FormLinkNullable<TMajorGetter> :
        IFormLink<TMajorGetter>,
        IEquatable<FormLink<TMajorGetter>>,
        IEquatable<FormLinkNullable<TMajorGetter>>,
        IEquatable<IFormLink<TMajorGetter>>,
        IEquatable<IFormLinkNullable<TMajorGetter>>
        where TMajorGetter : class, IMajorRecordCommonGetter
    {
        /// <summary>
        /// A readonly singleton representing an unlinked and null FormLinkNullable
        /// </summary>
        public static readonly FormLinkNullable<TMajorGetter> Null = new FormLinkNullable<TMajorGetter>();

        /// <summary>
        /// FormKey of the target record.
        /// </summary>
        public FormKey? FormKeyNullable { get; }

        /// <summary>
        /// Non null FormKey of the target record.  If null, it will instead return FormKey.Null.
        /// </summary>
        public FormKey FormKey => FormKeyNullable ?? FormKey.Null;
        
        Type ILink.TargetType => typeof(TMajorGetter);

        /// <summary>
        /// True if unlinked and ID points to Null
        /// </summary>
        public bool IsNull => this.FormKeyNullable?.IsNull ?? true;

        /// <summary>
        /// Default constructor that creates a link to the target FormKey
        /// </summary>
        public FormLinkNullable(FormKey? formKey)
        {
            this.FormKeyNullable = formKey;
        }

        public static bool operator ==(FormLinkNullable<TMajorGetter> lhs, FormLink<TMajorGetter> rhs)
        {
            return lhs.FormKeyNullable?.Equals(rhs.FormKey) ?? false;
        }

        public static bool operator !=(FormLinkNullable<TMajorGetter> lhs, FormLink<TMajorGetter> rhs)
        {
            return !lhs.FormKeyNullable?.Equals(rhs.FormKey) ?? true;
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
            if (obj is not IFormLink<TMajorGetter> rhs) return false;
            return this.Equals(rhs);
        }

        /// <summary>
        /// Compares equality of two links, where rhs is a non nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(FormLink<TMajorGetter> other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKeyNullable, other.FormKey);

        /// <summary>
        /// Compares equality of two links, where rhs is a nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(FormLinkNullable<TMajorGetter> other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKeyNullable, other.FormKeyNullable);

        /// <summary>
        /// Compares equality of two links, where rhs is a non nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(IFormLink<TMajorGetter>? other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKeyNullable, other?.FormKey);

        /// <summary>
        /// Compares equality of two links, where rhs is a nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(IFormLinkNullable<TMajorGetter>? other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKeyNullable, other?.FormKeyNullable);

        /// <summary>
        /// Returns hash code
        /// </summary>
        /// <returns>Hash code evaluated from FormKey member</returns>
        public override int GetHashCode() => this.FormKeyNullable?.GetHashCode() ?? 0;

        /// <summary>
        /// Returns string representation of link
        /// </summary>
        /// <returns>Returns FormKey string</returns>
        public override string ToString() => this.FormKeyNullable?.ToString() ?? "Null";

        /// <summary>
        /// Attempts to locate link target in given Link Cache.
        /// </summary>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <param name="major">Located record if successful</param>
        /// <returns>True if link was resolved and a record was retrieved</returns>
        public bool TryResolve(ILinkCache cache, [MaybeNullWhen(false)] out TMajorGetter major)
        {
            if (this.FormKeyNullable == null
                || this.FormKeyNullable.Equals(Mutagen.Bethesda.FormKey.Null))
            {
                major = default!;
                return false;
            }
            if (cache.TryResolve<TMajorGetter>(this.FormKeyNullable.Value, out var majorRec))
            {
                major = majorRec;
                return true;
            }
            major = default!;
            return false;
        }

        /// <summary>
        /// Attempts to locate an associated FormKey from the link
        /// </summary>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <param name="formKey">FormKey if found</param>
        /// <returns>True if FormKey is not null</returns>
        public bool TryResolveFormKey(ILinkCache cache, [MaybeNullWhen(false)] out FormKey formKey)
        {
            if (this.FormKeyNullable == null)
            {
                formKey = default!;
                return false;
            }
            formKey = this.FormKeyNullable.Value;
            return true;
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
            if (this.FormKeyNullable?.Equals(Mutagen.Bethesda.FormKey.Null) ?? true)
            {
                major = default!;
                return false;
            }
            if (cache.TryResolve<TScopedMajor>(this.FormKeyNullable.Value, out var majorRec))
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
        /// <typeparam name="TMajorSetter">Major Record setter type to resolve to</typeparam>
        public bool TryResolveContext<TMod, TMajorSetter>(
            ILinkCache<TMod> cache,
            [MaybeNullWhen(false)] out IModContext<TMod, TMajorSetter, TMajorGetter> majorRecord)
            where TMod : class, IContextMod<TMod>
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
        {
            if (!this.FormKeyNullable.TryGet(out var formKey))
            {
                majorRecord = default;
                return false;
            }
            return cache.TryResolveContext<TMajorSetter, TMajorGetter>(formKey, out majorRecord);
        }

        /// <summary>
        /// Locates link target record in given Link Cache.
        /// </summary>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>Located Major Record</returns>
        /// <exception cref="NullReferenceException">If link was not succesful</exception>
        /// <typeparam name="TMod">Mod setter type that can be overridden into</typeparam>
        /// <typeparam name="TMajorSetter">Major Record setter type to resolve to</typeparam>
        public IModContext<TMod, TMajorSetter, TMajorGetter>? ResolveContext<TMod, TMajorSetter>(ILinkCache<TMod> cache)
            where TMod : class, IContextMod<TMod>
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
        {
            if (this.TryResolveContext<TMod, TMajorSetter, TMajorGetter>(cache, out var majorRecord))
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
        /// <typeparam name="TScopedSetter">Inheriting Major Record setter type to scope to</typeparam>
        /// <typeparam name="TScopedGetter">Inheriting Major Record getter type to scope to</typeparam>
        public bool TryResolveContext<TMod, TScopedSetter, TScopedGetter>(
            ILinkCache<TMod> cache,
            [MaybeNullWhen(false)] out IModContext<TMod, TScopedSetter, TScopedGetter> majorRecord)
            where TMod : class, IContextMod<TMod>
            where TScopedSetter : class, TScopedGetter, IMajorRecordCommon
            where TScopedGetter : class, TMajorGetter
        {
            if (!this.FormKeyNullable.TryGet(out var formKey))
            {
                majorRecord = default;
                return false;
            }
            return cache.TryResolveContext<TScopedSetter, TScopedGetter>(formKey, out majorRecord);
        }

        /// <summary> 
        /// Locates link target record in given Link Cache. 
        /// </summary> 
        /// <param name="cache">Link Cache to resolve against</param> 
        /// <returns>Located Major Record</returns> 
        /// <exception cref="NullReferenceException">If link was not succesful</exception> 
        /// <typeparam name="TMod">Mod setter type that can be overridden into</typeparam>
        /// <typeparam name="TScopedSetter">Inheriting Major Record setter type to scope to</typeparam>
        /// <typeparam name="TScopedGetter">Inheriting Major Record getter type to scope to</typeparam>
        public IModContext<TMod, TScopedSetter, TScopedGetter>? ResolveContext<TMod, TScopedSetter, TScopedGetter>(ILinkCache<TMod> cache)
            where TMod : class, IContextMod<TMod>
            where TScopedSetter : class, TScopedGetter, IMajorRecordCommon
            where TScopedGetter : class, TMajorGetter
        {
            if (this.TryResolveContext<TMod, TMajorGetter, TScopedSetter, TScopedGetter>(cache, out var majorRecord))
            {
                return majorRecord;
            }
            return null;
        }

        /// <summary>
        /// Attempts to locate an associated ModKey from the link
        /// </summary>
        /// <param name="modKey">ModKey if found</param>
        /// <returns>True if FormKey is not null</returns>
        public bool TryGetModKey([MaybeNullWhen(false)] out ModKey modKey)
        {
            if (this.FormKeyNullable.TryGet(out var formKey))
            {
                modKey = formKey.ModKey;
                return true;
            }
            modKey = default!;
            return false;
        }

        public static implicit operator FormLinkNullable<TMajorGetter>(TMajorGetter? major)
        {
            return new FormLinkNullable<TMajorGetter>(major?.FormKey);
        }

        public static implicit operator FormLinkNullable<TMajorGetter>(FormKey? formKey)
        {
            return new FormLinkNullable<TMajorGetter>(formKey);
        }

        public static implicit operator FormLinkNullable<TMajorGetter>(FormKey formKey)
        {
            return new FormLinkNullable<TMajorGetter>(formKey);
        }
    }
}
