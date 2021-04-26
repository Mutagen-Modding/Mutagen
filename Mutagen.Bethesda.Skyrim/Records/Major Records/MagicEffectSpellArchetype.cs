using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MagicEffectSpellArchetype
    {
        public FormLink<ISpell> Association => new FormLink<ISpell>(this.AssociationKey);

        public MagicEffectSpellArchetype()
            : base(TypeEnum.Cloak)
        {
        }
    }
}
