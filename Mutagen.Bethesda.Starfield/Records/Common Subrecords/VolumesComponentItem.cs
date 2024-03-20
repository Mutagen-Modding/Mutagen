using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Starfield;

partial class VolumesComponentItemBinaryWriteTranslation
{
    public static partial void WriteBinaryTypeParseCustom(
        MutagenWriter writer,
        IVolumesComponentItemGetter item)
    {
        writer.Write(item.Ender switch
        {
            IVolumesUnknownEnderSingleGetter _ => 1,
            IVolumesUnknownEnderDoubleGetter _ => 3,
            IVolumesUnknownEnderTrioGetter _ => 5,
            _ => throw new NotImplementedException()
        });
    }
    
    public static partial void WriteBinaryEnderCustom(
        MutagenWriter writer,
        IVolumesComponentItemGetter item)
    {
        item.Ender.WriteToBinary(writer);
    }
}

partial class VolumesComponentItemBinaryCreateTranslation
{
    public static partial void FillBinaryTypeParseCustom(
        MutagenFrame frame,
        IVolumesComponentItem item)
    {
        var type = frame.ReadInt32();
        switch (type)
        {
            case 1:
                item.Ender = new VolumesUnknownEnderSingle();
                break;
            case 3:
                item.Ender = new VolumesUnknownEnderDouble();
                break;
            case 5:
                item.Ender = new VolumesUnknownEnderTrio();
                break;
            default:
                throw new NotImplementedException();
        }
    }
    
    public static partial void FillBinaryEnderCustom(
        MutagenFrame frame,
        IVolumesComponentItem item)
    {
        item.Ender.CopyInFromBinary(frame);
    }
}

partial class VolumesComponentItemBinaryOverlay
{
    partial void CustomEnderEndPos()
    {
        var val = BinaryPrimitives.ReadInt32LittleEndian(_structData);
        EnderEndingPos = 0x50;
        EnderEndingPos += val switch
        {
            1 => 4,
            3 => 8,
            5 => 12,
            _ => throw new NotImplementedException()
        };
    }
    
    public partial IAVolumesUnknownEnderGetter GetEnderCustom(int location)
    {
        var val = BinaryPrimitives.ReadInt32LittleEndian(_structData);
        return val switch
        {
            1 => VolumesUnknownEnderSingleBinaryOverlay.VolumesUnknownEnderSingleFactory(_structData.Slice(location),
                _package),
            3 => VolumesUnknownEnderDoubleBinaryOverlay.VolumesUnknownEnderDoubleFactory(_structData.Slice(location),
                _package),
            5 => VolumesUnknownEnderTrioBinaryOverlay.VolumesUnknownEnderTrioFactory(_structData.Slice(location),
                _package),
            _ => throw new NotImplementedException()
        };
    }
}