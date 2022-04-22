using System;

namespace Mutagen.Bethesda.Oblivion;

public partial class Weapon
{
    public enum WeaponType
    {
        BladeOneHand,
        BladeTwoHand,
        BluntOneHand,
        BluntTwoHand,
        Bow,
        Staff,
    }

    [Flags]
    public enum WeaponFlag
    {
        IgnoresNormalWeaponResistance = 0x01
    }
}