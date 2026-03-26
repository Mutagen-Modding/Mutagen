namespace Mutagen.Bethesda.Fallout76;

partial class EquipType
{
    [Flags]
    public enum MajorFlag
    {
    }

    [Flags]
    public enum Flags
    {
        UseAllParents = 0x01,
        ParentsOptional = 0x02,
        ItemSlot = 0x04,
    }
}
