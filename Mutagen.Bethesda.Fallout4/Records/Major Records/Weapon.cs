using System;

namespace Mutagen.Bethesda.Fallout4;

public partial class Weapon
{
    [Flags]
    public enum MajorFlag
    {
        NonPlayable = 0x0000_0004,
        HighResFirstPersonOnly = 0x2000_0000,
    }

    public enum HitBehavior
    {
        Normal,
        DismemberOnly,
        ExplodeOnly,
        NoDismemberOrExplode
    }

    [Flags]
    public enum Flag
    {
        PlayerOnly = 0x0000_0001,
        NpcsUseAmmo = 0x0000_0002,
        NoJamAfterReload = 0x0000_0004,
        ChargingReload = 0x0000_0008,
        MinorCrime = 0x0000_0010,
        FixedRange = 0x0000_0020,
        NotUsedInNormalCombat = 0x0000_0040,
        CritEffectOnDeath = 0x0000_0100,
        ChargingAttack = 0x0000_0200,
        HoldInputToPower = 0x0000_0800,
        NonHostile = 0x0000_1000,
        BoundWeapon = 0x0000_2000,
        IgnoresNormalWeaponResistence = 0x0000_4000,
        Automatic = 0x0000_8000,
        RepeatableSingleFire = 0x0001_0000,
        CannotDrop = 0x0002_0000,
        HideBackpack = 0x0004_0000,
        EmbeddedWeapon = 0x0008_0000,
        NotPlayable = 0x0010_0000,
        HasScope = 0x0020_0000,
        BoltAction = 0x0040_0000,
        SecondaryWeapon = 0x0080_0000,
        DisableShells = 0x0100_0000,
    }

    public enum AnimationTypes
    {
        HandToHandMelee,
        OneHandSword,
        OneHandDagger,
        OneHandAxe,
        OneHandMace,
        TwoHandSword,
        TwoHandAxe,
        Bow,
        Staff,
        Gun,
        Grenade,
        Mine,
    }

    public enum PatternType
    {
        Constant,
        Square,
        Triangle,
        Sawtooth,
    }

    public enum MeleeSpeeds
    {
        VerySlow,
        Slow,
        Medium,
        Fast,
        VeryFast,
    }

    public enum Property
    {
        Speed = 0,
        Reach = 1,
        MinRange = 2,
        MaxRange = 3,
        AttackDelaySec = 4,
        OutOfRangeDamageMult = 6,
        SecondaryDamage = 7,
        CriticalChargeBonus = 8,
        HitBehaviour = 9,
        Rank = 10,
        AmmoCapacity = 12,
        Type = 15,
        IsPlayerOnly = 16,
        NPCsUseAmmo = 17,
        HasChargingReload = 18,
        IsMinorCrime = 19,
        IsFixedRange = 20,
        HasEffectOnDeath = 21,
        HasAlternateRumble = 22,
        IsNonHostile = 23,
        IgnoreResist = 24,
        IsAutomatic = 25,
        CantDrop = 26,
        IsNonPlayable = 27,
        AttackDamage = 28,
        Value = 29,
        Weight = 30,
        Keywords = 31,
        AimModel = 32,
        AimModelMinConeDegrees = 33,
        AimModelMaxConeDegrees = 34,
        AimModelConeIncreasePerShot = 35,
        AimModelConeDecreasePerSec = 36,
        AimModelConeDecreaseDelayMs = 37,
        AimModelConeSneakMultiplier = 38,
        AimModelRecoilDiminishSpringForce = 39,
        AimModelRecoilDiminishSightsMult = 40,
        AimModelRecoilMaxDegPerShot = 41,
        AimModelRecoilMinDegPerShot = 42,
        AimModelRecoilHipMult = 43,
        AimModelRecoilShotsForRunaway = 44,
        AimModelRecoilArcDeg = 45,
        AimModelRecoilArcRotateDeg = 46,
        AimModelConeIronSightsMultiplier = 47,
        HasScope = 48,
        ZoomDataFOVMult = 49,
        FireSeconds = 50,
        NumProjectiles = 51,
        AttackSound = 52,
        AttackSound2D = 53,
        AttackLoop = 54,
        AttackFailSound = 55,
        IdleSound = 56,
        EquipSound = 57,
        UnEquipSound = 58,
        SoundLevel = 59,
        ImpactDataSet = 50,
        Ammo = 61,
        CritEffect = 62,
        BashImpactDataSet = 63,
        BlockMaterial = 64,
        Enchantments = 65,
        AimModelBaseStability = 66,
        ZoomData = 67,
        ZoomDataOverlay = 68,
        ZoomDataImageSpace = 69,
        ZoomDataCameraOffsetX = 70,
        ZoomDataCameraOffsetY = 71,
        ZoomDataCameraOffsetZ = 72,
        EquipSlot = 73,
        SoundLevelMult = 74,
        NPCAmmoList = 75,
        ReloadSpeed = 76,
        DamageTypeValues = 77,
        AccuracyBonus = 78,
        AttackActionPointCost = 79,
        OverrideProjectile = 80,
        HasBoltAction = 81,
        StaggerValue = 82,
        SightedTransitionSeconds = 83,
        FullPowerSeconds = 84,
        HoldInputToPower = 85,
        HasRepeatableSingleFire = 86,
        MinPowerPerShot = 87,
        ColorRemappingIndex = 88,
        MaterialSwaps = 89,
        CriticalDamageMult = 90,
        FastEquipSound = 91,
        DisableShells = 92,
        HasChargingAttack = 93,
        ActorValues = 94,
    }
}