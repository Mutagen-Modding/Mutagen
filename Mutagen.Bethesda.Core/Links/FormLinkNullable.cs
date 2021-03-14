using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// A FormKey with an associated Major Record Type that it is allowed to link to.
    /// This provides type safety concepts on top of a basic FormKey.
    /// FormKey allowed to be null to communicate the absence of the field.
    /// </summary>
    /// <typeparam name="TMajorGetter">The type of Major Record the Link is allowed to connect with</typeparam>
    public class FormLinkNullable<TMajorGetter> :
        IFormLinkNullable<TMajorGetter>,
        IEquatable<FormLink<TMajorGetter>>,
        IEquatable<FormLinkNullable<TMajorGetter>>,
        IEquatable<IFormLinkGetter<TMajorGetter>>,
        IEquatable<IFormLinkNullableGetter<TMajorGetter>>
        where TMajorGetter : class, IMajorRecordCommonGetter
    {
        /// <summary>
        /// A readonly singleton representing an unlinked and null FormLinkNullable
        /// </summary>
        public static readonly FormLinkNullable<TMajorGetter> Null = new FormLinkNullable<TMajorGetter>();

        /// <summary>
        /// FormKey of the target record.
        /// </summary>
        public FormKey? FormKeyNullable { get; set; }

        /// <summary>
        /// Non null FormKey of the target record.  If null, it will instead return FormKey.Null.
        /// </summary>
        public FormKey FormKey
        {
            get => FormKeyNullable ?? FormKey.Null;
            set => FormKeyNullable = value;
        }
        
        Type ILink.TargetType => typeof(TMajorGetter);

        /// <inheritdoc />
        public Type Type => typeof(TMajorGetter);

        /// <summary>
        /// True if unlinked and ID points to Null
        /// </summary>
        public bool IsNull => this.FormKeyNullable?.IsNull ?? true;

        public FormLinkNullable()
        {
        }

        /// <summary>
        /// Default constructor that creates a link to the target FormKey
        /// </summary>
        public FormLinkNullable(FormKey? formKey)
        {
            this.FormKeyNullable = formKey;
        }

        /// <summary>
        /// Default constructor that creates a link to the target record
        /// </summary>
        public FormLinkNullable(TMajorGetter? record)
        {
            this.FormKeyNullable = record?.FormKey;
        }

        /// <summary>
        /// Sets the link to the target FormKey
        /// </summary>
        /// <param name="formKey">Target FormKey to link to</param>
        public void SetTo(FormKey? formKey)
        {
            this.FormKeyNullable = formKey;
        }

        /// <summary>
        /// Sets the link to the target Record
        /// </summary>
        /// <param name="record">Target record to link to</param>
        public void SetTo(TMajorGetter? record)
        {
            this.FormKeyNullable = record?.FormKey;
        }

        /// <summary>
        /// Sets the link to match another link
        /// </summary>
        /// <param name="link">Target link to set to</param>
        public void SetTo(IFormLinkNullableGetter<TMajorGetter> link)
        {
            this.FormKeyNullable = link.FormKeyNullable;
        }

        public void Clear()
        {
            this.FormKeyNullable = null;
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
            return IFormLinkExt.EqualsWithInheritanceConsideration(this, obj);
        }

        /// <summary>
        /// Compares equality of two links, where rhs is a non nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(FormLink<TMajorGetter>? other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKeyNullable, other?.FormKey);

        /// <summary>
        /// Compares equality of two links, where rhs is a nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(FormLinkNullable<TMajorGetter>? other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKeyNullable, other?.FormKeyNullable);

        /// <summary>
        /// Compares equality of two links, where rhs is a non nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(IFormLinkGetter<TMajorGetter>? other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKeyNullable, other?.FormKey);

        /// <summary>
        /// Compares equality of two links, where rhs is a nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(IFormLinkNullableGetter<TMajorGetter>? other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKeyNullable, other?.FormKeyNullable);

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

        bool ILink.TryResolveCommon(ILinkCache cache, [MaybeNullWhen(false)] out IMajorRecordCommonGetter formKey)
        {
            if (this.TryResolve(cache, out var rec))
            {
                formKey = rec;
                return true;
            }
            formKey = default!;
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

        /// <summary>
        /// Attempts to locate link target in given Link Cache.
        /// </summary>
        /// <param name="cache">Link Cache to resolve against</param>
        /// <returns>TryGet object with located record if successful</returns>
        public ITryGetter<TMajorGetter> TryResolve(ILinkCache cache)
        {
            if (this.TryResolve(cache, out var rec))
            {
                return TryGet<TMajorGetter>.Succeed(rec);
            }
            return TryGet<TMajorGetter>.Failure;
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
