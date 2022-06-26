namespace Mutagen.Bethesda.Fallout4;

partial class EquipType
{
    [Flags]
    public enum Flags
    {
        UseAllParents = 0x01,
        ParentsOptional = 0x02,
        ItemSlot = 0x04,
    }
}
