using Mutagen.Bethesda.Records.Binary.Streams;

namespace Mutagen.Bethesda.Records.Binary.Translations
{
    public class ByteBinaryTranslation : PrimitiveBinaryTranslation<byte>
    {
        public readonly static ByteBinaryTranslation Instance = new ByteBinaryTranslation();
        public override int ExpectedLength => 1;

        public override byte ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadUInt8();
        }

        public override void Write(MutagenWriter writer, byte item)
        {
            writer.Write(item);
        }
    }
}
