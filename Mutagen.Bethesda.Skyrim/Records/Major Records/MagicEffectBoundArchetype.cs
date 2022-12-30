using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim;

public partial class MagicEffectBoundArchetype
{
    public FormLink<IBindableEquipmentGetter> Association => this.AssociationKey.ToLink<IBindableEquipmentGetter>();

    IFormLink<IBindableEquipmentGetter> IMagicEffectBoundArchetype.Association => this.Association;
    IFormLinkGetter<IBindableEquipmentGetter> IMagicEffectBoundArchetypeGetter.Association => this.Association;

    public MagicEffectBoundArchetype()
        : base(TypeEnum.Bound)
    {
    }
}

public partial interface IMagicEffectBoundArchetype
{
    new IFormLink<IBindableEquipmentGetter> Association { get; }
}

public partial interface IMagicEffectBoundArchetypeGetter
{
    IFormLinkGetter<IBindableEquipmentGetter> Association { get; }
}

partial class MagicEffectBoundArchetypeBinaryOverlay
{
    public IFormLinkGetter<IBindableEquipmentGetter> Association => this.AssociationKey.ToLink<IBindableEquipmentGetter>();
}