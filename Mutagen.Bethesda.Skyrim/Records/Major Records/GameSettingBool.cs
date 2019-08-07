using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;

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
                item.Data = (bool)(frame.GetInt32() != 0);
            }
        }

        public partial class GameSettingBoolBinaryWriteTranslation
        {
            static partial void WriteBinaryDataCustom(MutagenWriter writer, IGameSettingBoolInternalGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                writer.Write(item.Data ? 1 : 0);
            }
        }

        public partial class GameSettingBoolBinaryWrapper
        {
            bool GetDataIsSetCustom() => _DataLocation.HasValue;
            bool GetDataCustom() => _DataLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(HeaderTranslation.ExtractSubrecordSpan(_data, _DataLocation.Value, _package.Meta)) != 0 : default;
        }
    }
}
