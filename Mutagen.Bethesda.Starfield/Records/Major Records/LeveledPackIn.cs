using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Starfield;

partial class LeveledPackInBinaryCreateTranslation
{
    public static partial void FillBinaryChanceNoneCustom(MutagenFrame frame, ILeveledPackInInternal item, PreviousParse lastParsed)
    {
        item.ChanceNone = LeveledNpcBinaryCreateTranslation.GetChance(frame.ReadSubrecord(), frame.MetaData.FormVersion!.Value);
    }
}

partial class LeveledPackInBinaryWriteTranslation
{
    public static partial void WriteBinaryChanceNoneCustom(MutagenWriter writer, ILeveledPackInGetter item)
    {
        LeveledNpcBinaryWriteTranslation.WriteBinaryChanceNoneCustom(writer, item.ChanceNone, item.FormVersion);
    }
}

partial class LeveledPackInBinaryOverlay
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