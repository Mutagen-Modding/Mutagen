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
    public struct FormLink<TMajorGetter> : 
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
        public FormKey FormKey { get; }
        
        Type ILink.TargetType => typeof(TMajorGetter);

        /// <summary>
        /// True if unlinked and ID points to Null
        /// </summary>
        public bool IsNull => this.FormKey.IsNull;

        FormKey? IFormLink.FormKeyNullable => this.FormKey;

        /// <summary>
        /// Default constructor that creates a link to the target FormKey
        /// </summary>
        public FormLink(FormKey formKey)
        {
            this.FormKey = formKey;
        }

        /// <summary>
        /// Sets the link to the target FormKey
        /// </summary>
        /// <param name="formKey">Target FormKey to link to</param>
        public void Set(FormKey formKey)
        {
            this.Set(formKey);
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
            if (obj is not IFormLink<TMajorGetter> rhs) return false;
            return this.Equals(rhs);
        }

        /// <summary>
        /// Compares equality of two links.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(FormLink<TMajorGetter> other) => this.FormKey.Equals(other.FormKey);

        /// <summary>
        /// Compares equality of two links, where rhs is a nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(FormLinkNullable<TMajorGetter> other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKey, other.FormKeyNullable);

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
        public override string ToString() => this.FormKey.ToString();

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
}
