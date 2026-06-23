namespace Mutagen.Bethesda.Fallout3;

public partial class CombatStyle
{
    [Flags]
    public enum Flag : ushort
    {
        ChooseAttackUsingChance = 0x0001,
        MeleeAlertOk = 0x0002,
        FleeBasedOnPersonalSurvival = 0x0004,
        IgnoreThreats = 0x0010,
        IgnoreDamagingSelf = 0x0020,
        IgnoreDamagingGroup = 0x0040,
        IgnoreDamagingSpectators = 0x0080,
        CannotUseStealthboy = 0x0100,
    }

    public enum WeaponRestriction
    {
        None,
        MeleeOnly,
        RangedOnly,
    }
}
