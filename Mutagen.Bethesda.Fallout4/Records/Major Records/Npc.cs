using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System.Buffers.Binary;
using System.Diagnostics;
using Mutagen.Bethesda.Fallout4.Internals;

namespace Mutagen.Bethesda.Fallout4;

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

    public enum AssistenceType
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
        if (EnumExt.HasFlag((uint)item.Flags, PcLevelMultFlag))
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

    public static partial ParseResult FillBinaryMorphParsingCustom(
        MutagenFrame frame,
        INpcInternal item)
    {
        var subrec = frame.ReadSubrecordHeader();
        switch (subrec.RecordTypeInt)
        {
            case RecordTypeInts.MSDK:
            {
                var amount = subrec.ContentLength / 4;
                for (int i = 0; i < amount; i++)
                {
                    if (!item.Morphs.InRange(i))
                    {
                        item.Morphs.Add(new NpcMorph());
                    }
                    item.Morphs[i].Key = frame.ReadUInt32();
                }
                break;
            }
            case RecordTypeInts.MSDV:
            {
                var amount = subrec.ContentLength / 4;
                for (int i = 0; i < amount; i++)
                {
                    if (!item.Morphs.InRange(i))
                    {
                        item.Morphs.Add(new NpcMorph());
                    }
                    item.Morphs[i].Value = frame.ReadFloat();
                }
                break;
            }
                break;
            default:
                throw new NotImplementedException();
        }
        return (int)Npc_FieldIndex.Morphs;
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

    public static partial void WriteBinaryMorphParsingCustom(MutagenWriter writer, INpcGetter item)
    {
        var morphs = item.Morphs;
        if (morphs.Count == 0) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.MSDK))
        {
            foreach (var morph in morphs)
            {
                writer.Write(morph.Key);
            }
        }
        using (HeaderExport.Subrecord(writer, RecordTypes.MSDV))
        {
            foreach (var morph in morphs)
            {
                writer.Write(morph.Value);
            }
        }
    }
}

partial class NpcBinaryOverlay
{
    private int? _templateLinksLocation;
    private int? _MSDKLocation;
    private int? _MSDVLocation;

    public partial Npc.Flag GetFlagsCustom()
    {
        uint rawFlags = BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(_FlagsLocation));
        // Clear out PcLevelMult flag, as that information is kept in the field type above
        rawFlags &= ~NpcBinaryCreateTranslation.PcLevelMultFlag;
        return (Npc.Flag)rawFlags;
    }

    public partial IANpcLevelGetter GetLevelCustom()
    {
        uint rawFlags = BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(_FlagsLocation));
        if (EnumExt.HasFlag(rawFlags, NpcBinaryCreateTranslation.PcLevelMultFlag))
        {
            var raw = BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(_LevelLocation));
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
                Level = BinaryPrimitives.ReadInt16LittleEndian(_data.Slice(_LevelLocation))
            };
        }
    }

    public partial ParseResult MorphParsingCustomParse(OverlayStream stream, int offset)
    {
        var subRec = stream.GetSubrecordHeader();
        switch (subRec.RecordTypeInt)
        {
            case RecordTypeInts.MSDK:
                _MSDKLocation = (stream.Position - offset);
                break;
            case RecordTypeInts.MSDV:
                _MSDVLocation = (stream.Position - offset);
                break;
            default:
                throw new NotImplementedException();
        }
        return (int)Npc_FieldIndex.Morphs;
    }

    public IReadOnlyList<INpcMorphGetter> Morphs
    {
        get
        {
            if (!_MSDVLocation.HasValue && !_MSDKLocation.HasValue) return Array.Empty<INpcMorphGetter>();
            ReadOnlyMemorySlice<byte> msdk = _MSDKLocation.HasValue ? HeaderTranslation.ExtractSubrecordMemory(_data, _MSDKLocation.Value, _package.MetaData.Constants) : Array.Empty<byte>();
            ReadOnlyMemorySlice<byte> msdv = _MSDVLocation.HasValue ? HeaderTranslation.ExtractSubrecordMemory(_data, _MSDVLocation.Value, _package.MetaData.Constants) : Array.Empty<byte>();
            var amount = Math.Max(msdk.Length, msdv.Length) / 4; 
            var ret = new List<INpcMorphGetter>(amount);
            for (int i = 0; i < amount; i++)
            {
                var item = new NpcMorph();
                var loc = i * 4;
                if (msdk.Length > loc)
                {
                    item.Key = BinaryPrimitives.ReadUInt32LittleEndian(msdk.Slice(loc));
                }
                if (msdv.Length > loc)
                {
                    item.Value = BinaryPrimitives.ReadSingleLittleEndian(msdv.Slice(loc));
                }
                ret.Add(item);
            }

            return ret;
        }
    }
}