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
    public class FormLinkNullable<TMajor> : IFormLinkNullable<TMajor>, IEquatable<IFormLinkGetter<TMajor>>, IEquatable<IFormLinkNullableGetter<TMajor>>
       where TMajor : class, IMajorRecordCommonGetter
    {
        /// <summary>
        /// A readonly singleton representing an unlinked and null FormLinkNullable
        /// </summary>
        public static readonly IFormLinkNullableGetter<TMajor> Empty = new FormLinkNullable<TMajor>();

        /// <summary>
        /// FormKey of the target record.
        /// </summary>
        public FormKey? FormKey { get; set; } = null;
        
        Type ILinkGetter.TargetType => typeof(TMajor);

        /// <summary>
        /// Default constructor that starts unlinked and null
        /// </summary>
        public FormLinkNullable()
        {
        }

        /// <summary>
        /// Default constructor that creates a link to the target FormKey
        /// </summary>
        public FormLinkNullable(FormKey? formKey)
        {
            this.FormKey = formKey;
        }

        /// <summary>
        /// Sets the link to the target FormKey
        /// </summary>
        /// <param name="formKey">Target FormKey to link to</param>
        public void Set(FormKey? formKey)
        {
            this.FormKey = formKey;
        }

        /// <summary>
        /// Sets the link to the target Major Record
        /// </summary>
        /// <param name="value">Target record to link to</param>
        public void Set(TMajor? value)
        {
            this.FormKey = value?.FormKey;
        }

        /// <summary>
        /// Resets to an unlinked and null state
        /// </summary>
        public void Unset()
        {
            this.FormKey = default;
        }

        public static bool operator ==(FormLinkNullable<TMajor> lhs, IFormLink<TMajor> rhs)
        {
            return lhs.FormKey?.Equals(rhs.FormKey) ?? false;
        }

        public static bool operator !=(FormLinkNullable<TMajor> lhs, IFormLink<TMajor> rhs)
        {
            return !lhs.FormKey?.Equals(rhs.FormKey) ?? true;
        }

        /// <summary>
        /// Default Equality
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>True if object is ILinkGetter<TMajor> and FormKeys match</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ILinkGetter<TMajor> rhs)) return false;
            return this.Equals(rhs);
        }
        
        /// <summary>
        /// Compares equality of two links, where rhs is a non nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(IFormLinkGetter<TMajor> other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKey, other.FormKey);

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
        public override int GetHashCode() => this.FormKey?.GetHashCode() ?? 0;

        /// <summary>
        /// Returns string representation of link
        /// </summary>
        /// <returns>Returns FormKey string</returns>
        public override string ToString() => this.FormKey?.ToString() ?? string.Empty;

        /// <summary>
        /// Attempts to locate link target in given Link Cache.
        /// </summary>
        /// <param name="package">Link Cache to resolve against</param>
        /// <param name="major">Located record if successful</param>
        /// <typeparam name="TMod">Mod type</typeparam>
        /// <returns>True if link was resolved and a record was retrieved</returns>
        public bool TryResolve<TMod>(ILinkCache<TMod> package, [MaybeNullWhen(false)] out TMajor major)
            where TMod : IModGetter
        {
            if (this.FormKey == null
                || this.FormKey.Equals(FormKey.Null))
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

        /// <summary>
        /// Attempts to locate an associated FormKey from the link
        /// </summary>
        /// <param name="formKey">FormKey if found</param>
        /// <returns>True if FormKey is not null</returns>
        /// <typeparam name="TMod">Mod type</typeparam>
        public bool TryResolveFormKey<TMod>(ILinkCache<TMod> package, [MaybeNullWhen(false)] out FormKey formKey)
            where TMod : IModGetter
        {
            if (this.FormKey == null)
            {
                formKey = default!;
                return false;
            }
            formKey = this.FormKey;
            return true;
        }

        bool ILinkGetter.TryResolveCommon<M>(ILinkCache<M> package, [MaybeNullWhen(false)] out IMajorRecordCommonGetter formKey)
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
        /// <typeparam name="TMod">Mod type</typeparam>
        public ITryGetter<TMajor> TryResolve<TMod>(ILinkCache<TMod> package)
            where TMod : IModGetter
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
    }
}
