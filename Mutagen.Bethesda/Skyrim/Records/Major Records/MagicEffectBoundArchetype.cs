using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MagicEffectBoundArchetype
    {
        public FormLink<IBoundableEquipment> Association => new FormLink<IBoundableEquipment>(this.AssociationKey);

        public MagicEffectBoundArchetype()
            : base(TypeEnum.Bound)
        {
        }
    }
}
