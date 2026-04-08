using System.Buffers.Binary;
using Mutagen.Bethesda.Fallout3.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;

namespace Mutagen.Bethesda.Fallout3;

partial class PlacedObjectBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryBoundDataCustom(MutagenFrame frame, IPlacedObjectInternal item, PreviousParse lastParsed)
    {
        var header = frame.ReadSubrecord();
        if (header.Content.Length != 4)
        {
            throw new ArgumentException($"Unexpected data header length: {header.Content.Length} != 4");
        }
        // First 2 bytes = linked rooms count, next 2 bytes = unknown
        item.Unknown = BinaryPrimitives.ReadInt16LittleEndian(header.Content.Slice(2));

        // Read XLRM linked rooms
        while (frame.Reader.TryReadSubrecordHeader(out var subHeader))
        {
            switch (subHeader.RecordTypeInt)
            {
                case RecordTypeInts.XLRM:
                    item.LinkedRooms.Add(new FormLink<IPlacedObjectGetter>(FormKeyBinaryTranslation.Instance.Parse(frame)));
                    break;
                default:
                    frame.Reader.Position -= subHeader.HeaderLength;
                    return null;
            }
        }
        return null;
    }
}

partial class PlacedObjectBinaryWriteTranslation
{
    public static partial void WriteBinaryBoundDataCustom(MutagenWriter writer, IPlacedObjectGetter item)
    {
        var linkedRooms = item.LinkedRooms;
        var unknown = item.Unknown;
        if (linkedRooms.Count == 0 && unknown == 0)
        {
            return;
        }
        using (HeaderExport.Subrecord(writer, RecordTypes.XRMR))
        {
            writer.Write((short)item.LinkedRooms.Count);
            writer.Write(item.Unknown);
        }
        foreach (var room in linkedRooms)
        {
            FormLinkBinaryTranslation.Instance.Write(writer, room, RecordTypes.XLRM);
        }
    }
}

partial class PlacedObjectBinaryOverlay
{
    int? _boundDataLoc;

    public short Unknown => _boundDataLoc.HasValue ? BinaryPrimitives.ReadInt16LittleEndian(_recordData.Slice(_boundDataLoc.Value + 8)) : default(short);

    public IReadOnlyList<IFormLinkGetter<IPlacedObjectGetter>> LinkedRooms { get; private set; } = Array.Empty<IFormLinkGetter<IPlacedObjectGetter>>();

    public partial ParseResult BoundDataCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        _boundDataLoc = stream.Position - offset;
        var header = stream.ReadSubrecord();
        if (header.Content.Length != 4)
        {
            throw new ArgumentException($"Unexpected data header length: {header.Content.Length} != 4");
        }

        // Read XLRM linked rooms
        while (stream.TryGetSubrecordHeader(out var subHeader))
        {
            switch (subHeader.RecordTypeInt)
            {
                case RecordTypeInts.XLRM:
                    LinkedRooms = BinaryOverlayList.FactoryByArray<IFormLinkGetter<IPlacedObjectGetter>>(
                        stream.RemainingMemory,
                        _package,
                        (s, p) => FormLinkBinaryTranslation.Instance.OverlayFactory<IPlacedObjectGetter>(p, s),
                        locs: ParseRecordLocations(
                            stream: stream,
                            trigger: RecordTypes.XLRM,
                            constants: _package.MetaData.Constants.SubConstants,
                            skipHeader: true));
                    return null;
                default:
                    return null;
            }
        }
        return null;
    }
}
