using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Records;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Plugins;

public class FormLinkOrIndexGetter<TMajorGetter> : IFormLinkOrIndexGetter<TMajorGetter>, IEquatable<IFormLinkOrIndexGetter<TMajorGetter>>
    where TMajorGetter : class, IMajorRecordGetter
{
    private readonly IFormLinkOrIndexFlagGetter _parent;
    public IFormLinkNullableGetter<TMajorGetter> Link { get; }
    public uint? Index { get; }

    public FormLinkOrIndexGetter(IFormLinkOrIndexFlagGetter parent, uint index)
    {
        _parent = parent;
        Index = index;
        Link = FormLinkNullableGetter<TMajorGetter>.Null;
    }

    public FormLinkOrIndexGetter(IFormLinkOrIndexFlagGetter parent, FormKey key)
    {
        _parent = parent;
        Index = default;
        Link = new FormLinkNullable<TMajorGetter>(key);
    }
    
    [MemberNotNullWhen(false, nameof(Index))]
    public bool UsesLink()
    {
        return !_parent.UseAliases && ! _parent.UsePackageData;
    }

    [MemberNotNullWhen(true, nameof(Index))]
    public bool UsesAlias()
    {
        return _parent.UseAliases;
    }

    [MemberNotNullWhen(true, nameof(Index))]
    public bool UsesPackageData()
    {
        return _parent.UsePackageData;
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

    public bool Equals(IFormLinkOrIndexGetter<TMajorGetter>? other) => FormLinkOrIndex<TMajorGetter>.EqualityComparer.Equals(this, other);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not IFormLinkOrIndexGetter<TMajorGetter> rhs) return false;
        return Equals(rhs);
    }

    public override int GetHashCode() => FormLinkOrIndex<TMajorGetter>.EqualityComparer.GetHashCode(this);
}

public class FormLinkOrIndex<TMajorGetter> : IFormLinkOrIndex<TMajorGetter>
    where TMajorGetter : class, IMajorRecordGetter
{
    private readonly IFormLinkOrIndexFlagGetter _parent;
    
    IFormLinkNullableGetter<TMajorGetter> IFormLinkOrIndexGetter<TMajorGetter>.Link => Link;

    public IFormLinkNullable<TMajorGetter> Link { get; }
    public uint? Index { get; set; }

    public FormLinkOrIndex(IFormLinkOrIndexFlagGetter parent)
    {
        _parent = parent;
        Index = default;
        Link = new FormLinkNullable<TMajorGetter>();
    }

    public FormLinkOrIndex(IFormLinkOrIndexFlagGetter parent, uint index)
    {
        _parent = parent;
        Index = index;
        Link = new FormLinkNullable<TMajorGetter>();
    }

    public FormLinkOrIndex(IFormLinkOrIndexFlagGetter parent, FormKey key)
    {
        _parent = parent;
        Index = default;
        Link = new FormLinkNullable<TMajorGetter>(key);
    }

    public static FormLinkOrIndex<TMajorGetter> Factory(IFormLinkOrIndexFlagGetter parent, FormKey key, uint index)
    {
        if (parent.UseAliases || parent.UsePackageData)
        {
            return new FormLinkOrIndex<TMajorGetter>(parent, index);
        }

        return new FormLinkOrIndex<TMajorGetter>(parent, key);
    }

    [MemberNotNullWhen(false, nameof(Index))]
    public bool UsesLink()
    {
        return !_parent.UseAliases && ! _parent.UsePackageData;
    }

    [MemberNotNullWhen(true, nameof(Index))]
    public bool UsesAlias()
    {
        return _parent.UseAliases;
    }

    [MemberNotNullWhen(true, nameof(Index))]
    public bool UsesPackageData()
    {
        return _parent.UsePackageData;
    }

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks()
    {
        if (UsesLink())
        {
            yield return Link;
        }
    }

    public void RemapLinks(IReadOnlyDictionary<FormKey, FormKey> mapping)
    {
        if (UsesLink())
        {
            Link.Relink(mapping);
        }
    }
    
    public void Clear()
    {
        Link.Clear();
        Index = 0;
    }

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        throw new NotImplementedException();
    }

    public bool Equals(IFormLinkOrIndexGetter<TMajorGetter>? other) => FormLinkOrIndex<TMajorGetter>.EqualityComparer.Equals(this, other);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not IFormLinkOrIndexGetter<TMajorGetter> rhs) return false;
        return Equals(rhs);
    }

    public override int GetHashCode() => FormLinkOrIndex<TMajorGetter>.EqualityComparer.GetHashCode(this);

    public static IEqualityComparer<IFormLinkOrIndexGetter<TMajorGetter>> EqualityComparer { get; } = new FormLinkOrIndexEqualityComparer();

    private sealed class FormLinkOrIndexEqualityComparer : IEqualityComparer<IFormLinkOrIndexGetter<TMajorGetter>>
    {
        public bool Equals(IFormLinkOrIndexGetter<TMajorGetter>? x, IFormLinkOrIndexGetter<TMajorGetter>? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            var usesLink = x.UsesLink();
            if (usesLink != y.UsesLink()) return false;
            if (usesLink)
            {
                return x.Link.Equals(y.Link);
            }

            return x.Index == y.Index;
        }

        public int GetHashCode(IFormLinkOrIndexGetter<TMajorGetter> obj)
        {
            if (obj.UsesLink())
            {
                return obj.Link.GetHashCode();
            }
            return obj.Index.GetHashCode();
        }
    }

    public void SetTo<TMajorRhs>(IFormLinkOrIndexGetter<TMajorRhs> rhs)
        where TMajorRhs : class, TMajorGetter
    {
        Index = rhs.Index;
        Link.SetTo(rhs.Link);
    }

    public void Relink(IReadOnlyDictionary<FormKey, FormKey> mapping)
    {
        Link.Relink(mapping);
    }
}