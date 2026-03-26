namespace Mutagen.Bethesda.Fallout76;

public partial class Hazard
{
    [Flags]
    public enum MajorFlag
    {
    }

    [Flags]
    public enum Flag
    {
        AffectsPlayerOnly = 0x01,
        InheritDurationFromSpawnSpell = 0x02,
        AlignToImpactNormal = 0x04,
        InheritRadiusFromSpawnSpell = 0x08,
        DropToGround = 0x10,
        TaperEffectivenessByProximity = 0x20
    }
}