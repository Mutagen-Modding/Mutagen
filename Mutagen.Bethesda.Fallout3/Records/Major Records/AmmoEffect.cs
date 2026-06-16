namespace Mutagen.Bethesda.Fallout3;

public partial class AmmoEffect
{
    public enum AmmoEffectType
    {
        DamageMod = 0,
        DRMod = 1,
        DTMod = 2,
        SpreadMod = 3,
        WeaponConditionMod = 4,
        FatigueMod = 5,
    }

    public enum AmmoEffectOperation
    {
        Add = 0,
        Multiply = 1,
        Subtract = 2,
    }
}
