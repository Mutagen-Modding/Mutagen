using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Records;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Plugins;

public interface IFormLinkOrAliasFlagGetter
{
    bool UseAliases { get; }
}

public interface IFormLinkOrAliasGetter<TMajorGetter> : IFormLinkContainerGetter, IPrintable, IFormLinkNullableGetter<TMajorGetter>
    where TMajorGetter : class, IMajorRecordGetter
{
    IFormLinkNullableGetter<TMajorGetter> Link { get; }
    
    uint? Alias { get; } 
    
    [MemberNotNullWhen(true, nameof(Link))]
    [MemberNotNullWhen(false, nameof(Alias))]
    bool UsesLink();
    
    [MemberNotNullWhen(false, nameof(Link))]
    [MemberNotNullWhen(true, nameof(Alias))]
    bool UsesAlias();
}

public interface IFormLinkOrAlias<TMajorGetter> : IFormLinkOrAliasGetter<TMajorGetter>, IFormLinkContainer, IFormLinkNullable<TMajorGetter>
    where TMajorGetter : class, IMajorRecordGetter
{
    new IFormLinkNullable<TMajorGetter> Link { get; }
    new uint? Alias { get; set; }
    void Clear();
}