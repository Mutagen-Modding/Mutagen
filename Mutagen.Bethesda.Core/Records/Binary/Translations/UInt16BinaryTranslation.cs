using Mutagen.Bethesda.Records.Binary.Streams;

namespace Mutagen.Bethesda.Records.Binary.Translations
{
    public class UInt16BinaryTranslation : PrimitiveBinaryTranslation<ushort>
    {
        public readonly static UInt16BinaryTranslation Instance = new UInt16BinaryTranslation();
        public override int ExpectedLength => 2;

        public override ushort Parse(MutagenFrame reader)
        {
            return reader.Reader.ReadUInt16();
        }

        public override void Write(MutagenWriter writer, ushort item)
        {
            writer.Write(item);
        }
    }
}
