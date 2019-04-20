using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class P2IntBinaryTranslation : PrimitiveBinaryTranslation<P2Int>
    {
        public readonly static P2IntBinaryTranslation Instance = new P2IntBinaryTranslation();
        public override int ExpectedLength => 8;

        public override P2Int ParseValue(MutagenFrame reader)
        {
            return new P2Int(
                reader.Reader.ReadInt32(),
                reader.Reader.ReadInt32());
        }

        public override void WriteValue(MutagenWriter writer, P2Int item)
        {
            writer.Write(item.X);
            writer.Write(item.Y);
        }
    }
}
