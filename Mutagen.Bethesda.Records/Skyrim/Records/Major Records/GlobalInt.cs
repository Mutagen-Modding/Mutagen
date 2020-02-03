using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class GlobalInt
    {
        public const char TRIGGER_CHAR = 'l';
        public override char TypeChar => TRIGGER_CHAR;

        public override float? RawFloat
        {
            get => this.Data.HasValue ? (float)this.Data : default;
            set
            {
                if (value.HasValue)
                {
                    this.Data = (int)value.Value;
                }
                else
                {
                    this.Data = default;
                }
            }
        }

        internal static GlobalInt Factory()
        {
            return new GlobalInt();
        }
    }

    namespace Internals
    {
        public partial class GlobalIntBinaryCreateTranslation
        {
            static partial void FillBinaryDataCustom(MutagenFrame frame, IGlobalIntInternal item, MasterReferences masterReferences)
            {
            }
        }

        public partial class GlobalIntBinaryWriteTranslation
        {
            static partial void WriteBinaryDataCustom(MutagenWriter writer, IGlobalIntGetter item, MasterReferences masterReferences)
            {
                if (!item.Data.TryGet(out var data)) return;
                using (HeaderExport.ExportSubRecordHeader(writer, GlobalInt_Registration.FLTV_HEADER))
                {
                    writer.Write((float)data);
                }
            }
        }

        public partial class GlobalIntBinaryOverlay
        {
            public override char TypeChar => GlobalInt.TRIGGER_CHAR;
            public override float? RawFloat => this.Data.TryGet(out var data) ? (float)data : default;

            private int? _DataLocation;
            public bool GetDataIsSetCustom() => _DataLocation.HasValue;
            public int GetDataCustom()
            {
                return (int)HeaderTranslation.ExtractSubrecordSpan(_data.Span, _DataLocation!.Value, _package.Meta).GetFloat();
            }
            partial void DataCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset)
            {
                _DataLocation = (ushort)(stream.Position - offset);
            }
        }
    }
}
