using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MagicEffectWerewolfArchetype
    {
        public FormLink<IRace> Association => new FormLink<IRace>(this.AssociationKey);

        public MagicEffectWerewolfArchetype()
            : base(TypeEnum.Werewolf)
        {
        }
    }
}
