using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using System.Buffers.Binary;
using Mutagen.Bethesda.Starfield.Internals;

namespace Mutagen.Bethesda.Starfield;

public partial class FindMatchingRefNearAlias
{
    public enum TypeEnum
    {
        LinkedFrom,
        LinkedRef,
    }
}

partial class FindMatchingRefNearAliasBinaryCreateTranslation
{
    public static partial void FillBinaryAliasIDCustom(MutagenFrame frame, IFindMatchingRefNearAlias item, PreviousParse lastParsed)
    {
        var subrecord = frame.ReadSubrecord();
        item.AliasID = checked((short)BinaryPrimitives.ReadInt32LittleEndian(subrecord.Content));
    }
}

partial class FindMatchingRefNearAliasBinaryWriteTranslation
{
    public static partial void WriteBinaryAliasIDCustom(MutagenWriter writer, IFindMatchingRefNearAliasGetter item)
    {
        if (item.AliasID is not { } ID) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.ALNA))
        {
            writer.Write((int)ID);
        }
    }
}

partial class FindMatchingRefNearAliasBinaryOverlay
{
    int? _aliasIDLoc;
    public partial Int16? GetAliasIDCustom() => _aliasIDLoc == null ? default(short?) : checked((short)BinaryPrimitives.ReadInt32LittleEndian(HeaderTranslation.ExtractSubrecordMemory(_recordData, _aliasIDLoc.Value, _package.MetaData.Constants)));

    partial void AliasIDCustomParse(OverlayStream stream, long finalPos, int offset)
    {
        _aliasIDLoc = stream.Position - offset;
    }
}