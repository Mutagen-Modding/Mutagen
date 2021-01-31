using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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
            static partial void FillBinaryDataMarkerCustom(MutagenFrame frame, INpcInternal item)
            {
                // Skip marker
                frame.ReadSubrecordFrame();
            }
        }

        public partial class NpcBinaryWriteTranslation
        {
            static partial void WriteBinaryDataMarkerCustom(MutagenWriter writer, INpcGetter item)
            {
                using var header = HeaderExport.Subrecord(writer, RecordTypes.DATA);
            }
        }

        public partial class NpcBinaryOverlay
        {
            partial void DataMarkerCustomParse(OverlayStream stream, int offset)
            {
                // Skip marker
                stream.ReadSubrecordFrame();
            }
        }
    }
}
