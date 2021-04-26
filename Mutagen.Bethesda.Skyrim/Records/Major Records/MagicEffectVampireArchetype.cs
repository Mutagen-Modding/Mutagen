using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MagicEffectVampireArchetype
    {
        public FormLink<IRace> Association => new FormLink<IRace>(this.AssociationKey);

        public MagicEffectVampireArchetype()
            : base(TypeEnum.VampireLord)
        {
        }
    }
}
