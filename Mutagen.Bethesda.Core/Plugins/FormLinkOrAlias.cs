using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Plugins;

public class FormLinkOrAliasGetter<TMajorGetter> : IFormLinkOrAliasGetter<TMajorGetter>
    where TMajorGetter : class, IMajorRecordGetter
{
    private readonly IFormLinkOrAliasFlagGetter _parent;
    public IFormLinkNullableGetter<TMajorGetter> Link { get; }
    public uint? Alias { get; }

    public FormLinkOrAliasGetter(IFormLinkOrAliasFlagGetter parent, uint alias)
    {
        _parent = parent;
        Alias = alias;
        Link = FormLinkNullableGetter<TMajorGetter>.Null;
    }

    public FormLinkOrAliasGetter(IFormLinkOrAliasFlagGetter parent, FormKey key)
    {
        _parent = parent;
        Alias = default;
        Link = new FormLinkNullable<TMajorGetter>(key);
    }
    
    [MemberNotNullWhen(false, nameof(Alias))]
    public bool UsesLink()
    {
        return !_parent.UseAliases;
    }

    [MemberNotNullWhen(true, nameof(Alias))]
    public bool UsesAlias()
    {
        return _parent.UseAliases;
    }

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks()
    {
        if (UsesLink())
        {
            yield return Link;
        }
    }

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        throw new NotImplementedException();
    }

    public TMajorGetter? TryResolve(ILinkCache cache)
    {
        return Link.TryResolve(cache);
    }

    IFormLink<TMajorRet> IFormLinkGetter<TMajorGetter>.Cast<TMajorRet>()
    {
        return Link.Cast<TMajorRet>();
    }

    IFormLinkNullable<TMajorRet> IFormLinkNullableGetter<TMajorGetter>.Cast<TMajorRet>()
    {
        return Link.Cast<TMajorRet>();
    }

    public Type Type => Link.Type;

    public bool TryGetModKey(out ModKey modKey)
    {
        return Link.TryGetModKey(out modKey);
    }

    public bool TryResolveFormKey(ILinkCache cache, out FormKey formKey)
    {
        return Link.TryResolveFormKey(cache, out formKey);
    }

    public bool TryResolveCommon(ILinkCache cache, [MaybeNullWhen(false)] out IMajorRecordGetter majorRecord)
    {
        return Link.TryResolveCommon(cache, out majorRecord);
    }

    public FormKey FormKey => Link.FormKey;

    public FormKey? FormKeyNullable => Link.FormKeyNullable;

    public bool IsNull => Link.IsNull;
}

public class FormLinkOrAlias<TMajorGetter> : IFormLinkOrAlias<TMajorGetter>
    where TMajorGetter : class, IMajorRecordGetter
{
    private readonly IFormLinkOrAliasFlagGetter _parent;
    
    IFormLinkNullableGetter<TMajorGetter> IFormLinkOrAliasGetter<TMajorGetter>.Link => Link;

    public IFormLinkNullable<TMajorGetter> Link { get; }
    public uint? Alias { get; set; }

    public FormLinkOrAlias(IFormLinkOrAliasFlagGetter parent)
    {
        _parent = parent;
        Alias = default;
        Link = new FormLinkNullable<TMajorGetter>();
    }

    public FormLinkOrAlias(IFormLinkOrAliasFlagGetter parent, uint alias)
    {
        _parent = parent;
        Alias = alias;
        Link = new FormLinkNullable<TMajorGetter>();
    }

    public FormLinkOrAlias(IFormLinkOrAliasFlagGetter parent, FormKey key)
    {
        _parent = parent;
        Alias = default;
        Link = new FormLinkNullable<TMajorGetter>(key);
    }

    public static FormLinkOrAlias<TMajorGetter> Factory(IFormLinkOrAliasFlagGetter parent, FormKey key, uint alias)
    {
        if (parent.UseAliases)
        {
            return new FormLinkOrAlias<TMajorGetter>(parent, alias);
        }

        return new FormLinkOrAlias<TMajorGetter>(parent, key);
    }

    [MemberNotNullWhen(false, nameof(Alias))]
    public bool UsesLink()
    {
        return !_parent.UseAliases;
    }

    [MemberNotNullWhen(true, nameof(Alias))]
    public bool UsesAlias()
    {
        return _parent.UseAliases;
    }

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks()
    {
        if (!_parent.UseAliases)
        {
            yield return Link;
        }
    }

    public void RemapLinks(IReadOnlyDictionary<FormKey, FormKey> mapping)
    {
        Link.Relink(mapping);
    }
    
    public void Clear()
    {
        Link.Clear();
        Alias = 0;
    }

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        throw new NotImplementedException();
    }

    public TMajorGetter? TryResolve(ILinkCache cache)
    {
        return Link.TryResolve(cache);
    }

    IFormLink<TMajorRet> IFormLinkGetter<TMajorGetter>.Cast<TMajorRet>()
    {
        return Link.Cast<TMajorRet>();
    }

    IFormLinkNullable<TMajorRet> IFormLinkNullableGetter<TMajorGetter>.Cast<TMajorRet>()
    {
        return Link.Cast<TMajorRet>();
    }

    public Type Type => Link.Type;

    public bool TryGetModKey(out ModKey modKey)
    {
        return Link.TryGetModKey(out modKey);
    }

    public bool TryResolveFormKey(ILinkCache cache, out FormKey formKey)
    {
        return Link.TryResolveFormKey(cache, out formKey);
    }

    public bool TryResolveCommon(ILinkCache cache, [MaybeNullWhen(false)] out IMajorRecordGetter majorRecord)
    {
        return Link.TryResolveCommon(cache, out majorRecord);
    }

    public FormKey FormKey
    {
        get => Link.FormKey;
        set => Link.FormKey = value;
    }

    public void SetTo(FormKey? formKey)
    {
        Link.SetTo(formKey);
    }

    public void SetToNull()
    {
        Link.SetToNull();
    }

    public FormKey? FormKeyNullable
    {
        get => Link.FormKeyNullable;
        set => Link.FormKeyNullable = value;
    }

    public bool IsNull => Link.IsNull;
}