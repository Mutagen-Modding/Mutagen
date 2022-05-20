using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Fallout4;

partial class DialogResponseBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryInterruptPercentageTNAMCustom(
        MutagenFrame frame,
        IDialogResponse item,
        PreviousParse lastParsed)
    {
        var tnam = frame.ReadSubrecord(RecordTypes.TNAM);
        // TNAM outranks TRDA
        item.InterruptPercentage = tnam.AsUInt16();
        return (int)DialogResponse_FieldIndex.ListenerIdleAnimation;
    }
}

partial class DialogResponseBinaryWriteTranslation
{
    public static partial void WriteBinaryInterruptPercentageTNAMCustom(MutagenWriter writer, IDialogResponseGetter item)
    {
        var perc = item.InterruptPercentage;
        if (perc == 0) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.TNAM))
        {
            writer.Write(perc);
        }
    }
}

partial class DialogResponseBinaryOverlay
{
    public partial ParseResult InterruptPercentageTNAMCustomParse(
        OverlayStream stream,
        int offset,
        PreviousParse lastParsed)
    {
        throw new NotImplementedException();
    }
}