﻿using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;

namespace Mutagen.Bethesda.Fallout3;

public partial class GlobalUnknown
{
    char IGlobalGetter.TypeChar => TypeChar;

    public override float? RawFloat
    {
        get => this.Data;
        set => this.Data = value;
    }
}

partial class GlobalUnknownBinaryCreateTranslation
{
    public static partial void FillBinaryTypeCharCustom(MutagenFrame frame, IGlobalUnknownInternal item, PreviousParse lastParsed)
    {
        frame.Position += frame.MetaData.Constants.SubConstants.HeaderLength;
        item.TypeChar = frame.ReadChar();
    }
}

partial class GlobalUnknownBinaryWriteTranslation
{
    public static partial void WriteBinaryTypeCharCustom(MutagenWriter writer, IGlobalUnknownGetter item)
    {
        // Handled elsewhere
    }
}

partial class GlobalUnknownBinaryOverlay
{
    char IGlobalGetter.TypeChar => TypeChar;
    public override float? RawFloat => this.Data;

    private int? _TypeCharLocation;

    partial void TypeCharCustomParse(OverlayStream stream, int finalPos, int offset)
    {
        _TypeCharLocation = (stream.Position - offset);
    }

    public partial char GetTypeCharCustom()
    {
        if (!_TypeCharLocation.HasValue) throw new MalformedDataException("Global had no FNAM record");
        return (char)HeaderTranslation.ExtractSubrecordMemory(_recordData, _TypeCharLocation.Value, _package.MetaData.Constants)[0];
    }
}