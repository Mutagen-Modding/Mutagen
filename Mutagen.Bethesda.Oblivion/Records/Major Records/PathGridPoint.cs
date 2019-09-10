using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Oblivion.Internals
{
    public partial class PathGridPointBinaryWrapper
    {
        public List<short> Connections;
        IReadOnlyList<short> IPathGridPointGetter.Connections => Connections;

        public static PathGridPointBinaryWrapper Factory(
            ReadOnlyMemorySlice<byte> stream,
            BinaryWrapperFactoryPackage package)
        {
            return new PathGridPointBinaryWrapper(
                bytes: stream.Slice(0, 16),
                package: package);
        }
    }
}
