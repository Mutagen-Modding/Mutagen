using Mutagen.Bethesda.Plugins;
using System;

namespace Mutagen.Bethesda.Fallout76;

public partial class Npc
{
    [Flags]
    public enum MajorFlag
    {
        BleedoutOverride = 0x2000_0000
    }

    [Flags]
    public enum Flag : uint
    {
        Female = 0x0000_0001,
        Essential = 0x0000_0002,
        IsCharGenFacePreset = 0x0000_0004,
        Respawn = 0x0000_0008,
        AutoCalcStats = 0x0000_0010,
        Unique = 0x0000_0020,
        DoesntAffectStealthMeter = 0x0000_0040,
        CalcForEachTemplate = 0x0000_0200,
        Protected = 0x0000_0800,
        Summonable = 0x0000_4000,
        DoesNotBleed = 0x0001_0000,
        BleedoutOverride = 0x0004_0000,
        OppositeGenderAnims = 0x0008_0000,
        SimpleActor = 0x0010_0000,
        NoActivationOrHellos = 0x0080_0000,
        DiffuseAlphaTest = 0x0100_0000,
        IsGhost = 0x2000_0000,
        Invulnerable = 0x8000_0000
    }

    public enum AggressionType
    {
        Unaggressive,
        Aggressive,
        VeryAggressive,
        Frenzied,
    }

    public enum ConfidenceType
    {
        Cowardly,
        Cautious,
        Average,
        Brave,
        Foolhardy,
    }

    public enum ResponsibilityType
    {
        AnyCrime,
        ViolenceAgainstEnemies,
        PropertyCrimeOnly,
        NoCrime,
    }

    public enum MoodType
    {
        Neutral,
        Angry,
        Fear,
        Happy,
        Sad,
        Surprised,
        Puzzled,
        Disgusted,
    }

    public enum AssistanceType
    {
        HelpsNobody,
        HelpsAllies,
        HelpsFriendsAndAllies,
    }

    public enum Property
    {
        Keywords,
        ForcedInventory,
        XpOffset,
        Enchantments,
        ColorRemappingIndex,
        MaterialSwaps,
    }

    public enum TemplateActorType
    {
        Traits = 0x1,
        Stats = 0x2,
        Factions = 0x4,
        SpellList = 0x8,
        AiData = 0x10,
        AiPackages = 0x20,
        ModelOrAnimation = 0x40,
        BaseData = 0x80,
        Inventory = 0x100,
        Script = 0x200,
        DefPackList = 0x400,
        AttackData = 0x800,
        Keywords = 0x1000,
    }
}

partial class NpcBinaryOverlay
{
}
