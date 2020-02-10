using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class RecordHeaderConstants
    {
        public ObjectType ObjectType { get; }
        public GameMode GameMode { get; }
        public sbyte HeaderLength { get; }
        public sbyte LengthLength { get; }
        public sbyte LengthAfterLength { get; }
        public sbyte LengthAfterType { get; }
        public sbyte TypeAndLengthLength { get; }
        public bool HeaderIncludedInLength { get; }

        public RecordHeaderConstants(
            GameMode gameMode,
            ObjectType type,
            sbyte headerLength,
            sbyte lengthLength)
        {
            this.GameMode = gameMode;
            this.ObjectType = type;
            this.HeaderLength = headerLength;
            this.LengthLength = lengthLength;
            this.LengthAfterLength = (sbyte)(this.HeaderLength - Constants.HEADER_LENGTH - this.LengthLength);
            this.LengthAfterType = (sbyte)(this.HeaderLength - Constants.HEADER_LENGTH);
            this.TypeAndLengthLength = (sbyte)(Constants.HEADER_LENGTH + this.LengthLength);
            this.HeaderIncludedInLength = type == ObjectType.Group;
        }

        public VariableHeader VariableMeta(ReadOnlySpan<byte> span) => new VariableHeader(this, span);
        public VariableHeader GetVariableMeta(IBinaryReadStream stream, int offset = 0) => new VariableHeader(this, stream.GetSpan(this.HeaderLength, offset));
        public VariableHeader ReadVariableMeta(IBinaryReadStream stream) => new VariableHeader(this, stream.ReadSpan(this.HeaderLength));
    }
}
