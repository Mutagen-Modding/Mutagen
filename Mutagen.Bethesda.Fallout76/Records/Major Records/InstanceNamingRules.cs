namespace Mutagen.Bethesda.Fallout76;

partial class InstanceNamingRules
{
    [Flags]
    public enum MajorFlag
    {
    }

    [Flags]
    public enum RuleTarget
    {
        None = 0x00,
        Armor = 0x1D,
        Actor = 0x2D,
        Furniture = 0x2A,
        Weapon = 0x2B
    }
}