using System;
using Mutagen.Bethesda.Binary;
using Noggog;
using Mutagen.Bethesda.Records.Binary.Overlay;
using Mutagen.Bethesda.Records.Binary.Streams;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class GlobalBool
    {
        public const char TRIGGER_CHAR = 'b';
        public override char TypeChar => TRIGGER_CHAR;

        public override float? RawFloat
        {
            get => this.Data ?? default ? 1 : 0;
            set
            {
                if (value.HasValue)
                {
                    this.Data = value.Value != 0;
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
        public partial class GlobalBoolBinaryCreateTranslation
        {
            static partial void FillBinaryDataCustom(MutagenFrame frame, IGlobalBoolInternal item)
            {
            }
        }

        public partial class GlobalBoolBinaryWriteTranslation
        {
            static partial void WriteBinaryDataCustom(MutagenWriter writer, IGlobalBoolGetter item)
            {
                if (!item.Data.TryGet(out var data)) return;
                using (HeaderExport.Subrecord(writer, RecordTypes.FLTV))
                {
                    writer.Write(data);
                }
            }
        }

        public partial class GlobalBoolBinaryOverlay
        {
            public override char TypeChar { get { return GlobalBool.TRIGGER_CHAR; } set { } }
            public override float? RawFloat => this.Data.TryGet(out var data) ? (data ? 1 : 0) : default;

            private int? _DataLocation;
            public bool GetDataIsSetCustom() => _DataLocation.HasValue;
            public bool GetDataCustom()
            {
                return HeaderTranslation.ExtractSubrecordMemory(_data, _DataLocation!.Value, _package.MetaData.Constants).Float() != 0;
            }
            partial void DataCustomParse(OverlayStream stream, long finalPos, int offset)
            {
                _DataLocation = (ushort)(stream.Position - offset);
            }
        }
    }
}
