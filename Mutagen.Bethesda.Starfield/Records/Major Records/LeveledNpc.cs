using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Starfield.Internals;

namespace Mutagen.Bethesda.Starfield;

partial class LeveledNpc
{
    [Flags]
    public enum Flag
    {
        CalculateFromAllLevelsLessThanOrEqualPlayer = 0x01,
        CalculateForEachItemInCount = 0x02,
        CalculateAll = 0x04
    }
}

partial class LeveledNpcBinaryCreateTranslation
{
    public const int FloatChanceNoneVersion = 510;

    public static float GetChance(SubrecordFrame subrecord, ushort version)
    {
        if (version < FloatChanceNoneVersion)
        {
            var chance = subrecord.AsUInt8();
            return chance / 255f;
        }
        else
        {
            return subrecord.AsFloat();
        }
    }
    
    public static partial void FillBinaryChanceNoneCustom(MutagenFrame frame, ILeveledNpcInternal item, PreviousParse lastParsed)
    {
        item.ChanceNone = GetChance(frame.ReadSubrecord(), frame.MetaData.FormVersion!.Value);
    }
}

partial class LeveledNpcBinaryWriteTranslation
{
    public static void WriteBinaryChanceNoneCustom(MutagenWriter writer, float chanceNone, ushort formVersion)
    {
        using var s = HeaderExport.Subrecord(writer, RecordTypes.LVLD);
        if (formVersion < LeveledNpcBinaryCreateTranslation.FloatChanceNoneVersion)
        {
            writer.Write((byte)(chanceNone * 255));
        }
        else
        {
            writer.Write(chanceNone);
        }
    }
    
    public static partial void WriteBinaryChanceNoneCustom(MutagenWriter writer, ILeveledNpcGetter item)
    {
        WriteBinaryChanceNoneCustom(writer, item.ChanceNone, item.FormVersion);
    }
}

partial class LeveledNpcBinaryOverlay
{
    private float _chanceNone;
    
    partial void ChanceNoneCustomParse(OverlayStream stream, int finalPos, int offset)
    {
        _chanceNone = LeveledNpcBinaryCreateTranslation.GetChance(stream.ReadSubrecord(), this.FormVersion);
    }

    public partial float GetChanceNoneCustom()
    {
        return _chanceNone;
    }
}