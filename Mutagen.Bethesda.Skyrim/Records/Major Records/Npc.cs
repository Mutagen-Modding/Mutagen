using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using System;
using Mutagen.Bethesda.Skyrim.Internals;

namespace Mutagen.Bethesda.Skyrim;

public partial class Npc
{
    [Flags]
    public enum MajorFlag
    {
        BleedoutOverride = 0x2000_0000
    }
}
    
partial class NpcBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryDataMarkerCustom(MutagenFrame frame, INpcInternal item)
    {
        // Skip marker
        frame.ReadSubrecord();
        return null;
    }
}

partial class NpcBinaryWriteTranslation
{
    public static partial void WriteBinaryDataMarkerCustom(MutagenWriter writer, INpcGetter item)
    {
        using var header = HeaderExport.Subrecord(writer, RecordTypes.DATA);
    }
}

partial class NpcBinaryOverlay
{
    public partial ParseResult DataMarkerCustomParse(OverlayStream stream, int offset)
    {
        // Skip marker
        stream.ReadSubrecord();
        return null;
    }
}