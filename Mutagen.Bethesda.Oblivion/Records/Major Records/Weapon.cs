namespace Mutagen.Bethesda.Oblivion;

public partial class Weapon
{
    public enum WeaponType
    {
        BladeOneHand,
        BladeTwoHand,
        BluntOneHand,
        BluntTwoHand,
        Staff,
        Bow,
    }

    [Flags]
    public enum WeaponFlag
    {
        IgnoresNormalWeaponResistance = 0x01
    }
}