
using Mutagen.Bethesda.Fallout3.Internals;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Fallout3;

public partial class Weapon
{
    [Flags]
    public enum MajorFlag
    {
        QuestItem = 0x0000_0400,
    }

    public enum WeaponAnimationType
    {
        HandToHand = 0,
        Melee1Hand = 1,
        Melee2Hand = 2,
        PistolBallistic1Hand = 3,
        PistolEnergy1Hand = 4,
        RifleBallistic2Hand = 5,
        RifleAutomatic2Hand = 6,
        RifleEnergy2Hand = 7,
        Handle2Hand = 8,
        Launcher2Hand = 9,
        GrenadeThrow1Hand = 10,
        LandMine1Hand = 11,
        MineDrop1Hand = 12,
    }

    [Flags]
    public enum WeaponFlag : uint
    {
        // Lower byte (Flags1 in binary, offset 12, 1 byte)
        IgnoresNormalWeaponResistance = 0x0000_0001,
        IsAutomatic = 0x0000_0002,
        HasScope = 0x0000_0004,
        CantDrop = 0x0000_0008,
        HideBackpack = 0x0000_0010,
        EmbeddedWeapon = 0x0000_0020,
        DontUse1stPersonISAnimations = 0x0000_0040,
        NonPlayable = 0x0000_0080,
        // Upper bits (Flags2 in binary, offset 56, 4 bytes, shifted left 8)
        PlayerOnly = 0x0000_0100,
        NpcsUseAmmo = 0x0000_0200,
        NoJamAfterReload = 0x0000_0400,
        OverrideActionPoints = 0x0000_0800,
        MinorCrime = 0x0000_1000,
        RangeFixed = 0x0000_2000,
        NotUsedInNormalCombat = 0x0000_4000,
        OverrideDamageToWeaponMult = 0x0000_8000,
        DontUse3rdPersonISAnimations = 0x0001_0000,
        ShortBurst = 0x0002_0000,
        RumbleAlternate = 0x0004_0000,
        LongBurst = 0x0008_0000,
        ScopeHasNightVision = 0x0010_0000,
        ScopeFromMod = 0x0020_0000,
    }

    public enum WeaponOnHit
    {
        NormalFormulaBehavior = 0,
        DismemberOnly = 1,
        ExplodeOnly = 2,
        NoDismemberExplode = 3,
    }

    public enum WeaponRumblePattern
    {
        Constant = 0,
        Square = 1,
        Triangle = 2,
        Sawtooth = 3,
    }

    public enum ReloadAnim : byte
    {
        ReloadA = 0,
        ReloadB = 1,
        ReloadC = 2,
        ReloadD = 3,
        ReloadE = 4,
        ReloadF = 5,
        ReloadG = 6,
        ReloadH = 7,
        ReloadI = 8,
        ReloadJ = 9,
        ReloadK = 10,
        ReloadL = 11,
        ReloadM = 12,
        ReloadN = 13,
        ReloadO = 14,
        ReloadP = 15,
        ReloadQ = 16,
        ReloadR = 17,
        ReloadS = 18,
        ReloadW = 19,
        ReloadX = 20,
        ReloadY = 21,
        ReloadZ = 22,
        None = 255,
    }

    public enum AttackAnim : byte
    {
        AttackLeft = 26,
        AttackRight = 32,
        Attack3 = 38,
        Attack4 = 44,
        Attack5 = 50,
        Attack6 = 56,
        Attack7 = 62,
        Attack8 = 68,
        AttackLoop = 74,
        AttackSpin = 80,
        AttackSpin2 = 86,
        PlaceMine = 97,
        PlaceMine2 = 103,
        AttackThrow = 109,
        AttackThrow2 = 115,
        AttackThrow3 = 121,
        AttackThrow4 = 127,
        AttackThrow5 = 133,
        Attack9 = 144,
        AttackThrow6 = 150,
        AttackThrow7 = 156,
        AttackThrow8 = 162,
        Default = 255,
    }

    public enum EmbeddedWeaponAV : byte
    {
        Perception = 0,
        Endurance = 1,
        LeftAttack = 2,
        RightAttack = 3,
        LeftMobility = 4,
        RightMobility = 5,
        Brain = 6,
    }

    public enum ModEffect
    {
        None = 0,
        IncreaseWeaponDamage = 1,
        IncreaseClipCapacity = 2,
        DecreaseSpread = 3,
        DecreaseWeight = 4,
        RegenerateAmmoShots = 5,
        RegenerateAmmoSeconds = 6,
        DecreaseEquipTime = 7,
        IncreaseRateOfFire = 8,
        IncreaseProjectileSpeed = 9,
        IncreaseMaxCondition = 10,
        Silence = 11,
        SplitBeam = 12,
        VatsBonus = 13,
        IncreaseZoom = 14,
        Suppressor = 16,
    }

}
