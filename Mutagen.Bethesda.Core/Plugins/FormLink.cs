using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Utility;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda.Plugins;

/// <summary>
/// A FormKey with an associated Major Record Type that it is allowed to link to.
/// This provides type safety concepts on top of a basic FormKey.
/// </summary>
/// <typeparam name="TMajorGetter">The type of Major Record the Link is allowed to connect with</typeparam>
[DebuggerDisplay("{this.FormKey}")]
public class FormLinkGetter<TMajorGetter> : IFormLinkGetter<TMajorGetter>,
    IEquatable<FormLink<TMajorGetter>>,
    IEquatable<FormLinkNullable<TMajorGetter>>,
    IEquatable<IFormLinkGetter<TMajorGetter>>,
    IEquatable<IFormLinkNullableGetter<TMajorGetter>>,
    IEquatable<TMajorGetter>
    where TMajorGetter : class, IMajorRecordGetter
{
    protected FormKey _formKey;

    /// <summary>
    /// A readonly singleton representing an unlinked FormLink
    /// </summary>
    public static readonly IFormLinkGetter<TMajorGetter> Null = new FormLinkGetter<TMajorGetter>();

    /// <summary>
    /// FormKey of the target record
    /// </summary>
    public FormKey FormKey => _formKey;

    /// <summary>
    /// True if unlinked and ID points to Null
    /// </summary>
    public bool IsNull => FormKey.IsNull;

    /// <inheritdoc />
    public Type Type => typeof(TMajorGetter);

    FormKey? IFormLinkGetter.FormKeyNullable => FormKey;

    public bool TryResolveFormKey(ILinkCache cache, [MaybeNullWhen(false)] out FormKey formKey)
    {
        formKey = FormKey;
        return true;
    }

    public bool TryResolveCommon(ILinkCache cache, [MaybeNullWhen(false)] out IMajorRecordGetter formKey)
    {
        if (this.TryResolve(cache, out var rec))
        {
            formKey = rec;
            return true;
        }
        formKey = default;
        return false;
    }

    public bool TryGetModKey([MaybeNullWhen(false)] out ModKey modKey)
    {
        modKey = FormKey.ModKey;
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
    public bool Equals(FormLink<TMajorGetter>? other) => FormKey.Equals(other?.FormKey ?? FormKey.Null);

    /// <summary>
    /// Compares equality of two links, where rhs is a nullable link.
    /// </summary>
    /// <param name="other">Other link to compare to</param>
    /// <returns>True if FormKey members are equal</returns>
    public bool Equals(FormLinkNullable<TMajorGetter>? other) => EqualityComparer<FormKey?>.Default.Equals(FormKey, other?.FormKeyNullable);

    /// <summary>
    /// Compares equality of two links.
    /// </summary>
    /// <param name="other">Other link to compare to</param>
    /// <returns>True if FormKey members are equal</returns>
    public bool Equals(IFormLinkGetter<TMajorGetter>? other) => FormKey.Equals(other?.FormKey);

    /// <summary>
    /// Compares equality of two links, where rhs is a nullable link.
    /// </summary>
    /// <param name="other">Other link to compare to</param>
    /// <returns>True if FormKey members are equal</returns>
    public bool Equals(IFormLinkNullableGetter<TMajorGetter>? other) => EqualityComparer<FormKey?>.Default.Equals(FormKey, other?.FormKeyNullable);

    /// <summary>
    /// Returns hash code
    /// </summary>
    /// <returns>Hash code evaluated from FormKey member</returns>
    public override int GetHashCode() => FormKey.GetHashCode();

    /// <summary>
    /// Returns string representation of link
    /// </summary>
    /// <returns>Returns FormKey string</returns>
    public override string ToString() => $"<{MajorRecordPrinter<TMajorGetter>.TypeString}>{FormKey}";

    public bool Equals(TMajorGetter? other)
    {
        return IFormLinkExt.EqualsWithInheritanceConsideration(this, other);
    }

    public IFormLink<TMajorRet> Cast<TMajorRet>() 
        where TMajorRet : class, IMajorRecordGetter
    {
        return new FormLink<TMajorRet>(FormKey);
    }

    public static IEqualityComparer<IFormLinkGetter<TMajorGetter>> TypelessComparer => FormLinkTypelessComparer<TMajorGetter>.Instance;
}

/// <summary>
/// A FormKey with an associated Major Record Type that it is allowed to link to.
/// This provides type safety concepts on top of a basic FormKey.
/// </summary>
/// <typeparam name="TMajorGetter">The type of Major Record the Link is allowed to connect with</typeparam>
[DebuggerDisplay("{this.FormKey}")]
public class FormLink<TMajorGetter> : FormLinkGetter<TMajorGetter>, IFormLink<TMajorGetter>
    where TMajorGetter : class, IMajorRecordGetter
{
    /// <summary>
    /// FormKey of the target record
    /// </summary>
    public new FormKey FormKey
    {
        get => _formKey;
        set => _formKey = value;
    }

    FormKey? IFormLink<TMajorGetter>.FormKeyNullable
    {
        get => FormKey;
        set => FormKey = value ?? FormKey.Null;
    }

    public FormLink()
    {
        FormKey = FormKey.Null;
    }

    /// <summary>
    /// Default constructor that creates a link to the target FormKey
    /// </summary>
    public FormLink(FormKey formKey)
    {
        FormKey = formKey;
    }

    /// <summary>
    /// Default constructor that creates a link to the target FormKey
    /// </summary>
    public FormLink(TMajorGetter record)
    {
        FormKey = record.FormKey;
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

    public void SetToNull()
    {
        _formKey = FormKey.Null;
    }

    public void Clear()
    {
        FormKey = FormKey.Null;
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
    
[DebuggerDisplay("{this.FormKey}")]
public struct FormLink<TMajor, TMajorGetter>
    where TMajor : class, IMajorRecord, TMajorGetter
    where TMajorGetter : class, IMajorRecordGetter
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
        FormKey = formKey;
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