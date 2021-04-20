using System;
using System.Buffers.Binary;
using Mutagen.Bethesda.Records.Binary.Overlay;
using Mutagen.Bethesda.Records.Binary.Streams;
using Mutagen.Bethesda.Records.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class GameSettingBool
    {
        public override GameSettingType SettingType => GameSettingType.Bool;
    }

    namespace Internals
    {
        public partial class GameSettingBoolBinaryCreateTranslation
        {
            static partial void FillBinaryDataCustom(MutagenFrame frame, IGameSettingBoolInternal item)
            {
                var subFrame = frame.ReadSubrecordFrame();
                item.Data = (bool)(BinaryPrimitives.ReadUInt32LittleEndian(subFrame.Content) != 0);
            }
        }

        public partial class GameSettingBoolBinaryWriteTranslation
        {
            static partial void WriteBinaryDataCustom(MutagenWriter writer, IGameSettingBoolGetter item)
            {
                if (!item.Data.TryGet(out var data)) return;
                using (HeaderExport.Subrecord(writer, RecordTypes.DATA))
                {
                    writer.Write(data ? 1 : 0);
                }
            }
        }

        public partial class GameSettingBoolBinaryOverlay
        {
            private int? _DataLocation;
            bool GetDataIsSetCustom() => _DataLocation.HasValue;
            bool GetDataCustom() => _DataLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(HeaderTranslation.ExtractSubrecordMemory(_data, _DataLocation.Value, _package.MetaData.Constants)) != 0 : default;
            partial void DataCustomParse(OverlayStream stream, long finalPos, int offset)
            {
                _DataLocation = (ushort)(stream.Position - offset);
            }
        }
    }
}
