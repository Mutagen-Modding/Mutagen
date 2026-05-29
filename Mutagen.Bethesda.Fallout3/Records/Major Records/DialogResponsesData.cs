using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Fallout3;

internal partial class DialogResponsesDataBinaryCreateTranslation
{
    public static partial void FillBinaryFlagsCustom(
        MutagenFrame frame,
        IDialogResponsesData item)
    {
        // Flags1 (low byte) is always present. Flags2 (high byte) is an optional trailing byte:
        // FO3 omits it when zero, so the DATA struct is 3 or 4 bytes. Read whichever is present.
        ushort value = frame.ReadUInt8();
        if (!frame.Complete)
        {
            value |= (ushort)(frame.ReadUInt8() << 8);
        }
        item.Flags = (DialogResponses.InfoFlag)value;
    }
}

public partial class DialogResponsesDataBinaryWriteTranslation
{
    public static partial void WriteBinaryFlagsCustom(
        MutagenWriter writer,
        IDialogResponsesDataGetter item)
    {
        // Always export the full 2-byte (Flags1 + Flags2) form. FO3 sometimes omits a zero Flags2
        // byte (3-byte DATA); the passthrough processor pads those to 4 bytes so the reference
        // matches this canonical output. (Read still accepts 1-or-2 bytes for real-world data.)
        writer.Write((ushort)item.Flags);
    }
}

internal partial class DialogResponsesDataBinaryOverlay
{
    public partial DialogResponses.InfoFlag GetFlagsCustom(int location)
    {
        var span = _structData.Span;
        ushort value = span[location];
        if (span.Length > location + 1)
        {
            value |= (ushort)(span[location + 1] << 8);
        }
        return (DialogResponses.InfoFlag)value;
    }
}
