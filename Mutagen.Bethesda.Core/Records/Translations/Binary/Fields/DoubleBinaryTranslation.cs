using Mutagen.Bethesda.Records.Binary.Streams;

namespace Mutagen.Bethesda.Binary
{
    public class DoubleBinaryTranslation : PrimitiveBinaryTranslation<double>
    {
        public readonly static DoubleBinaryTranslation Instance = new DoubleBinaryTranslation();
        public override int ExpectedLength => 8;

        public override double ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadDouble();
        }

        public override void Write(MutagenWriter writer, double item)
        {
            writer.Write(item);
        }
    }
}
