using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class ScriptMetaSummary
    {
        internal int CompiledSizeInternal
        {
            set => this.CompiledSize = value;
        }
    }

    namespace Internals
    {
        public partial class ScriptMetaSummaryBinaryCreateTranslation
        {
            public static partial void FillBinaryCompiledSizeCustom(MutagenFrame frame, IScriptMetaSummary item)
            {
                frame.Position += 4;
            }
        }

        public partial class ScriptMetaSummaryBinaryWriteTranslation
        {
            public static partial void WriteBinaryCompiledSizeCustom(MutagenWriter writer, IScriptMetaSummaryGetter item)
            {
                Int32BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
                    writer,
                    item.CompiledSize);
            }
        }

        public partial class ScriptMetaSummaryBinaryOverlay
        {
            public int GetCompiledSizeCustom(int location)
            {
                return BinaryPrimitives.ReadInt32LittleEndian(_data.Span.Slice(location));
            }
        }
    }
}
