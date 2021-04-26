using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MagicEffectLightArchetype
    {
        public FormLink<ILight> Association => new FormLink<ILight>(this.AssociationKey);

        public MagicEffectLightArchetype()
            : base(TypeEnum.Light)
        {
        }
    }
}
