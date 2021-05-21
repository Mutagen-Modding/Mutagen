using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Translations.Binary
{
    public class ByteBinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<byte, TReader, TWriter>
        where TReader : IBinaryReadStream
        where TWriter : IBinaryWriteStream
    {
        public readonly static ByteBinaryTranslation<TReader, TWriter> Instance = new();
        public override int ExpectedLength => 1;

        public override byte Parse(TReader reader)
        {
            return reader.ReadUInt8();
        }

        public override void Write(TWriter writer, byte item)
        {
            writer.Write(item);
        }
    }
}
