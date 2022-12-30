using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;

namespace Mutagen.Bethesda.Fallout4;

internal partial class DistantLodBinaryCreateTranslation
{
    public static partial void FillBinaryDataCustom(MutagenFrame frame, IDistantLod item)
    {
        item.Data = ByteArrayBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Parse(reader: frame);
    }
}

public partial class DistantLodBinaryWriteTranslation
{
    private const uint DataLength = 260;
    
    public static partial void WriteBinaryDataCustom(MutagenWriter writer, IDistantLodGetter item)
    {
        if (item.Data == null)
        {
            var len = DataLength - item.Mesh.Length - 1;
            if (len > 0)
            {
                writer.WriteZeros((uint)len);
            }
        }
        else
        {
            var len = item.Data.Value.Length + item.Mesh.Length + 1;
            if (len != DataLength)
            {
                throw new ArgumentException($"Distant Lod string and data size did not add up to expected length. {len} != 260");
            }
            writer.Write(item.Data.Value);
        }
    }
}

internal partial class DistantLodBinaryOverlay
{
    public partial ReadOnlyMemorySlice<Byte>? GetDataCustom(int location)
    {
        return _structData.Span.Slice(MeshEndingPos).ToArray();
    }
}