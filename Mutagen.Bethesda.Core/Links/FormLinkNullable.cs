using Loqui;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// A FormKey with an associated Major Record Type that it is allowed to link to.
    /// This provides type safety concepts on top of a basic FormKey.
    /// FormKey allowed to be null to communicate the absence of the field.
    /// </summary>
    /// <typeparam name="TMajor">The type of Major Record the Link is allowed to connect with</typeparam>
    public struct FormLinkNullable<TMajor> : 
        IFormLinkNullable<TMajor>,
        IEquatable<FormLink<TMajor>>,
        IEquatable<FormLinkNullable<TMajor>>,
        IEquatable<IFormLink<TMajor>>,
        IEquatable<IFormLinkNullable<TMajor>>
       where TMajor : class, IMajorRecordCommonGetter
    {
        /// <summary>
        /// A readonly singleton representing an unlinked and null FormLinkNullable
        /// </summary>
        public static readonly FormLinkNullable<TMajor> Null = new FormLinkNullable<TMajor>();

        /// <summary>
        /// FormKey of the target record.
        /// </summary>
        public FormKey? FormKeyNullable { get; }
        
        Type ILink.TargetType => typeof(TMajor);

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

        public static bool operator ==(FormLinkNullable<TMajor> lhs, FormLink<TMajor> rhs)
        {
            return lhs.FormKeyNullable?.Equals(rhs.FormKey) ?? false;
        }

        public static bool operator !=(FormLinkNullable<TMajor> lhs, FormLink<TMajor> rhs)
        {
            return !lhs.FormKeyNullable?.Equals(rhs.FormKey) ?? true;
        }

        public static bool operator ==(FormLink<TMajor> lhs, FormLinkNullable<TMajor> rhs)
        {
            return EqualityComparer<FormKey?>.Default.Equals(lhs.FormKey, rhs.FormKeyNullable);
        }

        public static bool operator !=(FormLink<TMajor> lhs, FormLinkNullable<TMajor> rhs)
        {
            return !EqualityComparer<FormKey?>.Default.Equals(lhs.FormKey, rhs.FormKeyNullable);
        }

        /// <summary>
        /// Default Equality
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>True if object is ILinkGetter and FormKeys match</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is IFormLink<TMajor> rhs)) return false;
            return this.Equals(rhs);
        }

        /// <summary>
        /// Compares equality of two links, where rhs is a non nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(FormLink<TMajor> other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKeyNullable, other.FormKey);

        /// <summary>
        /// Compares equality of two links, where rhs is a nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(FormLinkNullable<TMajor> other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKeyNullable, other.FormKeyNullable);

        /// <summary>
        /// Compares equality of two links, where rhs is a non nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(IFormLink<TMajor> other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKeyNullable, other.FormKey);

        /// <summary>
        /// Compares equality of two links, where rhs is a nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(IFormLinkNullable<TMajor> other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKeyNullable, other.FormKeyNullable);

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
        public bool TryResolve(ILinkCache cache, [MaybeNullWhen(false)] out TMajor major)
        {
            if (this.FormKeyNullable == null
                || this.FormKeyNullable.Equals(Mutagen.Bethesda.FormKey.Null))
            {
                major = default!;
                return false;
            }
            if (cache.TryResolve<TMajor>(this.FormKeyNullable.Value, out var majorRec))
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
            where TScopedMajor : class, TMajor
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
        public TMajor? Resolve(ILinkCache cache)
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
        public TMajor? Resolve<TScopedMajor>(ILinkCache cache)
            where TScopedMajor : class, TMajor
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
        public ITryGetter<TMajor> TryResolve(ILinkCache cache)
        {
            if (TryResolve(cache, out var rec))
            {
                return TryGet<TMajor>.Succeed(rec);
            }
            return TryGet<TMajor>.Failure;
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

        public static implicit operator FormLinkNullable<TMajor>(TMajor? major)
        {
            return new FormLinkNullable<TMajor>(major?.FormKey);
        }

        public static implicit operator FormLinkNullable<TMajor>(FormKey? formKey)
        {
            return new FormLinkNullable<TMajor>(formKey);
        }

        public static implicit operator FormLinkNullable<TMajor>(FormKey formKey)
        {
            return new FormLinkNullable<TMajor>(formKey);
        }
    }
}
