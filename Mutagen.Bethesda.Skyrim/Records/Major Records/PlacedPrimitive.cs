using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;

namespace Mutagen.Bethesda.Skyrim;

public partial class PlacedPrimitive
{
    public enum TypeEnum
    {
        None,
        Box,
        Sphere,
        PortalBox,
    }
}

partial class PlacedPrimitiveBinaryCreateTranslation
{
    public static partial void FillBinaryBoundsCustom(MutagenFrame frame, IPlacedPrimitive item)
    {
        item.Bounds = new P3Float(
            frame.ReadFloat() * 2,
            frame.ReadFloat() * 2,
            frame.ReadFloat() * 2);
    }
}

partial class PlacedPrimitiveBinaryWriteTranslation
{
    public static partial void WriteBinaryBoundsCustom(MutagenWriter writer, IPlacedPrimitiveGetter item)
    {
        var bounds = item.Bounds;
        writer.Write(bounds.X / 2);
        writer.Write(bounds.Y / 2);
        writer.Write(bounds.Z / 2);
    }
}

partial class PlacedPrimitiveBinaryOverlay
{
    public partial P3Float GetBoundsCustom(int location)
    {
        return new P3Float(
            _structData.Slice(location).Float() * 2,
            _structData.Slice(location + 4).Float() * 2,
            _structData.Slice(location + 8).Float() * 2);
    }
}
