using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Noggog;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Oblivion;

internal partial class PathGridPointBinaryOverlay
{
    public List<short> Connections = new();
    IReadOnlyList<short> IPathGridPointGetter.Connections => Connections;

    public static PathGridPointBinaryOverlay Factory(
        ReadOnlyMemorySlice<byte> stream,
        BinaryOverlayFactoryPackage package)
    {
        return new PathGridPointBinaryOverlay(
            bytes: stream.Slice(0, 16),
            package: package);
    }
}