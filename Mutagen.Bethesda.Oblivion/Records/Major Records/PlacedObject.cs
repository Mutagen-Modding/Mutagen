using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class PlacedObject
    {
        [Flags]
        public enum ActionFlag
        {
            UseDefault = 0x001,
            Activate = 0x002,
            Open = 0x004,
            OpenByDefault = 0x008
        }
    }

    namespace Internals
    {
        public partial class PlacedObjectBinaryCreateTranslation
        {
            static partial void FillBinaryOpenByDefaultCustom(MutagenFrame frame, IPlacedObjectInternal item)
            {
                item.OpenByDefault = true;
                frame.Position += frame.MetaData.Constants.SubConstants.HeaderLength;
            }
        }

        public partial class PlacedObjectBinaryWriteTranslation
        {
            static partial void WriteBinaryOpenByDefaultCustom(MutagenWriter writer, IPlacedObjectGetter item)
            {
                if (item.OpenByDefault)
                {
                    using (HeaderExport.Subrecord(writer, RecordTypes.ONAM))
                    {
                    }
                }
            }
        }

        public partial class PlacedObjectBinaryOverlay
        {
            private int? _OpenByDefaultLocation;
            public bool GetOpenByDefaultCustom() => _OpenByDefaultLocation.HasValue;
            partial void OpenByDefaultCustomParse(OverlayStream stream, long finalPos, int offset)
            {
                _OpenByDefaultLocation = (ushort)(stream.Position - offset);
            }
        }
    }
}
