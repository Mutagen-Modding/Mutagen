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
        public FormKey? FormKey { get; }
        
        Type ILink.TargetType => typeof(TMajor);

        /// <summary>
        /// True if unlinked and ID points to Null
        /// </summary>
        public bool IsNull => this.FormKey?.IsNull ?? true;

        /// <summary>
        /// Default constructor that creates a link to the target FormKey
        /// </summary>
        public FormLinkNullable(FormKey? formKey)
        {
            this.FormKey = formKey;
        }

        public static bool operator ==(FormLinkNullable<TMajor> lhs, FormLink<TMajor> rhs)
        {
            return lhs.FormKey?.Equals(rhs.FormKey) ?? false;
        }

        public static bool operator !=(FormLinkNullable<TMajor> lhs, FormLink<TMajor> rhs)
        {
            return !lhs.FormKey?.Equals(rhs.FormKey) ?? true;
        }

        public static bool operator ==(FormLink<TMajor> lhs, FormLinkNullable<TMajor> rhs)
        {
            return EqualityComparer<FormKey?>.Default.Equals(lhs.FormKey, rhs.FormKey);
        }

        public static bool operator !=(FormLink<TMajor> lhs, FormLinkNullable<TMajor> rhs)
        {
            return !EqualityComparer<FormKey?>.Default.Equals(lhs.FormKey, rhs.FormKey);
        }

        /// <summary>
        /// Default Equality
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>True if object is ILinkGetter and FormKeys match</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ILink<TMajor> rhs)) return false;
            return this.Equals(rhs);
        }

        /// <summary>
        /// Compares equality of two links, where rhs is a non nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(FormLink<TMajor> other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKey, other.FormKey);

        /// <summary>
        /// Compares equality of two links, where rhs is a nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(FormLinkNullable<TMajor> other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKey, other.FormKey);

        /// <summary>
        /// Compares equality of two links, where rhs is a non nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(IFormLink<TMajor> other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKey, other.FormKey);

        /// <summary>
        /// Compares equality of two links, where rhs is a nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(IFormLinkNullable<TMajor> other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKey, other.FormKey);

        /// <summary>
        /// Returns hash code
        /// </summary>
        /// <returns>Hash code evaluated from FormKey member</returns>
        public override int GetHashCode() => this.FormKey?.GetHashCode() ?? 0;

        /// <summary>
        /// Returns string representation of link
        /// </summary>
        /// <returns>Returns FormKey string</returns>
        public override string ToString() => this.FormKey?.ToString() ?? "Null";

        /// <summary>
        /// Attempts to locate link target in given Link Cache.
        /// </summary>
        /// <param name="package">Link Cache to resolve against</param>
        /// <param name="major">Located record if successful</param>
        /// <returns>True if link was resolved and a record was retrieved</returns>
        public bool TryResolve(ILinkCache package, [MaybeNullWhen(false)] out TMajor major)
        {
            if (this.FormKey == null
                || this.FormKey.Equals(Mutagen.Bethesda.FormKey.Null))
            {
                major = default!;
                return false;
            }
            if (package.TryLookup<TMajor>(this.FormKey.Value, out var majorRec))
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
        /// <param name="package">Link Cache to resolve against</param>
        /// <param name="formKey">FormKey if found</param>
        /// <returns>True if FormKey is not null</returns>
        public bool TryResolveFormKey(ILinkCache package, [MaybeNullWhen(false)] out FormKey formKey)
        {
            if (this.FormKey == null)
            {
                formKey = default!;
                return false;
            }
            formKey = this.FormKey.Value;
            return true;
        }

        bool ILink.TryResolveCommon(ILinkCache package, [MaybeNullWhen(false)] out IMajorRecordCommonGetter formKey)
        {
            if (TryResolve(package, out var rec))
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
        /// <param name="package">Link Cache to resolve against</param>
        /// <returns>TryGet object with located record if successful</returns>
        public ITryGetter<TMajor> TryResolve(ILinkCache package)
        {
            if (TryResolve(package, out var rec))
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
            if (this.FormKey.TryGet(out var formKey))
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
