using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class UInt64BinaryTranslation : PrimitiveBinaryTranslation<ulong>
    {
        public readonly static UInt64BinaryTranslation Instance = new UInt64BinaryTranslation();
        public override ContentLength? ExpectedLength => new ContentLength(8);

        protected override ulong ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadUInt64();
        }

        protected override void WriteValue(MutagenWriter writer, ulong item)
        {
            writer.Write(item);
        }
    }
}
