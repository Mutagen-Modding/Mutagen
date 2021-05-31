using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Translations.Binary
{
    public class UInt32BinaryTranslation<TReader, TWriter> : PrimitiveBinaryTranslation<uint, TReader, TWriter>
        where TReader : IBinaryReadStream
        where TWriter : IBinaryWriteStream
    {
        public readonly static UInt32BinaryTranslation<TReader, TWriter> Instance = new();
        public override int ExpectedLength => 4;

        public override uint Parse(TReader reader)
        {
            return reader.ReadUInt32();
        }

        public override void Write(TWriter writer, uint item)
        {
            writer.Write(item);
        }
    }
}
