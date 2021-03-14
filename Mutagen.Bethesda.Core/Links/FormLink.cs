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
        IEquatable<IFormLinkGetter<TMajorGetter>>,
        IEquatable<IFormLinkNullableGetter<TMajorGetter>>,
        IEquatable<TMajorGetter>
        where TMajorGetter : class, IMajorRecordCommonGetter
    {
        /// <summary>
        /// A readonly singleton representing an unlinked FormLink
        /// </summary>
        public static IFormLinkGetter<TMajorGetter> Null => new FormLink<TMajorGetter>();

        /// <summary>
        /// FormKey of the target record
        /// </summary>
        public FormKey FormKey { get; set; }
        
        Type ILink.TargetType => typeof(TMajorGetter);

        /// <summary>
        /// True if unlinked and ID points to Null
        /// </summary>
        public bool IsNull => this.FormKey.IsNull;

        /// <inheritdoc />
        public Type Type => typeof(TMajorGetter);

        FormKey? IFormLinkGetter.FormKeyNullable => this.FormKey;

        FormKey? IFormLink<TMajorGetter>.FormKeyNullable
        {
            get => this.FormKey;
            set => this.FormKey = value ?? FormKey.Null;
        }

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
        public void SetTo(FormKey? formKey)
        {
            FormKey = formKey ?? FormKey.Null;
        }

        /// <summary>
        /// Sets the link to the target Record
        /// </summary>
        /// <param name="record">Target record to link to</param>
        public void SetTo(TMajorGetter? record)
        {
            FormKey = record?.FormKey ?? FormKey.Null;
        }

        /// <summary>
        /// Sets the link to match another link
        /// </summary>
        /// <param name="link">Target link to set to</param>
        public void SetTo(IFormLinkNullableGetter<TMajorGetter> link)
        {
            FormKey = link.FormKey;
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
        public bool Equals(IFormLinkGetter<TMajorGetter>? other) => this.FormKey.Equals(other?.FormKey);

        /// <summary>
        /// Compares equality of two links, where rhs is a nullable link.
        /// </summary>
        /// <param name="other">Other link to compare to</param>
        /// <returns>True if FormKey members are equal</returns>
        public bool Equals(IFormLinkNullableGetter<TMajorGetter>? other) => EqualityComparer<FormKey?>.Default.Equals(this.FormKey, other?.FormKeyNullable);

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

        bool ILink.TryResolveFormKey(ILinkCache cache, [MaybeNullWhen(false)] out FormKey formKey)
        {
            formKey = this.FormKey;
            return true;
        }

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

        bool ILink.TryGetModKey([MaybeNullWhen(false)] out ModKey modKey)
        {
            modKey = this.FormKey.ModKey;
            return true;
        }

        /// <summary> 
        /// Attempts to locate link target in given Link Cache. 
        /// </summary> 
        /// <param name="cache">Link Cache to resolve against</param> 
        /// <returns>TryGet object with located record if successful</returns> 
        public TMajorGetter? TryResolve(ILinkCache cache)
        {
            if (this.TryResolve(cache, out var rec))
            {
                return rec;
            }
            return default;
        }

        public bool Equals(TMajorGetter? other)
        {
            return IFormLinkExt.EqualsWithInheritanceConsideration(this, other);
        }

        public static implicit operator FormLink<TMajorGetter>(TMajorGetter major)
        {
            return new FormLink<TMajorGetter>(major.FormKey);
        }

        public static implicit operator FormLink<TMajorGetter>(FormKey formKey)
        {
            return new FormLink<TMajorGetter>(formKey);
        }

        public static implicit operator FormKey(FormLink<TMajorGetter> link)
        {
            return link.FormKey;
        }

        public static IEqualityComparer<IFormLinkGetter<TMajorGetter>> TypelessComparer => TypelessComparer<TMajorGetter>.Instance;
    }

    class TypelessComparer<TMajorGetter> : IEqualityComparer<IFormLinkGetter<TMajorGetter>>
        where TMajorGetter : class, IMajorRecordCommonGetter
    {
        public static readonly TypelessComparer<TMajorGetter> Instance = new TypelessComparer<TMajorGetter>();

        public bool Equals(IFormLinkGetter<TMajorGetter>? x, IFormLinkGetter<TMajorGetter>? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.FormKey == y.FormKey;
        }

        public int GetHashCode(IFormLinkGetter<TMajorGetter> obj)
        {
            return obj.GetHashCode();
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
