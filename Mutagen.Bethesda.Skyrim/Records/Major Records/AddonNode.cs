using Mutagen.Bethesda.Plugins.Binary.Streams;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Skyrim;

partial class AddonNodeBinaryCreateTranslation
{
    public static partial void FillBinaryAlwaysLoadedCustom(MutagenFrame frame, IAddonNodeInternal item)
    {
        var flags = frame.ReadUInt16();
        item.AlwaysLoaded = flags switch 
        {
            1 => false,
            3 => true,
            _ => throw new NotImplementedException()
        };
    }
}

partial class AddonNodeBinaryWriteTranslation
{
    public static partial void WriteBinaryAlwaysLoadedCustom(MutagenWriter writer, IAddonNodeGetter item)
    {
        if (item.AlwaysLoaded)
        {
            writer.Write((short)3);
        }
        else
        {
            writer.Write((short)1);
        }
    }
}

partial class AddonNodeBinaryOverlay
{
    public partial Boolean GetAlwaysLoadedCustom() => BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(_AlwaysLoadedLocation)) switch
    {
        1 => false,
        3 => true,
        _ => throw new NotImplementedException()
    };
}