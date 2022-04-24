using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using System;
using System.Buffers.Binary;
using Mutagen.Bethesda.Skyrim.Internals;

namespace Mutagen.Bethesda.Skyrim;

public partial class FindMatchingRefNearAlias
{
    public enum TypeEnum
    {
        LinkedRefChild 
    }
}

partial class FindMatchingRefNearAliasBinaryCreateTranslation
{
    public static partial void FillBinaryAliasIndexCustom(MutagenFrame frame, IFindMatchingRefNearAlias item)
    {
        var subrecord = frame.ReadSubrecord();
        item.AliasIndex = checked((short)BinaryPrimitives.ReadInt32LittleEndian(subrecord.Content));
    }
}

partial class FindMatchingRefNearAliasBinaryWriteTranslation
{
    public static partial void WriteBinaryAliasIndexCustom(MutagenWriter writer, IFindMatchingRefNearAliasGetter item)
    {
        if (item.AliasIndex is not {} index) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.ALNA))
        {
            writer.Write((int)index);
        }
    }
}

partial class FindMatchingRefNearAliasBinaryOverlay
{
    int? _aliasIndexLoc;
    public partial Int16? GetAliasIndexCustom() => _aliasIndexLoc == null ? default(short?) : checked((short)BinaryPrimitives.ReadInt32LittleEndian(HeaderTranslation.ExtractSubrecordMemory(_data, _aliasIndexLoc.Value, _package.MetaData.Constants)));

    partial void AliasIndexCustomParse(OverlayStream stream, long finalPos, int offset)
    {
        _aliasIndexLoc = stream.Position - offset;
    }
}