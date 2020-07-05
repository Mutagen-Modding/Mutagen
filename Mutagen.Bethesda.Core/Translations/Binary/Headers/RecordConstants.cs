using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// Reference for Record alignment and length constants
    /// </summary>
    public class RecordHeaderConstants
    {
        /// <summary>
        /// Type of object the constants are associated with
        /// </summary>
        public ObjectType ObjectType { get; }
        
        /// <summary>
        /// The length that the header itself takes
        /// </summary>
        public sbyte HeaderLength { get; }
        
        /// <summary>
        /// Number of bytes that hold length information
        /// </summary>
        public sbyte LengthLength { get; }
        
        /// <summary>
        /// Number of bytes in the header following the length information
        /// </summary>
        public sbyte LengthAfterLength { get; }
        
        /// <summary>
        /// Number of bytes in the header following the record type information
        /// </summary>
        public sbyte LengthAfterType { get; }
        
        /// <summary>
        /// Size of the record type and length bytes
        /// </summary>
        public sbyte TypeAndLengthLength { get; }
        
        /// <summary>
        /// Whether the size of the header itself is included in the length bytes, in addition to the content length
        /// </summary>
        public bool HeaderIncludedInLength { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type of object to associate the constants with</param>
        /// <param name="headerLength">Length of the header</param>
        /// <param name="lengthLength">Number of bytes containing content length information</param>
        public RecordHeaderConstants(
            ObjectType type,
            sbyte headerLength,
            sbyte lengthLength)
        {
            this.ObjectType = type;
            this.HeaderLength = headerLength;
            this.LengthLength = lengthLength;
            this.LengthAfterLength = (sbyte)(this.HeaderLength - Constants.HeaderLength - this.LengthLength);
            this.LengthAfterType = (sbyte)(this.HeaderLength - Constants.HeaderLength);
            this.TypeAndLengthLength = (sbyte)(Constants.HeaderLength + this.LengthLength);
            this.HeaderIncludedInLength = type == ObjectType.Group;
        }

        public VariableHeader VariableMeta(ReadOnlySpan<byte> span) => new VariableHeader(this, span);
        public VariableHeader GetVariableMeta(IBinaryReadStream stream, int offset = 0) => new VariableHeader(this, stream.GetSpan(this.HeaderLength, offset));
        public VariableHeader ReadVariableMeta(IBinaryReadStream stream) => new VariableHeader(this, stream.ReadSpan(this.HeaderLength));
    }
}
