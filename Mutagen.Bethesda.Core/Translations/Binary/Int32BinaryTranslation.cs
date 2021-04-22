using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Translations.Binary
{
    public class Int32BinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<int, TReader, TWriter>
        where TReader : IBinaryReadStream
        where TWriter : IBinaryWriteStream
    {
        public readonly static Int32BinaryTranslation<TReader, TWriter> Instance = new();
        public override int ExpectedLength => 4;

        public override int Parse(TReader reader)
        {
            return reader.ReadInt32();
        }

        public override void Write(TWriter writer, int item)
        {
            writer.Write(item);
        }
    }
}
