using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda.Plugins;

/// <summary>
/// A FormKey with an associated Major Record Type that it is allowed to link to.
/// This provides type safety concepts on top of a basic FormKey.
/// FormKey allowed to be null to communicate the absence of the field.
/// </summary>
/// <typeparam name="TMajorGetter">The type of Major Record the Link is allowed to connect with</typeparam>
public class FormLinkNullableGetter<TMajorGetter> :
    IFormLinkNullableGetter<TMajorGetter>,
    IEquatable<FormLink<TMajorGetter>>,
    IEquatable<FormLinkNullable<TMajorGetter>>,
    IEquatable<IFormLinkGetter<TMajorGetter>>,
    IEquatable<IFormLinkNullableGetter<TMajorGetter>>
    where TMajorGetter : class, IMajorRecordGetter
{
    protected FormKey? _formKey;

    /// <summary>
    /// A readonly singleton representing an unlinked and null FormLinkNullable
    /// </summary>
    public static readonly IFormLinkNullableGetter<TMajorGetter> Null = new FormLinkNullableGetter<TMajorGetter>();

    /// <summary>
    /// FormKey of the target record.
    /// </summary>
    public FormKey? FormKeyNullable => _formKey;

    /// <summary>
    /// Non null FormKey of the target record.  If null, it will instead return FormKey.Null.
    /// </summary>o
    public FormKey FormKey => _formKey ?? FormKey.Null;

    /// <inheritdoc />
    public Type Type => typeof(TMajorGetter);

    /// <summary>
    /// True if unlinked and ID points to Null
    /// </summary>
    public bool IsNull => _formKey?.IsNull ?? true;

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
    public bool Equals(FormLink<TMajorGetter>? other) => EqualityComparer<FormKey?>.Default.Equals(this._formKey, other?.FormKey);

    /// <summary>
    /// Compares equality of two links, where rhs is a nullable link.
    /// </summary>
    /// <param name="other">Other link to compare to</param>
    /// <returns>True if FormKey members are equal</returns>
    public bool Equals(FormLinkNullable<TMajorGetter>? other) => EqualityComparer<FormKey?>.Default.Equals(this._formKey, other?._formKey);

    /// <summary>
    /// Compares equality of two links, where rhs is a non nullable link.
    /// </summary>
    /// <param name="other">Other link to compare to</param>
    /// <returns>True if FormKey members are equal</returns>
    public bool Equals(IFormLinkGetter<TMajorGetter>? other) => EqualityComparer<FormKey?>.Default.Equals(this._formKey, other?.FormKey);

    /// <summary>
    /// Compares equality of two links, where rhs is a nullable link.
    /// </summary>
    /// <param name="other">Other link to compare to</param>
    /// <returns>True if FormKey members are equal</returns>
    public bool Equals(IFormLinkNullableGetter<TMajorGetter>? other) => EqualityComparer<FormKey?>.Default.Equals(this._formKey, other?.FormKeyNullable);

    /// <summary>
    /// Returns hash code
    /// </summary>
    /// <returns>Hash code evaluated from FormKey member</returns>
    public override int GetHashCode() => _formKey?.GetHashCode() ?? 0;

    /// <summary>
    /// Returns string representation of link
    /// </summary>
    /// <returns>Returns FormKey string</returns>
    public override string ToString() => this._formKey?.ToString() ?? "Null";

    bool ILink.TryResolveCommon(ILinkCache cache, [MaybeNullWhen(false)] out IMajorRecordGetter formKey)
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
        if (_formKey == null)
        {
            formKey = default!;
            return false;
        }
        formKey = _formKey.Value;
        return true;
    }

    /// <summary> 
    /// Attempts to locate an associated ModKey from the link 
    /// </summary> 
    /// <param name="modKey">ModKey if found</param> 
    /// <returns>True if FormKey is not null</returns> 
    public bool TryGetModKey(out ModKey modKey)
    {
        if (_formKey is {} formKey)
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
    public TMajorGetter? TryResolve(ILinkCache cache)
    {
        if (this.TryResolve(cache, out var rec))
        {
            return rec;
        }
        return default;
    }

    IFormLinkNullable<TMajorRet> IFormLinkNullableGetter<TMajorGetter>.Cast<TMajorRet>()
    {
        return new FormLinkNullable<TMajorRet>(this.FormKeyNullable);
    }

    IFormLink<TMajorRet> IFormLinkGetter<TMajorGetter>.Cast<TMajorRet>()
    {
        return new FormLinkNullable<TMajorRet>(this.FormKeyNullable);
    }
}

public class FormLinkNullable<TMajorGetter> : FormLinkNullableGetter<TMajorGetter>, IFormLinkNullable<TMajorGetter>
    where TMajorGetter : class, IMajorRecordGetter
{
    /// <summary>
    /// FormKey of the target record.
    /// </summary>
    public new FormKey? FormKeyNullable
    {
        get => _formKey;
        set => _formKey = value;
    }

    /// <summary>
    /// Non null FormKey of the target record.  If null, it will instead return FormKey.Null.
    /// </summary>
    public new FormKey FormKey
    {
        get => _formKey ?? FormKey.Null;
        set => _formKey = value;
    }

    public FormLinkNullable()
    {
    }

    /// <summary>
    /// Default constructor that creates a link to the target FormKey
    /// </summary>
    public FormLinkNullable(FormKey? formKey)
    {
        _formKey = formKey;
    }

    /// <summary>
    /// Default constructor that creates a link to the target record
    /// </summary>
    public FormLinkNullable(TMajorGetter? record)
    {
        _formKey = record?.FormKey;
    }

    /// <summary>
    /// Sets the link to the target FormKey
    /// </summary>
    /// <param name="formKey">Target FormKey to link to</param>
    public void SetTo(FormKey? formKey)
    {
        _formKey = formKey;
    }

    /// <summary>
    /// Sets the link to the target Record
    /// </summary>
    /// <param name="record">Target record to link to</param>
    public void SetTo(TMajorGetter? record)
    {
        _formKey = record?.FormKey;
    }

    /// <summary>
    /// Sets the link to match another link
    /// </summary>
    /// <param name="link">Target link to set to</param>
    public void SetTo(IFormLinkNullableGetter<TMajorGetter> link)
    {
        _formKey = link.FormKeyNullable;
    }

    public void Clear()
    {
        _formKey = null;
    }

    public void SetToNull()
    {
        _formKey = null;
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