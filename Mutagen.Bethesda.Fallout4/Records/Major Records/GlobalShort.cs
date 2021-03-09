using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Noggog;
using Mutagen.Bethesda.Internals;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class GlobalShort
    {
        public const char TRIGGER_CHAR = 's';
        public override char TypeChar => TRIGGER_CHAR;

        public override float? RawFloat
        {
            get => this.Data.TryGet(out var data) ? (float)data : default;
            set
            {
                if (value.HasValue)
                {
                    this.Data = (short)value.Value;
                }
                else
                {
                    this.Data = default;
                }
            }
        }
    }

    namespace Internals
    {
        public partial class GlobalShortBinaryWriteTranslation
        {
            static partial void WriteBinaryDataCustom(MutagenWriter writer, IGlobalShortGetter item)
            {
                if (!item.Data.TryGet(out var data)) return;
                using (HeaderExport.Subrecord(writer, RecordTypes.FLTV))
                {
                    writer.Write((float)data);
                }
            }
        }

        public partial class GlobalShortBinaryOverlay
        {
            public override char TypeChar { get { return GlobalShort.TRIGGER_CHAR; } set { } }
            public override float? RawFloat => this.Data.HasValue ? (float)this.Data : default;

            private int? _DataLocation;
            public bool GetDataIsSetCustom() => _DataLocation.HasValue;
            public short GetDataCustom()
            {
                return (short)HeaderTranslation.ExtractSubrecordMemory(_data, _DataLocation!.Value, _package.MetaData.Constants).Float();
            }
            partial void DataCustomParse(OverlayStream stream, long finalPos, int offset)
            {
                _DataLocation = (ushort)(stream.Position - offset);
            }
        }
    }
}
