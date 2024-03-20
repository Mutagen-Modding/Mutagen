namespace Mutagen.Bethesda.Starfield;

public partial class Spell
{
    [Flags]
    public enum Flag
    {
        ManualCostCalc = 0x0000_0001,
        InstantCast = 0x0004_0000,
        AreaEffectIgnoresLOS = 0x0008_0000,
        IgnoreResistance = 0x0010_0000,
        NoAbsorbOrReflect = 0x0020_0000,
    }
    
    [Flags]
    public enum SpellType
    {
        Spell = 0,
        Disease = 1,
        Power = 2,
        LesserPower = 3,
        Ability = 4,
        Poison = 5,
        Addiction = 10,
        Voice = 11,
    }
}