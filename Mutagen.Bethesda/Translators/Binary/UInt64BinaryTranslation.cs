using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class UInt64BinaryTranslation : PrimitiveBinaryTranslation<ulong>
    {
        public readonly static UInt64BinaryTranslation Instance = new UInt64BinaryTranslation();
        public override int ExpectedLength => 8;

        public override ulong ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadUInt64();
        }

        public override void Write(MutagenWriter writer, ulong item)
        {
            writer.Write(item);
        }
    }
}
