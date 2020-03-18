using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class MutagenMemoryReadStream : BinaryMemoryReadStream, IMutagenReadStream
    {
        public long OffsetReference { get; }
        public GameConstants MetaData { get; }

        public MutagenMemoryReadStream(byte[] data, GameConstants metaData, long offsetReference = 0)
            : base(data)
        {
            this.MetaData = metaData;
            this.OffsetReference = offsetReference;
        }

        public MutagenMemoryReadStream(ReadOnlyMemorySlice<byte> data, GameConstants metaData, long offsetReference = 0)
            : base(data)
        {
            this.MetaData = metaData;
            this.OffsetReference = offsetReference;
        }

        public IMutagenReadStream ReadAndReframe(int length)
        {
            return new MutagenMemoryReadStream(this.Data, this.MetaData, this.OffsetReference + this.Position);
        }
    }
}
