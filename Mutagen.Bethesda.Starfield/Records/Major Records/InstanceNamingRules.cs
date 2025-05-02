using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Starfield.Internals;

namespace Mutagen.Bethesda.Starfield;

partial class InstanceNamingRules
{
    public enum Operator
    {
        GreaterThanOrEqualTo = 0,
        GreaterThan = 1,
        LessThanOrEqualTo = 2,
        LessThan = 3,
        EqualTo = 4,
    }
}

internal partial class InstanceNamingRulesBinaryCreateTranslation
{
    public enum TargetType
    {
        None,
        Armor = 0x22,
        Container = 0x24,
        Flora = 0x2E,
        Furniture = 0x2F,
        Weapon = 0x30,
        Actor = 0x32,
    }
    
    public static partial ParseResult FillBinaryRuleSetParserCustom(
        MutagenFrame frame,
        IInstanceNamingRulesInternal item,
        PreviousParse lastParsed)
    {
        var unam = frame.ReadSubrecord(RecordTypes.UNAM);
        frame = frame.SpawnAll();
        var type = (TargetType)unam.AsInt32();
        switch (type)
        {
            case TargetType.Armor:
                item.Rules = ArmorInstanceNamingRules.CreateFromBinary(frame);
                break;
            case TargetType.None:
                item.Rules = NoInstanceNamingRules.CreateFromBinary(frame);
                break;
            case TargetType.Container:
                item.Rules = ContainerInstanceNamingRules.CreateFromBinary(frame);
                break;
            case TargetType.Flora:
                item.Rules = FloraInstanceNamingRules.CreateFromBinary(frame);
                break;
            case TargetType.Furniture:
                item.Rules = FurnitureInstanceNamingRules.CreateFromBinary(frame);
                break;
            case TargetType.Weapon:
                item.Rules = WeaponInstanceNamingRules.CreateFromBinary(frame);
                break;
            case TargetType.Actor:
                item.Rules = ActorInstanceNamingRules.CreateFromBinary(frame);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return (int)InstanceNamingRules_FieldIndex.Rules;
    }
}

public partial class InstanceNamingRulesBinaryWriteTranslation
{
    public static partial void WriteBinaryRuleSetParserCustom(
        MutagenWriter writer,
        IInstanceNamingRulesGetter item)
    {
        var ruleSet = item.Rules;
        var t = ruleSet switch
        {
            INoInstanceNamingRulesGetter => InstanceNamingRulesBinaryCreateTranslation.TargetType.None,
            IArmorInstanceNamingRulesGetter => InstanceNamingRulesBinaryCreateTranslation.TargetType.Armor,
            IContainerInstanceNamingRulesGetter => InstanceNamingRulesBinaryCreateTranslation.TargetType.Container,
            IFloraInstanceNamingRulesGetter => InstanceNamingRulesBinaryCreateTranslation.TargetType.Flora,
            IFurnitureInstanceNamingRulesGetter => InstanceNamingRulesBinaryCreateTranslation.TargetType.Furniture,
            IWeaponInstanceNamingRulesGetter => InstanceNamingRulesBinaryCreateTranslation.TargetType.Weapon,
            IActorInstanceNamingRulesGetter => InstanceNamingRulesBinaryCreateTranslation.TargetType.Actor,
            _ => throw new ArgumentOutOfRangeException()
        };
        using (HeaderExport.Subrecord(writer, RecordTypes.UNAM))
        {
            writer.Write((int)t);
        }
        ruleSet.WriteToBinary(writer);
    }
}

internal partial class InstanceNamingRulesBinaryOverlay
{
    public IAInstanceNamingRulesGetter Rules { get; private set; } = null!;
    
