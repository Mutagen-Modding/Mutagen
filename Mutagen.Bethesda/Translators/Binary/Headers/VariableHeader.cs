using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public ref struct VariableHeader
    {
        public ReadOnlySpan<byte> Span { get; }
        public RecordHeaderConstants Constants { get; }

        public VariableHeader(RecordHeaderConstants constants, ReadOnlySpan<byte> span)
        {
            this.Constants = constants;
            this.Span = span.Slice(0, constants.HeaderLength);
        }

        public sbyte HeaderLength => this.Constants.HeaderLength;
        public int RecordTypeInt => BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(0, 4));
        public RecordType RecordType => new RecordType(this.RecordTypeInt);
        public uint RecordLength
        {
            get
            {
                switch (this.Constants.LengthLength)
                {
                    case 1:
                        return this.Span[4];
                    case 2:
                        return BinaryPrimitives.ReadUInt16LittleEndian(this.Span.Slice(4, 2));
                    case 4:
                        return BinaryPrimitives.ReadUInt32LittleEndian(this.Span.Slice(4, 4));
                    default:
                        throw new NotImplementedException();
                }
            }
        }
        public int TypeAndLengthLength => this.Constants.TypeAndLengthLength;
        public long TotalLength => this.Constants.HeaderIncludedInLength ? this.RecordLength : (this.HeaderLength + this.RecordLength);
        public bool IsGroup => this.Constants.ObjectType == ObjectType.Group;
        public long ContentLength => this.Constants.HeaderIncludedInLength ? this.RecordLength - this.HeaderLength : this.RecordLength;
    }
}
