﻿using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield.Internals;

namespace Mutagen.Bethesda.Starfield;

public partial class Weapon
{
    [Flags]
    public enum MajorFlag
    {
        NonPlayable = 0x0000_0004,
        HighResFirstPersonOnly = 0x2000_0000,
    }
    
    public enum FiringTypeEnum
    {
        SingleOrBinary,
        Burst,
        Automatic,
    }

    public const string ObjectModificationName = "TESObjectWEAP_InstanceData";
    
    public enum Property
    {
        AimAssistTemplate = RecordTypeInts.AA00,
        AimInnerConeAngleDegrees = RecordTypeInts.AA01,
        AimOuterConeAngleDegrees = RecordTypeInts.AA02,
        SteeringDegreesperSecond = RecordTypeInts.AA03,
        PitchScale = RecordTypeInts.AA04,
        InnerSteeringRing = RecordTypeInts.AA05,
        OuterSteeringRing = RecordTypeInts.AA06,
        Friction = RecordTypeInts.AA07,
        MoveFollowDegreesperSecond = RecordTypeInts.AA08,
        ADSSnapSteeringMultiplier = RecordTypeInts.AA09,
        ADSSnapSeconds = RecordTypeInts.AA10,
        ADSSnapConeAngleDegrees = RecordTypeInts.AA11,
        NoSteering = RecordTypeInts.AA12,
        BulletBendingConeAngleDegrees = RecordTypeInts.AA13,
        ADSSnapSteeringMultiplierInnerRing = RecordTypeInts.AA14,
        ADSSnapSteeringMultiplierOuterRing = RecordTypeInts.AA15,
        ADSMultiplierInnerConeAngleDegrees = RecordTypeInts.AA16,
        ADSMultiplierOuterConeAngleDegrees = RecordTypeInts.AA17,
        ADSMultiplierInnerSteeringRing = RecordTypeInts.AA18,
        ADSMultiplierOuterSteeringRing = RecordTypeInts.AA19,
        ADSMultiplierFriction = RecordTypeInts.AA20,
        ADSMultiplierSteeringDegreeperSecond = RecordTypeInts.AA21,
        AimAssistEnabled = RecordTypeInts.AA22,
        AmmoList = RecordTypeInts.ALIS,
        AmmoType = RecordTypeInts.AMMO,
        AmmoCapacity = RecordTypeInts.ACPT,
        NPCsUseAmmo = RecordTypeInts.AUSE,
        Resistance = RecordTypeInts.DRES,
        Skill = RecordTypeInts.DSKL,
        BoltChargeSeconds = RecordTypeInts.FBOL,
        HasStagedTrigger = RecordTypeInts.FHST,
        GeneralType = RecordTypeInts.GTYP,
        Value = RecordTypeInts.GVAL,
        Weight = RecordTypeInts.GWEI,
        AttackOxygenCost = RecordTypeInts.WAOC,
        Power = RecordTypeInts.PAVI,
        PowerBonus = RecordTypeInts.PBAV,
        FullRechargeTime = RecordTypeInts.PFCT,
        RechargeDelay = RecordTypeInts.PRCD,
        AccuracyBonus = RecordTypeInts.WACB,
        AimModelBaseStability = RecordTypeInts.WABS,
        AimModelTemplate = RecordTypeInts.WAIM,
        AimModelConeMinDegrees = RecordTypeInts.WCO1,
        AimModelConeMaxDegrees = RecordTypeInts.WCO2,
        AimModelConeIncreasePerShot = RecordTypeInts.WCO3,
        AimModelConeDecreasePerSec = RecordTypeInts.WCO4,
        AimModelConeDecreaseDelayPerSec = RecordTypeInts.WCO5,
        AimModelConeIronSightsMultiplier = RecordTypeInts.WCO6,
        AimModelRecoilDiminishSpringForce = RecordTypeInts.WRC1,
        AimModelRecoilDiminishSightsMult = RecordTypeInts.WRC2,
        AimModelRecoilMaxDegreePerShot = RecordTypeInts.WRC3,
        AimModelRecoilMultiplierHipFire = RecordTypeInts.WRC5,
        AimModelRecoilArcRotateDegrees = RecordTypeInts.WRC8,
        AimOpticalSightMarker = RecordTypeInts.WAOS,
        ActorValue = RecordTypeInts.WACV,
        AttackDelaySeconds = RecordTypeInts.WADL,
        BashDamage = RecordTypeInts.WSDM,
        Reach = RecordTypeInts.WREA,
        Stagger = RecordTypeInts.WSTG,
        AttackSeconds = RecordTypeInts.WATS,
        BlockMaterial = RecordTypeInts.WBMT,
        BoltAction = RecordTypeInts.WBOL,
        BashImpactDataSet = RecordTypeInts.WBSH,
        ChargingAttack = RecordTypeInts.WCAT,
        CriticalChargeBonus = RecordTypeInts.WCCB,
        ChargingReload = RecordTypeInts.WCHR,
        BurstDelaySeconds = RecordTypeInts.FBDS,
        BurstShots = RecordTypeInts.FBUR,
        CriticalDamageMultiplier = RecordTypeInts.WCDM,
        CriticalChanceIncMultiplier = RecordTypeInts.WCIM,
        DamagePhysical = RecordTypeInts.WDMG,
        DamageTypeValue = RecordTypeInts.WDTV,
        EnableMarkingTargetsRecon = RecordTypeInts.WEMT,
        Enchantment = RecordTypeInts.WENC,
        FullAuto = RecordTypeInts.FREP,
        FiringType = RecordTypeInts.FTYP,
        HasDualTrigger = RecordTypeInts.FHDT,
        ImpactDataset = RecordTypeInts.WIMP,
        Keyword = RecordTypeInts.WKEY,
        MinRange = RecordTypeInts.WMNR,
        MaxRange = RecordTypeInts.WMXR,
        ModCount = RecordTypeInts.WTMC,
        OverrideRateofFire = RecordTypeInts.FORF,
        ProjectileCount = RecordTypeInts.WNPR,
        ProjectileOverride = RecordTypeInts.WOPR,
        ReloadSpeed = RecordTypeInts.WRSP,
        Scope = RecordTypeInts.WSCP,
        ShotsperSecond = RecordTypeInts.WSPS,
        SightedTransitionSeconds = RecordTypeInts.WSTS,
        SoundLevel = RecordTypeInts.WSLV,
        Speed = RecordTypeInts.WSPD,
        ColorRemappingIndex = RecordTypeInts.WCOL,
        CriticalEffect = RecordTypeInts.WCRT,
        DisableShellCaseEject = RecordTypeInts.WDSH,
        CriticalEffectonDeathOnly = RecordTypeInts.WEDT,
        EquipSlot = RecordTypeInts.WEQS,
        FullPowerSeconds = RecordTypeInts.WFPS,
        FireSeconds = RecordTypeInts.WFSC,
        HitBehavior = RecordTypeInts.WHBV,
        HoldInput = RecordTypeInts.WHIP,
        MinorCrime = RecordTypeInts.WMCR,
        MinPowerperShot = RecordTypeInts.WMPS,
        Can = RecordTypeInts.WNDP,
        NonHostile = RecordTypeInts.WNHO,
        NonPlayable = RecordTypeInts.WNPL,
        OutofRangeDamageMult = RecordTypeInts.WOOR,
        OverrideShellCasing = RecordTypeInts.WOSC,
        PlayerOnly = RecordTypeInts.WPLY,
        VariableRangeApertureAcceleration = RecordTypeInts.VAAC,
        VariableRangeApertureDeceleration = RecordTypeInts.VADC,
        VariableRangeApertureInputRangeMin = RecordTypeInts.VAIN,
        VariableRangeApertureInputRangeMax = RecordTypeInts.VAIX,
        VariableRangeApertureValueRangeMin = RecordTypeInts.VAVN,
        VariableRangeApertureValueRangeMax = RecordTypeInts.VAVX,
        VariableRangeDistanceAcceleration = RecordTypeInts.VDAC,
        VariableRangeDistanceDeceleration = RecordTypeInts.VDDC,
        VariableRangeDistanceInputRangeMin = RecordTypeInts.VDIN,
        VariableRangeDistanceInputRangeMax = RecordTypeInts.VDIX,
        VariableRangeDistanceValueRangeMax = RecordTypeInts.VDVM,
        VariableRangeDistanceValueRangeMin = RecordTypeInts.VDVN,
        UseVariableRange = RecordTypeInts.VUSE,
        ReticleType = RecordTypeInts.WRET,
        Rank = RecordTypeInts.WRNK,
        ReloadSingle = RecordTypeInts.WRSG,
        SoundLevelMult = RecordTypeInts.WSLM,
        LayeredMaterialSwaps = RecordTypeInts.WSWL,
        ADSImageSpaceModifier = RecordTypeInts.ZIMG,
        ADSOffsetX = RecordTypeInts.ZOFX,
        ADSOffsetY = RecordTypeInts.ZOFY,
        ADSOffsetZ = RecordTypeInts.ZOFZ,
        Overlay = RecordTypeInts.ZOVL,
        FOVMult = RecordTypeInts.ZFOV,
        Template = RecordTypeInts.ZDTA,
    }
    
    IFormLinkNullableGetter<IObjectEffectGetter> IEnchantableGetter.ObjectEffect => this.ObjectEffect;
}