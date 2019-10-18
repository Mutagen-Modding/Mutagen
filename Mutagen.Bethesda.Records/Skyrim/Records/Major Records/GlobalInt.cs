using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Skyrim.Internals;
using Noggog;
using Noggog.Notifying;
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

        public override float RawFloat
        {
            get => (float)this.Data;
            set
            {
                var val = (int)value;
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

        internal static GlobalInt Factory()
        {
            return new GlobalInt();
        }

        partial void CustomCtor()
        {
            this.WhenAny(x => x.RawFloat)
                .Skip(1)
                .DistinctUntilChanged()
                .Select(x => (int)Math.Round(x))
                .BindTo(this, x => x.Data);
            this.WhenAny(x => x.Data)
                .Skip(1)
                .DistinctUntilChanged()
                .Select(i => (float)i)
                .BindTo(this, x => x.RawFloat);
        }
    }

    namespace Internals
    {
        public partial class GlobalIntBinaryCreateTranslation
        {
            static partial void FillBinaryDataCustom(MutagenFrame frame, GlobalInt item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
            }
        }

        public partial class GlobalIntBinaryWriteTranslation
        {
            static partial void WriteBinaryDataCustom(MutagenWriter writer, IGlobalIntGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                using (HeaderExport.ExportSubRecordHeader(writer, GlobalInt_Registration.FLTV_HEADER))
                {
                    writer.Write((float)item.Data);
                }
            }
        }

        public partial class GlobalIntBinaryWrapper
        {
            public override char TypeChar => GlobalInt.TRIGGER_CHAR;
            public override float RawFloat => (float)this.Data;

            private int? _DataLocation;
            public bool GetDataIsSetCustom() => _DataLocation.HasValue;
            public int GetDataCustom()
            {
                return (int)HeaderTranslation.ExtractSubrecordSpan(_data.Span, _DataLocation.Value, _package.Meta).GetFloat();
            }
            partial void DataCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset)
            {
                _DataLocation = (ushort)(stream.Position - offset);
            }
        }
    }
}
