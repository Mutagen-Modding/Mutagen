namespace Mutagen.Bethesda.Oblivion;

public partial class MagicEffect
{
    [Flags]
    public enum MagicFlag
    {
        Hostile = 0x00000001,
        Recover = 0x00000002,
        Detrimental = 0x00000004,
        MagnitudePercent = 0x00000008,
        Self = 0x00000010,
        Touch = 0x00000020,
        Target = 0x00000040,
        NoDuration = 0x00000080,
        NoMagnitude = 0x00000100,
        NoArea = 0x00000200,
        FXPersist = 0x00000400,
        Spellmaking = 0x00000800,
        Enchanting = 0x00001000,
        NoIngredient = 0x00002000,
        UseWeapon = 0x00010000,
        UseArmor = 0x00020000,
        UseCreature = 0x00040000,
        UseSkill = 0x00080000,
        UseAttribute = 0x00100000,
        UseActorValue = 0x01000000,
        // Spray if specified alone.  Fog if specified along with Bolt flag
        SprayProjectileOrFog = 0x02000000,
        BoltProjectile = 0x04000000,
        NoHitEffect = 0x08000000,
    }
}