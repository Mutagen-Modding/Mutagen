using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.FalloutNV.Internals;

namespace Mutagen.Bethesda.FalloutNV;

partial class PatrolBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryPatrolScriptMarkerCustom(MutagenFrame frame, IPatrol item, PreviousParse lastParsed)
    {
        if (frame.ReadSubrecord().Content.Length != 0)
        {
            throw new ArgumentException($"Marker had unexpected length.");
        }

        return lastParsed;
    }
}

partial class PatrolBinaryWriteTranslation
{
    public static partial void WriteBinaryPatrolScriptMarkerCustom(MutagenWriter writer, IPatrolGetter item)
    {
        using (HeaderExport.Subrecord(writer, RecordTypes.XPPA)) { }
    }
}

partial class PatrolBinaryOverlay
{
    public partial ParseResult PatrolScriptMarkerCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        if (stream.ReadSubrecord().Content.Length != 0)
        {
            throw new ArgumentException($"Marker had unexpected length.");
        }

        return lastParsed;
    }
}
