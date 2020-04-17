using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// A ref struct that overlays on top of bytes that is able to retrive basic header data on demand
    /// utilizing a given constants object to define the lengths
    /// </summary>
    public ref struct VariableHeader
    {
        /// <summary>
        /// Bytes overlaid onto
        /// </summary>
        public ReadOnlySpan<byte> Span { get; }
        
        /// <summary>
        /// Record metadata to use as reference for alignment
        /// </summary>
        public RecordHeaderConstants Constants { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="constants">Record constants to use as reference for alignment</param>
        /// <param name="span">Span to overlay on, aligned to the start of the header</param>
        public VariableHeader(RecordHeaderConstants constants, ReadOnlySpan<byte> span)
        {
            this.Constants = constants;
            this.Span = span.Slice(0, constants.HeaderLength);
        }

        /// <summary>
        /// The length that the header itself takes
        /// </summary>
        public sbyte HeaderLength => this.Constants.HeaderLength;
        
        public int RecordTypeInt => BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(0, 4));
        
        /// <summary>
        /// RecordType of the header
        /// </summary>
        public RecordType RecordType => new RecordType(this.RecordTypeInt);
        
        /// <summary>
        /// The length explicitly contained in the length bytes of the header
        /// </summary>
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
        
        /// <summary>
        /// The length of the RecordType and the length bytes
        /// </summary>
        public int TypeAndLengthLength => this.Constants.TypeAndLengthLength;
        
        /// <summary>
        /// Total length of the record, including the header and its content.
        /// </summary>
        public long TotalLength => this.Constants.HeaderIncludedInLength ? this.RecordLength : (this.HeaderLength + this.RecordLength);
        
        /// <summary>
        /// True if RecordType == "GRUP"
        /// </summary>
        public bool IsGroup => this.Constants.ObjectType == ObjectType.Group;
        
        /// <summary>
        /// The length of the content, excluding the header bytes.
        /// </summary>
        public long ContentLength => this.Constants.HeaderIncludedInLength ? this.RecordLength - this.HeaderLength : this.RecordLength;
    }
}
