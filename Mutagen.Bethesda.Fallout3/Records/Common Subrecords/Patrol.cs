using Mutagen.Bethesda.Fallout3.Internals;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Fallout3;

public partial class PatrolBinaryWriteTranslation
{
    public static partial void WriteBinaryPatrolScriptMarkerCustom(
        MutagenWriter writer,
        IPatrolGetter item)
    {
        // XPPA is a marker subrecord with no data
        using (HeaderExport.Subrecord(writer, RecordTypes.XPPA)) { }
    }
}

internal partial class PatrolBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryPatrolScriptMarkerCustom(
        MutagenFrame frame,
        IPatrol item,
        PreviousParse lastParsed)
    {
        // XPPA is a marker subrecord - just skip it
        return lastParsed;
    }
}

internal partial class PatrolBinaryOverlay
{
    public partial ParseResult PatrolScriptMarkerCustomParse(
        OverlayStream stream,
        int offset,
        PreviousParse lastParsed)
    {
        // XPPA is a marker subrecord - just skip it
        return lastParsed;
    }
}
