using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Records;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Plugins;

public interface IFormLinkOrIndexFlagGetter
{
    bool UseAliases { get; }
    bool UsePackageData { get; }
}

public interface IFormLinkOrIndexGetter<TMajorGetter> : IFormLinkContainerGetter, IPrintable
    where TMajorGetter : class, IMajorRecordGetter
{
    IFormLinkNullableGetter<TMajorGetter> Link { get; }
    
    uint? Index { get; } 
    
    [MemberNotNullWhen(false, nameof(Index))]
    bool UsesLink();
    
    [MemberNotNullWhen(true, nameof(Index))]
    bool UsesAlias();
    
    [MemberNotNullWhen(true, nameof(Index))]
    bool UsesPackageData();
}

public interface IFormLinkOrIndex<TMajorGetter> : IFormLinkOrIndexGetter<TMajorGetter>, IFormLinkContainer
    where TMajorGetter : class, IMajorRecordGetter
{
    new IFormLinkNullable<TMajorGetter> Link { get; }
    new uint? Index { get; set; }
    void Clear();
    void SetTo<TMajorRhs>(IFormLinkOrIndexGetter<TMajorRhs> rhs)
        where TMajorRhs : class, TMajorGetter;
    public void Relink(IReadOnlyDictionary<FormKey, FormKey> mapping);
}