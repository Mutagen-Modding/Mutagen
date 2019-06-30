using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class MutagenMemoryReadStream : BinaryMemoryReadStream, IMutagenReadStream
    {
        public long OffsetReference { get; }
        public MetaDataConstants MetaData { get; }

        public MutagenMemoryReadStream(byte[] data, MetaDataConstants metaData, long offsetReference = 0)
            : base(data)
        {
            this.MetaData = metaData;
            this.OffsetReference = offsetReference;
        }

        public MutagenMemoryReadStream(MemorySlice<byte> data, MetaDataConstants metaData, long offsetReference = 0)
            : base(data)
        {
            this.MetaData = metaData;
            this.OffsetReference = offsetReference;
        }

        public IMutagenReadStream ReadAndReframe(int length)
        {
            return new MutagenMemoryReadStream(this.Data, this.MetaData, this.OffsetReference + this.Position);
        }

        public string ReadZString(int length)
        {
            return BinaryStringUtility.ToZString(this.ReadSpan(length));
        }
    }
}
