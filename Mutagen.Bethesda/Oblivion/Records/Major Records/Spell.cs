using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Spell
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequiredGetter.Name => this.Name ?? string.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequired.Name
        {
            get => this.Name ?? string.Empty;
            set => this.Name = value;
        }

        public enum SpellType
        {
            Spell = 0,
            Disease = 1,
            Power = 2,
            LesserPower = 3,
            Ability = 4,
            Poison = 5
        }

        public enum SpellLevel
        {
            Apprentice = 0,
            Novice = 1,
            Journeyman = 2,
            Expert = 3,
            Master = 4
        }

        [Flags]
        public enum SpellFlag
        {
            ManualSpellCost = 0x01,
            ImmuneToSilence1 = 0x02,
            PlayerStartSpell = 0x04,
            ImmuneToSilence2 = 0x08,
            AoeIgnoresLos = 0x10,
            ScriptEffectAlwaysApplies = 0x20,
            DisallowAbsorbAndReflect = 0x40,
            TouchSpellExplodesWithoutTarget = 0x80,
        }
    }

    namespace Internals
    {
        public partial class SpellBinaryOverlay
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            String INamedRequiredGetter.Name => this.Name ?? string.Empty;
        }
    }
}
