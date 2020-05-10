using System;
using System.Collections.Generic;
using System.Text;

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
