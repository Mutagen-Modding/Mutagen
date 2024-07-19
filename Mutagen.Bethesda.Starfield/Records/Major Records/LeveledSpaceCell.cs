using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Starfield.Internals;

namespace Mutagen.Bethesda.Starfield;

partial class LeveledSpaceCellBinaryCreateTranslation
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
    
    public static partial void FillBinaryChanceNoneCustom(MutagenFrame frame, ILeveledSpaceCellInternal item, PreviousParse lastParsed)
    {
        item.ChanceNone = GetChance(frame.ReadSubrecord(), frame.MetaData.FormVersion!.Value);
    }
}

partial class LeveledSpaceCellBinaryWriteTranslation
{
    public static void WriteBinaryChanceNoneCustom(MutagenWriter writer, float chanceNone, ushort formVersion)
    {
        using var s = HeaderExport.Subrecord(writer, RecordTypes.LVLD);
        if (formVersion < LeveledSpaceCellBinaryCreateTranslation.FloatChanceNoneVersion)
        {
            writer.Write((byte)(chanceNone * 255));
        }
        else
        {
            writer.Write(chanceNone);
        }
    }
    
    public static partial void WriteBinaryChanceNoneCustom(MutagenWriter writer, ILeveledSpaceCellGetter item)
    {
        WriteBinaryChanceNoneCustom(writer, item.ChanceNone, item.FormVersion);
    }
}

partial class LeveledSpaceCellBinaryOverlay
{
    private float _chanceNone;
    
    partial void ChanceNoneCustomParse(OverlayStream stream, int finalPos, int offset)
    {
        _chanceNone = LeveledSpaceCellBinaryCreateTranslation.GetChance(stream.ReadSubrecord(), this.FormVersion);
    }

    public partial float GetChanceNoneCustom()
    {
        return _chanceNone;
    }
}