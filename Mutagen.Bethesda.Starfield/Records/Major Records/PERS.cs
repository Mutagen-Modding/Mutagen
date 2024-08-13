using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Starfield.Internals;
using NexusMods.Paths.Trees.Traits;

namespace Mutagen.Bethesda.Starfield;

partial class PERSBinaryCreateTranslation
{
    internal static readonly IReadOnlyRecordCollection Targets = RecordCollection.Factory(RecordTypes.XXXX, RecordTypes.DAT2);
    
    public static partial ParseResult FillBinaryItemsParseCustom(
        MutagenFrame frame,
        IPERSInternal item,
        PreviousParse lastParsed)
    {
        frame = frame.SpawnAll();
        item.Items.Clear();
        int? contentLen = null;
        while (frame.TryGetSubrecord(Targets, out var subRec))
        {
            if (subRec.RecordType == RecordTypes.XXXX)
            {
                contentLen = subRec.AsInt32();
                frame.Position += subRec.TotalLength;
            }
            else
            {
                if (contentLen == null)
                {
                    contentLen = subRec.ContentLength;
                }
                frame.Position += subRec.HeaderLength;

                var items = Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<PERSItem>.Instance.Parse(
                    reader: frame.SpawnWithLength(contentLen.Value),
                    transl: PERSItem.TryCreateFromBinary);
                item.Items.Add(new PERSItems()
                {
                    Items = items
                });
            }
        }
        
        return (int)PERS_FieldIndex.Items;
    }
}

partial class PERSBinaryWriteTranslation
{
    public static partial void WriteBinaryItemsParseCustom(
        MutagenWriter writer,
        IPERSGetter item)
    {
        foreach (var x in item.Items)
        {
            using (HeaderExport.Subrecord(writer, RecordTypes.DAT2,
                overflowRecord: RecordTypes.XXXX,
                out var writerToUse))
            {
                x.WriteToBinary(writerToUse);
            }
        }
    }
}

partial class PERSBinaryOverlay
{
    public IReadOnlyList<IPERSItemsGetter> Items { get; private set; } = Array.Empty<IPERSItemsGetter>();

    public partial ParseResult ItemsParseCustomParse(
        OverlayStream stream,
        int offset,
        PreviousParse lastParsed)
    {
        var itemsList = new List<IPERSItemsGetter>();
        int? contentLen = null;
        while (stream.TryGetSubrecord(PERSBinaryCreateTranslation.Targets, out var subRec))
        {
            if (subRec.RecordType == RecordTypes.XXXX)
            {
                contentLen = subRec.AsInt32();
                stream.Position += subRec.TotalLength;
            }
            else
            {
                if (contentLen == null)
                {
                    contentLen = subRec.ContentLength;
                }

                stream.Position += subRec.HeaderLength;
                
                itemsList.Add(PERSItemsBinaryOverlay.PERSItemsFactory(stream.ReadMemory(contentLen.Value), _package));
            }
        }

        Items = itemsList;
        
        return (int)PERS_FieldIndex.Items;
    }
}