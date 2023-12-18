using System.Buffers.Binary;
using System.Diagnostics;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Starfield.Internals;
using Noggog;

namespace Mutagen.Bethesda.Starfield;

public partial class Npc
{
    public const string ObjectModificationName = "TESNPC_InstanceData";
    
    [Flags]
    public enum MajorFlag
    {
        BleedoutOverride = 0x2000_0000
    }
    
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
        //PcLevelMult = 0x0000_0080,
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

    public ANpcLevel Level { get; set; } = new NpcLevel();
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IANpcLevelGetter INpcGetter.Level => Level;
}

partial class NpcBinaryCreateTranslation
{
    public const uint PcLevelMultFlag = 0x0000_0080;

    public static partial void FillBinaryFlagsCustom(MutagenFrame frame, INpcInternal item)
    {
        // Read normally
        item.Flags = (Npc.Flag)frame.ReadUInt32();
    }

    public static partial void FillBinaryLevelCustom(MutagenFrame frame, INpcInternal item)
    {
        if (Enums.HasFlag((uint)item.Flags, PcLevelMultFlag))
        {
            var raw = frame.ReadUInt16();
            float f = (float)raw;
            f /= 1000;
            item.Level = new PcLevelMult()
            {
                LevelMult = f
            };
        }
        else
        {
            item.Level = new NpcLevel()
            {
                Level = frame.ReadInt16()
            };
        }

        // Clear out PcLevelMult flag, as that information is kept in the field type above
        uint rawFlags = (uint)item.Flags;
        rawFlags &= ~PcLevelMultFlag;
        item.Flags = (Npc.Flag)rawFlags;
    }
}

partial class NpcBinaryWriteTranslation
{
    public static partial void WriteBinaryFlagsCustom(MutagenWriter writer, INpcGetter item)
    {
        // Add back PcLevelMult flag
        uint raw = (uint)item.Flags;
        switch (item.Level)
        {
            case IPcLevelMultGetter levelMult:
                raw |= NpcBinaryCreateTranslation.PcLevelMultFlag;
                break;
            case INpcLevelGetter level:
                raw &= ~NpcBinaryCreateTranslation.PcLevelMultFlag;
                break;
            default:
                throw new NotImplementedException();
        }
        writer.Write(raw);
    }

    public static partial void WriteBinaryLevelCustom(MutagenWriter writer, INpcGetter item)
    {
        switch (item.Level)
        {
            case IPcLevelMultGetter levelMult:
                var f = levelMult.LevelMult;
                f *= 1000;
                writer.Write((ushort)f);
                break;
            case INpcLevelGetter level:
                writer.Write(level.Level);
                break;
            default:
                throw new NotImplementedException();
        }
    }
}

partial class NpcBinaryOverlay
{
    private int? _templateLinksLocation;
    private int? _MSDKLocation;
    private int? _MSDVLocation;
    
    #region Level
    private int _LevelLocation => _ACBSLocation!.Value.Min + 0x6;
    public partial IANpcLevelGetter GetLevelCustom();
    public IANpcLevelGetter Level => GetLevelCustom();
    #endregion

    public partial Npc.Flag GetFlagsCustom()
    {
        uint rawFlags = BinaryPrimitives.ReadUInt32LittleEndian(_recordData.Slice(_FlagsLocation));
        // Clear out PcLevelMult flag, as that information is kept in the field type above
        rawFlags &= ~NpcBinaryCreateTranslation.PcLevelMultFlag;
        return (Npc.Flag)rawFlags;
    }

    public partial IANpcLevelGetter GetLevelCustom()
    {
        uint rawFlags = BinaryPrimitives.ReadUInt32LittleEndian(_recordData.Slice(_FlagsLocation));
        if (Enums.HasFlag(rawFlags, NpcBinaryCreateTranslation.PcLevelMultFlag))
        {
            var raw = BinaryPrimitives.ReadUInt16LittleEndian(_recordData.Slice(_LevelLocation));
            float f = (float)raw;
            f /= 1000;
            return new PcLevelMult()
            {
                LevelMult = f
            };
        }
        else
        {
            return new NpcLevel()
            {
                Level = BinaryPrimitives.ReadInt16LittleEndian(_recordData.Slice(_LevelLocation))
            };
        }
    }
}
