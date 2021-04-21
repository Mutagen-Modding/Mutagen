using Mutagen.Bethesda.Records.Binary.Streams;

namespace Mutagen.Bethesda.Records.Binary.Translations
{
    public class Int8BinaryTranslation : PrimitiveBinaryTranslation<sbyte>
    {
        public readonly static Int8BinaryTranslation Instance = new Int8BinaryTranslation();
        public override int ExpectedLength => 1;

        public override sbyte Parse(MutagenFrame reader)
        {
            return reader.Reader.ReadInt8();
        }

        public override void Write(MutagenWriter writer, sbyte item)
        {
            writer.Write(item);
        }
    }
}
