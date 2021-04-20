using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Records.Binary.Overlay;
using Noggog;
using System;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class GlobalInt
    {
        public const char TRIGGER_CHAR = 'l';
        public override char TypeChar => TRIGGER_CHAR;

        public override float? RawFloat
        {
            get => this.Data.TryGet(out var data) ? (float)data : default;
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
    }

    namespace Internals
    {
        public partial class GlobalIntBinaryCreateTranslation
        {
            static partial void FillBinaryDataCustom(MutagenFrame frame, IGlobalIntInternal item)
            {
            }
        }

        public partial class GlobalIntBinaryWriteTranslation
        {
            static partial void WriteBinaryDataCustom(MutagenWriter writer, IGlobalIntGetter item)
            {
                if (!item.Data.TryGet(out var data)) return;
                using (HeaderExport.Subrecord(writer, RecordTypes.FLTV))
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
                return (int)HeaderTranslation.ExtractSubrecordMemory(_data, _DataLocation!.Value, _package.MetaData.Constants).Float();
            }
            partial void DataCustomParse(OverlayStream stream, long finalPos, int offset)
            {
                _DataLocation = (ushort)(stream.Position - offset);
            }
        }
    }
}
