using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MagicEffectGuideArchetype
    {
        public FormLink<IHazard> Association => new FormLink<IHazard>(this.AssociationKey);

        public MagicEffectGuideArchetype()
            : base(TypeEnum.Guide)
        {
        }
    }
}
