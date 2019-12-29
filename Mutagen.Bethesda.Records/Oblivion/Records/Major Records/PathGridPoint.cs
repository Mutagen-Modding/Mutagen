using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Oblivion.Internals
{
    public partial class PathGridPointBinaryOverlay
    {
        public List<short> Connections;
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
