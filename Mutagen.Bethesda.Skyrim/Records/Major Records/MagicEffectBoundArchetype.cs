using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MagicEffectBoundArchetype
    {
        public FormLink<IBindableEquipmentGetter> Association => new FormLink<IBindableEquipmentGetter>(this.AssociationKey);

        public MagicEffectBoundArchetype()
            : base(TypeEnum.Bound)
        {
        }
    }
}
