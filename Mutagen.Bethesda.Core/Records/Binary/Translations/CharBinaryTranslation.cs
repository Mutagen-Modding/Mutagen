using Mutagen.Bethesda.Records.Binary.Streams;

namespace Mutagen.Bethesda.Records.Binary.Translations
{
    public class CharBinaryTranslation : PrimitiveBinaryTranslation<char>
    {
        public readonly static CharBinaryTranslation Instance = new CharBinaryTranslation();
        public override int ExpectedLength => 1;

        public override char Parse(MutagenFrame reader)
        {
            return (char)reader.Reader.ReadUInt8();
        }

        public override void Write(MutagenWriter writer, char item)
        {
            writer.Write(item);
        }
    }
}
