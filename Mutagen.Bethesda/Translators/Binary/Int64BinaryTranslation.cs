using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class Int64BinaryTranslation : PrimitiveBinaryTranslation<long>
    {
        public readonly static Int64BinaryTranslation Instance = new Int64BinaryTranslation();
        public override int? ExpectedLength => 8;

        public override long ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadInt64();
        }

        public override void WriteValue(MutagenWriter writer, long item)
        {
            writer.Write(item);
        }
    }
}
