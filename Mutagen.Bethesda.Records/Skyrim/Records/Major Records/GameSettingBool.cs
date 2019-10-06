using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
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
            static partial void FillBinaryDataCustom(MutagenFrame frame, GameSettingBool item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                var subFrame = frame.MetaData.ReadSubRecordFrame(frame);
                item.Data = (bool)(BinaryPrimitives.ReadUInt32LittleEndian(subFrame.ContentSpan) != 0);
            }
        }

        public partial class GameSettingBoolBinaryWriteTranslation
        {
            static partial void WriteBinaryDataCustom(MutagenWriter writer, IGameSettingBoolGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                using (HeaderExport.ExportSubRecordHeader(writer, GameSettingBool_Registration.DATA_HEADER))
                {
                    writer.Write(item.Data ? 1 : 0);
                }
            }
        }

        public partial class GameSettingBoolBinaryWrapper
        {
            private int? _DataLocation;
            bool GetDataIsSetCustom() => _DataLocation.HasValue;
            bool GetDataCustom() => _DataLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(HeaderTranslation.ExtractSubrecordSpan(_data, _DataLocation.Value, _package.Meta)) != 0 : default;
            partial void DataCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset)
            {
                _DataLocation = (ushort)(stream.Position - offset);
            }
        }
    }
}
