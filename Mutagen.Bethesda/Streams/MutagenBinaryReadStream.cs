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
        public MetaDataConstants MetaData { get; }

        public MutagenBinaryReadStream(string path, MetaDataConstants metaData, int bufferSize = 4096, long offsetReference = 0)
            : base(path, bufferSize)
        {
            this.MetaData = metaData;
            this.OffsetReference = offsetReference;
        }

        public MutagenBinaryReadStream(Stream stream, MetaDataConstants metaData, int bufferSize = 4096, bool dispose = true, long offsetReference = 0)
            : base(stream, bufferSize, dispose)
        {
            this.MetaData = metaData;
            this.OffsetReference = offsetReference;
        }

        public IMutagenReadStream ReadAndReframe(int length)
        {
            var offset = this.OffsetReference + this.Position;
            return new MutagenMemoryReadStream(this.ReadBytes(length), this.MetaData, offsetReference: offset);
        }

        public string ReadZString(int length)
        {
            return BinaryStringUtility.ToZString(this.ReadSpan(length));
        }
    }
}
