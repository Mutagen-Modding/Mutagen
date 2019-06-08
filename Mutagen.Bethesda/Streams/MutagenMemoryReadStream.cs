using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class MutagenMemoryReadStream : BinaryMemoryReadStream, IMutagenReadStream
    {
        public long OffsetReference { get; }

        public MutagenMemoryReadStream(byte[] data, long offsetReference = 0)
            : base(data)
        {
            this.OffsetReference = offsetReference;
        }

        public MutagenMemoryReadStream(MemorySlice<byte> data, long offsetReference = 0)
            : base(data)
        {
            this.OffsetReference = offsetReference;
        }

        public IMutagenReadStream ReadAndReframe(int length)
        {
            return new MutagenMemoryReadStream(this.Data, this.OffsetReference + this.Position);
        }

        public string ReadZString(int length)
        {
            return BinaryStringUtility.ToZString(this.ReadSpan(length));
        }
    }
}
