namespace Mutagen.Bethesda.Starfield;

partial class Hazard
{
    [Flags]
    public enum Flag
    {
        AffectsPlayerOnly = 0x01,
        InheritDurationFromSpawnSpell = 0x02,
        AlignToImpactNormal = 0x04,
        InheritRadiusFromSpawnSpell = 0x08,
        DropToGround = 0x10,
        TaperEffectivenessByProximity = 0x20,
        GravityPointSource = 0x40,
        GravityOverride = 0x80
    }
}