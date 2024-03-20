namespace Mutagen.Bethesda.Starfield;

partial class InstanceNamingRules
{
    [Flags]
    public enum RuleTarget
    {
        None = 0x00,
        Armor = 0x1D,
        Actor = 0x2D,
        Furniture = 0x2A,
        Weapon = 0x2B,
        AdjectiveArmor = 0x22,
        AdjectiveWeapon = 0x30,
        CreaturePrefixSuffix = 0x32,
    }
}