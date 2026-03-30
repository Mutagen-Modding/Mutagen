using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout3;

public partial class Creature
{
    IFormLinkNullableGetter<IVoiceTypeGetter> IHasVoiceTypeGetter.Voice => Voice;

    [Flags]
    public enum MajorFlag
    {
        QuestItem = 0x0000_0400,
    }

    [Flags]
    public enum CreatureFlag : uint
    {
        Biped = 0x0000_0001,
        Essential = 0x0000_0002,
        WeaponAndShield = 0x0000_0004,
        Respawn = 0x0000_0008,
        Swims = 0x0000_0010,
        Flies = 0x0000_0020,
        Walks = 0x0000_0040,
        PCLevelMult = 0x0000_0080,
        NoLowLevelProcessing = 0x0000_0200,
        NoBloodSpray = 0x0000_0800,
        NoBloodDecal = 0x0000_1000,
        NoHead = 0x0000_8000,
        NoRightArm = 0x0001_0000,
        NoLeftArm = 0x0002_0000,
        NoCombatInWater = 0x0004_0000,
        NoShadow = 0x0008_0000,
        NoVATSMelee = 0x0010_0000,
        AllowPCDialogue = 0x0020_0000,
        CantOpenDoors = 0x0040_0000,
        Immobile = 0x0080_0000,
        TiltFrontBack = 0x0100_0000,
        TiltLeftRight = 0x0200_0000,
        NoKnockdowns = 0x0400_0000,
        NotPushable = 0x0800_0000,
        AllowPickpocket = 0x1000_0000,
        IsGhost = 0x2000_0000,
        NoRotatingToHeadTrack = 0x4000_0000,
        Invulnerable = 0x8000_0000,
    }

    [Flags]
    public enum TemplateFlag : ushort
    {
        Traits = 0x0001,
        Stats = 0x0002,
        Factions = 0x0004,
        ActorEffectList = 0x0008,
        AIData = 0x0010,
        AIPackages = 0x0020,
        ModelAnimation = 0x0040,
        BaseData = 0x0080,
        Inventory = 0x0100,
        Script = 0x0200,
    }

    public enum CreatureType : byte
    {
        Animal = 0,
        MutatedAnimal = 1,
        MutatedInsect = 2,
        Abomination = 3,
        SuperMutant = 4,
        FeralGhoul = 5,
        Robot = 6,
        Giant = 7,
    }

    public enum ImpactMaterial : uint
    {
        Stone = 0,
        Dirt = 1,
        Grass = 2,
        Glass = 3,
        Metal = 4,
        Wood = 5,
        Organic = 6,
        Cloth = 7,
        Water = 8,
        HollowMetal = 9,
        OrganicBug = 10,
        OrganicGlow = 11,
    }

    public enum SoundType : uint
    {
        LeftFoot = 0,
        RightFoot = 1,
        LeftBackFoot = 2,
        RightBackFoot = 3,
        Idle = 4,
        Aware = 5,
        Attack = 6,
        Hit = 7,
        Death = 8,
        Weapon = 9,
        MovementLoop = 10,
        ConsciousLoop = 11,
        Auxiliary1 = 12,
        Auxiliary2 = 13,
        Auxiliary3 = 14,
        Auxiliary4 = 15,
        Auxiliary5 = 16,
        Auxiliary6 = 17,
        Auxiliary7 = 18,
        Auxiliary8 = 19,
        Jump = 20,
        PlayRandomLoop = 21,
    }
}
