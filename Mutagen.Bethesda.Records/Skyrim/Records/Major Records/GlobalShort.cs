using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive.Linq;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Noggog;
using Mutagen.Bethesda.Internals;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class GlobalShort
    {
        public const char TRIGGER_CHAR = 's';
        public override char TypeChar => TRIGGER_CHAR;

        public override float RawFloat
        {
            get => (float)this.Data;
            set
            {
                var val = (short)value;
                if (this.Data != val)
                {
                    this.Data = val;
                }
                else
                {
                    this.Data_IsSet = true;
                }
            }
        }

        internal static GlobalShort Factory()
        {
            return new GlobalShort();
        }
    }

    namespace Internals
    {
        public partial class GlobalShortBinaryWriteTranslation
        {
            static partial void WriteBinaryDataCustom(MutagenWriter writer, IGlobalShortGetter item, MasterReferences masterReferences)
            {
                using (HeaderExport.ExportSubRecordHeader(writer, GlobalShort_Registration.FLTV_HEADER))
                {
                    writer.Write((float)item.Data);
                }
            }
        }

        public partial class GlobalShortBinaryOverlay
        {
            public override char TypeChar => GlobalShort.TRIGGER_CHAR;
            public override float RawFloat => (float)this.Data;

            private int? _DataLocation;
            public bool GetDataIsSetCustom() => _DataLocation.HasValue;
            public short GetDataCustom()
            {
                return (short)HeaderTranslation.ExtractSubrecordSpan(_data.Span, _DataLocation.Value, _package.Meta).GetFloat();
            }
            partial void DataCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset)
            {
                _DataLocation = (ushort)(stream.Position - offset);
            }
        }
    }
}
