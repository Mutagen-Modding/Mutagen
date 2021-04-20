using Mutagen.Bethesda.Records.Binary.Overlay;
using Noggog;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Oblivion.Internals
{
    public partial class PathGridPointBinaryOverlay
    {
        public List<short> Connections = new List<short>();
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
}
