using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Oblivion;

public partial class ScriptMetaSummary
{
    internal int CompiledSizeInternal
    {
        set => this.CompiledSize = value;
    }
}
    
partial class ScriptMetaSummaryBinaryCreateTranslation
{
    public static partial void FillBinaryCompiledSizeCustom(MutagenFrame frame, IScriptMetaSummary item)
    {
        frame.Position += 4;
    }
}

partial class ScriptMetaSummaryBinaryWriteTranslation
{
    public static partial void WriteBinaryCompiledSizeCustom(MutagenWriter writer, IScriptMetaSummaryGetter item)
    {
        Int32BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
            writer,
            item.CompiledSize);
    }
}

partial class ScriptMetaSummaryBinaryOverlay
{
    public partial int GetCompiledSizeCustom(int location)
    {
        return BinaryPrimitives.ReadInt32LittleEndian(_structData.Span.Slice(location));
    }
}