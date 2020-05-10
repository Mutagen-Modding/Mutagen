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
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequiredGetter.Name => this.Name ?? string.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequired.Name
        {
            get => this.Name ?? string.Empty;
            set => this.Name = value;
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IObjectBoundsGetter IObjectBoundedGetter.ObjectBounds => this.ObjectBounds;
        #endregion

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
                using var header = HeaderExport.ExportSubrecordHeader(writer, Npc_Registration.DATA_HEADER);
            }
        }

        public partial class NpcBinaryOverlay
        {
            #region Interfaces
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            String INamedRequiredGetter.Name => this.Name ?? string.Empty;
            #endregion

            partial void DataMarkerCustomParse(BinaryMemoryReadStream stream, int offset)
            {
                // Skip marker
                _package.Meta.ReadSubrecordFrame(stream);
            }
        }
    }
}
