using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public class Int64BinaryTranslation : PrimitiveBinaryTranslation<long>
    {
        public readonly static Int64BinaryTranslation Instance = new Int64BinaryTranslation();
        public override byte? ExpectedLength => 8;

        protected override long ParseValue(BinaryReader reader)
        {
            return reader.ReadInt64();
        }

        protected override void WriteValue(BinaryWriter writer, long item)
        {
            writer.Write(item);
        }
    }
}
