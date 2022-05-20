using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using System.Buffers.Binary;

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
    public partial UInt16 GetInterruptPercentageCustom()
    { 
        return _tnamOverride ?? (_InterruptPercentage_IsSet? BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(_InterruptPercentageLocation, 2)) : default);
    }

    private bool _InterruptPercentage_IsSet => _TRDALocation.HasValue;
    private int InterruptPercentageEndingPos => _TRDALocation!.Value.Min + 0xC;

    private UInt16? _tnamOverride;

    public partial ParseResult InterruptPercentageTNAMCustomParse(
        OverlayStream stream,
        int offset,
        PreviousParse lastParsed)
    {
        var tnam = stream.ReadSubrecord(RecordTypes.TNAM);
        // TNAM outranks TRDA
        _tnamOverride = tnam.AsUInt16();
        return (int)DialogResponse_FieldIndex.ListenerIdleAnimation;
    }
}