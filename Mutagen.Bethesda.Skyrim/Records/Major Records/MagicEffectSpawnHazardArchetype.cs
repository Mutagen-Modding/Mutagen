using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MagicEffectSpawnHazardArchetype
    {
        public FormLink<IHazard> Association => new FormLink<IHazard>(this.AssociationKey);

        public MagicEffectSpawnHazardArchetype()
            : base(TypeEnum.SpawnHazard)
        {
        }
    }
}
