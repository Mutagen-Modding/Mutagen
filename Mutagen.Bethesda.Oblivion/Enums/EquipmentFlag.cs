namespace Mutagen.Bethesda.Oblivion;

[Flags]
public enum EquipmentFlag
{
    HideRings = 0x00010000,
    HideAmulets = 0x00020000,
    NonPlayable = 0x00400000,
    HeavyArmor = 0x00800000
}