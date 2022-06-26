namespace Mutagen.Bethesda.Fallout4;

[Flags]
public enum SpellDataFlag
{
    ManualCostCalc = 0x0000_0001,
    PCStartSpell = 0x0002_0000,
    InstantCast = 0x0004_0000,
    AreaEffectIgnoresLOS = 0x0008_0000,
    IgnoreResistance = 0x0010_0000,
    NoAbsorbOrReflect = 0x0020_0000,
    NoDualCastModification = 0x0080_0000,
}