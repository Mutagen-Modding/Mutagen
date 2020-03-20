using Mutagen.Bethesda.Internals;
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
        public MasterReferenceReader? MasterReferences { get; set; }

        public MutagenMemoryReadStream(
            byte[] data, 
            GameConstants metaData,
            MasterReferenceReader? masterReferences = null,
            long offsetReference = 0)
            : base(data)
        {
            this.MetaData = metaData;
            this.MasterReferences = masterReferences;
            this.OffsetReference = offsetReference;
        }

        public MutagenMemoryReadStream(
            ReadOnlyMemorySlice<byte> data, 
            GameConstants metaData,
            MasterReferenceReader? masterReferences = null,
            long offsetReference = 0)
            : base(data)
        {
            this.MetaData = metaData;
            this.MasterReferences = masterReferences;
            this.OffsetReference = offsetReference;
        }

        public IMutagenReadStream ReadAndReframe(int length)
        {
            return new MutagenMemoryReadStream(this.Data, this.MetaData, this.MasterReferences, this.OffsetReference + this.Position);
        }
    }
}
