using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class Int64BinaryTranslation : PrimitiveBinaryTranslation<long>
    {
        public readonly static Int64BinaryTranslation Instance = new Int64BinaryTranslation();
        public override int? ExpectedLength => 8;

        protected override long ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadInt64();
        }

        protected override void WriteValue(MutagenWriter writer, long item)
        {
            writer.Write(item);
        }
    }
}
