using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using System;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Npc
    {
        [Flags]
        public enum MajorFlag
        {
            BleedoutOverride = 0x2000_0000
        }
    }

    namespace Internals
    {
        public partial class NpcBinaryCreateTranslation
        {
            public static partial ParseResult FillBinaryDataMarkerCustom(MutagenFrame frame, INpcInternal item)
            {
                // Skip marker
                frame.ReadSubrecordFrame();
                return null;
            }
        }

        public partial class NpcBinaryWriteTranslation
        {
            public static partial void WriteBinaryDataMarkerCustom(MutagenWriter writer, INpcGetter item)
            {
                using var header = HeaderExport.Subrecord(writer, RecordTypes.DATA);
            }
        }

        public partial class NpcBinaryOverlay
        {
            public partial ParseResult DataMarkerCustomParse(OverlayStream stream, int offset)
            {
                // Skip marker
                stream.ReadSubrecordFrame();
                return null;
            }
        }
    }
}
