﻿using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Starfield;

public partial class LeveledItem
{
    [Flags]
    public enum MajorFlag
    {
        UseAll = 0x8000
    }

    [Flags]
    public enum Flag
    {
        CalculateFromAllLevelsLessThanOrEqualPlayer = 0x01,
        CalculateForEachItemInCount = 0x02,
        UseAll = 0x04,
        ShowAsMarker1 = 0x08,
        ShowAsMarker2 = 0x10,
        EvalAsStack = 0x20,
        GetChanceFromRequiredBiome = 0x80,
        DoAllBeforeRepeating = 0x100,
    }
}

partial class LeveledItemBinaryCreateTranslation
{
    public static partial void FillBinaryChanceNoneCustom(MutagenFrame frame, ILeveledItemInternal item, PreviousParse lastParsed)
    {
        item.ChanceNone = LeveledNpcBinaryCreateTranslation.GetChance(frame.ReadSubrecord(), frame.MetaData.FormVersion!.Value);
    }
}

partial class LeveledItemBinaryWriteTranslation
{
    public static partial void WriteBinaryChanceNoneCustom(MutagenWriter writer, ILeveledItemGetter item)
    {
        LeveledNpcBinaryWriteTranslation.WriteBinaryChanceNoneCustom(writer, item.ChanceNone, item.FormVersion);
    }
}

partial class LeveledItemBinaryOverlay
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