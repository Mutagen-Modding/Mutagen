using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class MutagenBinaryReadStream : BinaryReadStream, IMutagenReadStream
    {
        public long OffsetReference { get; }

        public MutagenBinaryReadStream(string path, int bufferSize = 4096, long offsetReference = 0)
            : base(path, bufferSize)
        {
            this.OffsetReference = offsetReference;
        }

        public MutagenBinaryReadStream(Stream stream, int bufferSize = 4096, bool dispose = true, long offsetReference = 0)
            : base(stream, bufferSize, dispose)
        {
            this.OffsetReference = offsetReference;
        }
    }
}
