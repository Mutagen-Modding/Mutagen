using System;
using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

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
            public static partial void FillBinaryDataCustom(MutagenFrame frame, IGlobalBoolInternal item)
            {
            }
        }

        public partial class GlobalBoolBinaryWriteTranslation
        {
            public static partial void WriteBinaryDataCustom(MutagenWriter writer, IGlobalBoolGetter item)
            {
                if (item.Data is not { } data) return;
                using (HeaderExport.Subrecord(writer, RecordTypes.FLTV))
                {
                    writer.Write(data);
                }
            }
        }

        public partial class GlobalBoolBinaryOverlay
        {
            public override char TypeChar { get { return GlobalBool.TRIGGER_CHAR; } set { } }
            public override float? RawFloat => this.Data is { } data ? (data ? 1 : 0) : default;
            
            private int? _DataLocation;
            public bool GetDataIsSetCustom() => _DataLocation.HasValue;
            public partial bool? GetDataCustom()
            {
                if (!_DataLocation.HasValue) return null;
                return HeaderTranslation.ExtractSubrecordMemory(_data, _DataLocation.Value, _package.MetaData.Constants).Float() != 0;
            }
            partial void DataCustomParse(OverlayStream stream, long finalPos, int offset)
            {
                _DataLocation = (ushort)(stream.Position - offset);
            }
        }
    }
}
