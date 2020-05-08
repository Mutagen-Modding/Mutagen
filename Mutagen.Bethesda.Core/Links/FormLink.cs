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
    /// </summary>
    /// <typeparam name="TMajor">The type of Major Record the Link is allowed to connect with</typeparam>
    public struct FormLink<TMajor> : IFormLink<TMajor>, IEquatable<IFormLinkGetter<TMajor>>
       where TMajor : class, IMajorRecordCommonGetter
    {
        /// <summary>
        /// A readonly singleton representing an unlinked FormLink
        /// </summary>
        public static readonly IFormLinkGetter<TMajor> Null = new FormLink<TMajor>();

        /// <summary>
        /// FormKey of the target record
        /// </summary>
        public FormKey FormKey { get; set; }
        
        Type ILinkGetter.TargetType => typeof(TMajor);

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

        /// <summary>
        /// Sets the link to the target Major Record
        /// </summary>
        /// <param name="value">Target record to link to</param>
        public void Set(TMajor value)
        {
            this.FormKey = value.FormKey;
        }

        /// <summary>
        /// Resets to an unlinked state
        /// </summary>
        public void Unset()
        {
            this.FormKey = FormKey.Null;
        }

        public static bool operator ==(FormLink<TMajor> lhs, IFormLink<TMajor> rhs)
        {
            return lhs.FormKey.Equals(rhs.FormKey);
        }

        public static bool operator !=(FormLink<TMajor> lhs, IFormLink<TMajor> rhs)
        {
            return !lhs.FormKey.Equals(rhs.FormKey);
        }

        /// <summary>
        /// Default Equality
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>True if object is ILinkGetter and FormKeys match</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ILinkGetter<TMajor> rhs)) return false;
            return this.Equals(rhs);
        }

        /// <summary>
        /// Compares equality of two links.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(IFormLinkGetter<TMajor> other) => this.FormKey.Equals(other.FormKey);
        
        /// <summary>
        /// Compares equality of two links, where rhs is a nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(IFormLinkNullableGetter<TMajor> other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKey, other.FormKey);

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
        /// <param name="package">Link Cache to resolve against</param>
        /// <param name="major">Located record if successful</param>
        /// <returns>True if link was resolved and a record was retrieved</returns>
        public bool TryResolve(ILinkCache package, [MaybeNullWhen(false)] out TMajor major)
        {
            if (this.FormKey.Equals(FormKey.Null))
            {
                major = default!;
                return false;
            }
            if (package.TryLookup<TMajor>(this.FormKey, out var majorRec))
            {
                major = majorRec;
                return true;
            }
            major = default!;
            return false;
        }

        bool ILinkGetter.TryResolveFormKey(ILinkCache package, [MaybeNullWhen(false)] out FormKey formKey)
        {
            formKey = this.FormKey;
            return true;
        }

        bool ILinkGetter.TryResolveCommon(ILinkCache package, [MaybeNullWhen(false)] out IMajorRecordCommonGetter formKey)
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

        bool ILinkGetter.TryGetModKey([MaybeNullWhen(false)] out ModKey modKey)
        {
            modKey = this.FormKey.ModKey;
            return true;
        }

        public static implicit operator FormLink<TMajor>(FormKey formKey)
        {
            return new FormLink<TMajor>(formKey);
        }
    }
}
