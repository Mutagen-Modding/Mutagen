namespace Mutagen.Bethesda.Oblivion;

public partial class AmmunitionData
{
    [Flags]
    public enum Flag
    {
        IgnoresNormalWeaponResistance = 0x01
    }
}