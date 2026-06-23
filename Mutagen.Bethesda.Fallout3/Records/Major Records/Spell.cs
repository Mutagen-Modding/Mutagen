using System.Diagnostics;

namespace Mutagen.Bethesda.Fallout3;

public partial class Spell
{
    public enum SpellType
    {
        ActorEffect = 0,
        Disease = 1,
        Power = 2,
        LesserPower = 3,
        Ability = 4,
        Poison = 5,
        Addiction = 10,
    }

    [Flags]
    public enum SpellFlag
    {
        NoAutoCalc = 0x01,
        ImmuneToSilence1 = 0x02,
        PCStartEffect = 0x04,
        ImmuneToSilence2 = 0x08,
        AreaEffectIgnoresLOS = 0x10,
        ScriptEffectAlwaysApplies = 0x20,
        DisableAbsorbReflect = 0x40,
        ForceTouchExplode = 0x80,
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlyList<IEffectGetter> IHasEffectsGetter.Effects => Effects;
}
