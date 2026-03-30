namespace Mutagen.Bethesda.Fallout3;

public partial class Npc
{
    [Flags]
    public enum MajorFlag
    {
        QuestItem = 0x0000_0400,
    }

    [Flags]
    public enum NpcFlag : uint
    {
        Female = 0x0000_0001,
        Essential = 0x0000_0002,
        IsCharGenFacePreset = 0x0000_0004,
        Respawn = 0x0000_0008,
        AutoCalcStats = 0x0000_0010,
        PCLevelMult = 0x0000_0080,
        UseTemplate = 0x0000_0100,
        NoLowLevelProcessing = 0x0000_0200,
        NoBloodSpray = 0x0000_0800,
        NoBloodDecal = 0x0000_1000,
        NoVATSMelee = 0x0010_0000,
        CanBeAllRaces = 0x0040_0000,
        AutocalcService = 0x0080_0000,
        NoKnockdowns = 0x0400_0000,
        NotPushable = 0x0800_0000,
        NoRotatingToHeadTrack = 0x4000_0000,
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
        DefPackList = 0x0400,
        AttackData = 0x0800,
        Keywords = 0x1000,
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
}
