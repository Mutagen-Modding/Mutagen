using System;
using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;
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
            public static partial void FillBinaryDataCustom(MutagenFrame frame, IGameSettingBoolInternal item)
            {
                var subFrame = frame.ReadSubrecordFrame();
                item.Data = (bool)(BinaryPrimitives.ReadUInt32LittleEndian(subFrame.Content) != 0);
            }
        }

        public partial class GameSettingBoolBinaryWriteTranslation
        {
            public static partial void WriteBinaryDataCustom(MutagenWriter writer, IGameSettingBoolGetter item)
            {
                if (item.Data is not { } data) return;
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
