using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;

namespace Mutagen.Bethesda.Fallout4;

internal partial class DistantLodBinaryCreateTranslation
{
    public static partial void FillBinaryMeshCustom(MutagenFrame frame, IDistantLod item)
    {
        item.Mesh = StringBinaryTranslation.Instance.Parse(
            reader: frame,
            stringBinaryType: StringBinaryType.NullTerminate,
            parseWhole: false);
        if (frame.Complete) return;
        ByteArrayBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Parse(reader: frame);
    }
}

public partial class DistantLodBinaryWriteTranslation
{
    private const uint DataLength = 260;
    
    public static partial void WriteBinaryMeshCustom(MutagenWriter writer, IDistantLodGetter item)
    {
        var mesh = item.Mesh;
        StringBinaryTranslation.Instance.Write(
            writer: writer,
            item: mesh,
            binaryType: StringBinaryType.NullTerminate);
        writer.WriteZeros((uint)(DataLength - mesh.Length - 1));
    }
}

internal partial class DistantLodBinaryOverlay
{
    partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
    {
        Mesh = BinaryStringUtility.ParseUnknownLengthString(_structData, _package.MetaData.Encodings.NonTranslated);
    }
}