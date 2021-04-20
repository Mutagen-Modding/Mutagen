using BenchmarkDotNet.Attributes;
using Mutagen.Bethesda.Records.Binary.Streams;
using Mutagen.Bethesda.Records.Binary.Translations;
using System;

namespace Mutagen.Bethesda.Tests.Benchmarks
{
    public class MiscOverhead
    {
        static byte[] bytes = new byte[] { 1, 2, 3 };
        static MutagenFrame frame = new MutagenFrame(new MutagenMemoryReadStream(bytes, new ParsingBundle(GameRelease.Oblivion, masterReferences: null!)));
        static ByteBinaryTranslation transl = ByteBinaryTranslation.Instance;

        [Benchmark]
        public byte Direct()
        {
            frame.Position = 0;
            return frame.ReadUInt8();
        }

        [Benchmark]
        public byte TranslationInstance()
        {
            frame.Position = 0;
            if (ByteBinaryTranslation.Instance.Parse(
                frame: frame,
                item: out Byte MarksmanParse))
            {
                return MarksmanParse;
            }
            else
            {
                return default(byte);
            }
        }

        [Benchmark]
        public byte ParseValue()
        {
            frame.Position = 0;
            return ByteBinaryTranslation.Instance.ParseValue(frame);
        }

        [Benchmark]
        public byte InstanceDirect()
        {
            frame.Position = 0;
            return transl.ParseValue(frame);
        }

        [Benchmark]
        public byte Hmmm()
        {
            frame.Position = 0;
            return frame.Reader.ReadUInt8();
        }

        [Benchmark]
        public byte Hmmm2()
        {
            frame.Position = 0;
            return transl.Parse(frame);
        }
    }
}
