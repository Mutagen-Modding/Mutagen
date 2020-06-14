using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class FindMatchingRefNearAlias
    {
        public enum TypeEnum
        {
            LinkedRefChild 
        }
    }

    namespace Internals
    {
        public partial class FindMatchingRefNearAliasBinaryCreateTranslation
        {
            static partial void FillBinaryAliasIndexCustom(MutagenFrame frame, IFindMatchingRefNearAlias item)
            {
                var subrecord = frame.ReadSubrecordFrame();
                item.AliasIndex = checked((short)BinaryPrimitives.ReadInt32LittleEndian(subrecord.Content));
            }
        }

        public partial class FindMatchingRefNearAliasBinaryWriteTranslation
        {
            static partial void WriteBinaryAliasIndexCustom(MutagenWriter writer, IFindMatchingRefNearAliasGetter item)
            {
                if (!item.AliasIndex.TryGet(out var index)) return;
                using (HeaderExport.Subrecord(writer, RecordTypes.ALNA))
                {
                    writer.Write((int)index);
                }
            }
        }

        public partial class FindMatchingRefNearAliasBinaryOverlay
        {
            int? _aliasIndexLoc;
            public Int16? GetAliasIndexCustom() => _aliasIndexLoc == null ? default(short?) : checked((short)BinaryPrimitives.ReadInt32LittleEndian(HeaderTranslation.ExtractSubrecordSpan(_data, _aliasIndexLoc.Value, _package.MetaData.Constants)));

            partial void AliasIndexCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset)
            {
                _aliasIndexLoc = stream.Position - offset;
            }
        }
    }
}
