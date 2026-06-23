using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Fallout3;

partial class DialogResponseBinaryCreateTranslation
{
    public static partial void FillBinaryResponseDataCustom(
        MutagenFrame frame,
        IDialogResponse item,
        PreviousParse lastParsed)
    {
        // The frame is already scoped to TRDT header + content.
        // Read the subrecord header and create a struct-scoped sub-frame
        // so that the Break check (frame.Complete) works correctly for
        // variable-length TRDT (16, 20, or 24 bytes in FO3).
        var subHeader = frame.ReadSubrecordHeader();
        var structFrame = frame.SpawnWithLength(subHeader.ContentLength);
        DialogResponseDataBinaryCreateTranslation.FillBinaryStructs(item.ResponseData, structFrame);
        frame.Position = structFrame.Position;
    }
}

partial class DialogResponseBinaryWriteTranslation
{
    public static partial void WriteBinaryResponseDataCustom(
        MutagenWriter writer,
        IDialogResponseGetter item)
    {
        ((DialogResponseDataBinaryWriteTranslation)((IBinaryItem)item.ResponseData).BinaryWriteTranslator).Write(
            item: item.ResponseData,
            writer: writer,
            translationParams: default);
    }
}

partial class DialogResponseBinaryOverlay
{
    private IDialogResponseDataGetter? _responseData;

    partial void ResponseDataCustomParse(
        OverlayStream stream,
        int finalPos,
        int offset)
    {
        _responseData = DialogResponseDataBinaryOverlay.DialogResponseDataFactory(
            stream: stream,
            package: _package);
    }

    public partial IDialogResponseDataGetter GetResponseDataCustom()
    {
        return _responseData ?? new DialogResponseData();
    }
}
