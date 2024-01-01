namespace Mutagen.Bethesda.Starfield;

public partial class Ammunition
{
    [Flags]
    public enum MajorFlag
    {
        NonPlayable = 0x0000_0004,
    }

    [Flags]
    public enum Flag
    {
        IgnoresNormalWeaponResistance = 0x01,
        NonPlayable = 0x02,
        HasCountBased3d = 0x04
    }
}