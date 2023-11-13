using Mutagen.Bethesda.Starfield.Internals;

namespace Mutagen.Bethesda.Starfield;

public partial class Npc
{
    public const string ObjectModificationName = "TESNPC_InstanceData";
    
    public enum Property
    {
        ActorValue = RecordTypeInts.NACV,
        NpcRaceOverride = RecordTypeInts.NARO,
        AIData = RecordTypeInts.NAID,
        CombatStyle = RecordTypeInts.NCST,
        Enchantment = RecordTypeInts.NENC,
        Faction = RecordTypeInts.NFAC,
        GroupFaction = RecordTypeInts.NGFA,
        Inventory = RecordTypeInts.NINV,
        DisplayName = RecordTypeInts.NNAM,
        Package = RecordTypeInts.NPAC,
        RaceOverride = RecordTypeInts.NRCO,
        VoiceType = RecordTypeInts.NVTP,
        ColorRemappingIndex = RecordTypeInts.NCOL,
        Keyword = RecordTypeInts.NKEY,
        LayeredMaterialSwap = RecordTypeInts.NMSL,
        MinMaxSize = RecordTypeInts.NMMX,
        Perk = RecordTypeInts.NPRK,
        RaceChange = RecordTypeInts.NRCE,
        ReactionRadius = RecordTypeInts.NREA,
        Skin = RecordTypeInts.NSKN,
        Spell = RecordTypeInts.NSPL,
        XPOverride = RecordTypeInts.NXPO,
    }
}