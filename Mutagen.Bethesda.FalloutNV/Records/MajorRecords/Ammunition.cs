namespace Mutagen.Bethesda.FalloutNV;

public partial class Ammunition
{
    [Flags]
    public enum MajorFlag
    {
        NonPlayable = 0x4
    }

    [Flags]
    public enum Flag
    {
        IgnoresNormalWeaponResistance = 0x01,
        NonPlayable = 0x02,
    }
}
