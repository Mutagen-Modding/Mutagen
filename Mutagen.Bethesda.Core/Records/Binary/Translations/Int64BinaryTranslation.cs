using Mutagen.Bethesda.Records.Binary.Streams;

namespace Mutagen.Bethesda.Records.Binary.Translations
{
    public class Int64BinaryTranslation : PrimitiveBinaryTranslation<long>
    {
        public readonly static Int64BinaryTranslation Instance = new Int64BinaryTranslation();
        public override int ExpectedLength => 8;

        public override long Parse(MutagenFrame reader)
        {
            return reader.Reader.ReadInt64();
        }

        public override void Write(MutagenWriter writer, long item)
        {
            writer.Write(item);
        }
    }
}