    public partial ParseResult RuleSetParserCustomParse(
        OverlayStream stream,
        int offset,
        PreviousParse lastParsed)
    {
        var unam = stream.ReadSubrecord(RecordTypes.UNAM);
        var type = (InstanceNamingRulesBinaryCreateTranslation.TargetType)unam.AsInt32();
        switch (type)
        {
            case InstanceNamingRulesBinaryCreateTranslation.TargetType.Armor:
                Rules = ArmorInstanceNamingRulesBinaryOverlay.ArmorInstanceNamingRulesFactory(stream, _package);
                break;
            case InstanceNamingRulesBinaryCreateTranslation.TargetType.None:
                Rules = NoInstanceNamingRulesBinaryOverlay.NoInstanceNamingRulesFactory(stream, _package);
                break;
            case InstanceNamingRulesBinaryCreateTranslation.TargetType.Container:
                Rules = ContainerInstanceNamingRulesBinaryOverlay.ContainerInstanceNamingRulesFactory(stream, _package);
                break;
            case InstanceNamingRulesBinaryCreateTranslation.TargetType.Flora:
                Rules = FloraInstanceNamingRulesBinaryOverlay.FloraInstanceNamingRulesFactory(stream, _package);
                break;
            case InstanceNamingRulesBinaryCreateTranslation.TargetType.Furniture:
                Rules = FurnitureInstanceNamingRulesBinaryOverlay.FurnitureInstanceNamingRulesFactory(stream, _package);
                break;
            case InstanceNamingRulesBinaryCreateTranslation.TargetType.Weapon:
                Rules = WeaponInstanceNamingRulesBinaryOverlay.WeaponInstanceNamingRulesFactory(stream, _package);
                break;
            case InstanceNamingRulesBinaryCreateTranslation.TargetType.Actor:
                Rules = ActorInstanceNamingRulesBinaryOverlay.ActorInstanceNamingRulesFactory(stream, _package);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return (int)InstanceNamingRules_FieldIndex.Rules;
    }
}

public partial class NoInstanceNamingRules
{
    public enum Target
    {
    }
}

public partial class ArmorInstanceNamingRules
{
    public enum Target
    {
        Enchantments = 0,
        BashImpactDataSet = 1,
        BlockMaterial = 2,
        Keywords = 3,
        Weight = 4,
        Value = 5,
        Rating = 6,
        AddonIndex = 7,
        BodyPart = 8,
        DamageTypeValues = 9,
        ActorValues = 10,
        Health = 11,
        ColorRemappingIndex = 12,
        ModCount = 13,
        LayeredMaterialSwaps = 14,
        ActorValues2 = 15,
    }
}

public partial class ContainerInstanceNamingRules
{
    public enum Target
    {
        Keywords = 0,
    }
}

public partial class FloraInstanceNamingRules
{
    public enum Target
    {
        Keywords = 0,
        LayeredMaterialSwaps = 1,
    }
}

public partial class FurnitureInstanceNamingRules
{
    public enum Target
    {
    }
}

public partial class WeaponInstanceNamingRules
{
    public enum Target
    {
        Speed = 0,
        Reach = 1,
        MinRange = 2,
        MaxRange = 3,
        AttackDelaySeconds = 4,
        ModCount = 5,
        OutOfRangeDamageMult = 6,
        BashDamage = 7,
        CriticalChargeBonus = 8,
        HitBehavior = 9,
        Rank = 10,
        AmmoCapacity = 12,
        Type = 15,
        PlayerOnly = 16,
        NPCUseAmmo = 17,
        ChargingReload = 18,
        MinorCrime = 19,
        VariableRange = 20,
        CriticalEffectOnDeathOnly = 21,
        NonHostile = 23,
        CantDrop = 26,
        NonPlayable = 27,
        AttackDamage = 28,
        Value = 29,
        Weight = 30,
        Keywords = 31,
        AimModelTemplate = 32,
        AimModelMinConeDegrees = 33,
        AimModelMaxConeDegrees = 34,
        AimModelConeIncreasePerShot = 35,
        AimModelConeDecreasePerSec = 36,
        AimModelConeDecreaseDelaySec = 37,
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
        ReticleType = 49,
        AimDownSightWorldFOVMult = 50,
        FireSeconds = 51,
        NumProjectiles = 52,
        SoundLevel = 53,
        ImpactDataSet = 54,
        Ammo = 55,
        CritEffect = 56,
        BashImpactDataSet = 57,
        BlockMaterial = 58,
        Enchantments = 59,
        AimModelBaseStability = 60,
        AimDownSightTemplate = 61,
        ZoomDataOverlay = 62,
        AimDownSightDataImageSpace = 63,
        AimDownSightFPGeometryOffsetX = 64,
        AimDownSightFPGeometryOffsetY = 65,
        AimDownSightFPGeometryOffsetZ = 66,
        EquipSlots = 67,
        SoundLevelMult = 68,
        NPCAmmoList = 69,
        ReloadSpeed = 70,
        DamageTypeValues = 71,
        AccuracyBonus = 72,
        AttackOxygenCost = 73,
        OverrideProjectile = 74,
        OverrideShellCasing = 75,
        BoltAction = 76,
        StaggerValue = 77,
        SightedTransitionSeconds = 78,
        FullPowerSeconds = 79,
        HoldInput = 80,
        MinPowerPerShot = 82,
        ColorRemappingIndex = 83,
        LayeredMaterialSwaps = 85,
        CriticalDamageMult = 86,
        DisableShellCaseEject = 87,
        ChargingAttack = 88,
        ActorValues = 89,
        ReloadSingle = 91,
        UsePower = 93,
        AttackSeconds = 95,
        FiringType = 96,
        BurstShots = 97,
        BoltChargeSeconds2 = 98,
        RepeateableFire = 99,
        Resistance = 100,
        Skill = 101,
        PowerAVIF = 102,
        FullRechargeTime = 103,
        BoltChargeSeconds1 = 104,
        ConsumeAmmo = 105,
        VariableRangeApertureValueRangeMin = 106,
        VariableRangeApertureValueRangeMax = 107,
        VariableRangeApertureInputRangeMin = 108,
        VariableRangeApertureInputRangeMax = 109,
        VariableRangeApertureAcceleration = 110,
        VariableRangeApertureDeceleration = 111,
        VariableRangeDistanceValueRangeMin = 112,
        VariableRangeDistanceValueRangeMax = 113,
        VariableRangeDistanceInputRangeMin = 114,
        VariableRangeDistanceInputRangeMax = 115,
        VariableRangeDistanceAcceleration = 116,
        VariableRangeDistanceDeceleration = 117,
        PowerBonusAVIF = 118,
        AimAssistTemplate = 119,
        InnerConeAngleDegrees = 120,
        OuterConeAngleDegrees = 121,
        SteeringDegPerSec = 122,
        PitchScale = 123,
        InnerSteeringRing = 124,
        OuterSteeringRing = 125,
        Friction = 126,
        MoveFollowDegPerSec = 127,
        ADSSnapSteeringMultiplier = 128,
        ADSSnapSeconds = 129,
        ADSSnapConeAngleDegrees = 130,
        NoSteering = 131,
        BulletBendingConeAngleDegrees = 132,
        ADSSnapSteeringMultiplierInnerRing = 133,
        ADSSnapSteeringMultiplierOuterRing = 134,
        ADSMultiplierInnerConeAngleDegrees = 135,
        ADSMultiplierOuterConeAngleDegrees = 136,
        ADSMultiplierInnerSteeringRing = 137,
        ADSMultiplierOuterSteeringRing = 138,
        ADSMultiplierFriction = 139,
        ADSMultiplierSteeringRingDegPerSec = 140,
        AimAssist = 141,
        AimOpticalSightModel = 142,
        HasStagedTrigger = 143,
        HasDualTrigger = 144,
        BurstDelaySeconds = 145,
        OverrideRateOfFire = 146,
        CritChanceIncMult = 147,
        ShotsPerSecond = 148,
        EnableMarkingTargets = 149,
    }
}

public partial class ActorInstanceNamingRules
{
    public enum Target
    {
        Keywords = 0,
        ForcedInventory = 1,
        XPOffset = 2,
        Enchantments = 3,
        ColorRemappingIndex = 4,
        LayeredMaterialSwaps = 6,
        RaceChange = 7,
        SkinChange = 8,
        VoiceChange = 9,
        HeightMinMax = 10,
        CombatStyle = 11,
        RaceOverride = 12,
        Spells = 13,
        Perks = 14,
        Factions = 15,
        ActorValues = 16,
        Packages = 17,
        DisplayName = 18,
        ReactionRadius = 19,
        GroupFaction = 20,
        AiData = 21,
        NpcRaceOverride = 22,
    }
}
